using Planetbase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using UnityModManagerNet;


namespace CharacterCam
{
    public class CharacterCam
    {
        
        [LoaderOptimization(LoaderOptimization.NotSpecified)]
        public static void Init(UnityModManager.ModEntry modData)
        {
            modData.OnUpdate = Update;
            Debug.Log("[MOD] CharacterCam activated");
        }

        public static void Update(UnityModManager.ModEntry modData, float timeStep) 
        {
            
        }
    }

    public abstract class CustomCharacter : Character
    {
        public override bool isCloseCameraAvailable()
        {
            return true;
        }
    }

    public abstract class CustomCloseCameraCinematic : CloseCameraCinematic
    {
        private CustomCloseCameraCinematic(Selectable s, bool b) : base(s, b) { }

        public void UpdateCharacter(Character character, float timeStep)
        {
            Transform cameraTransform = CameraManager.getInstance().getTransform();
            Transform characterTransform = character.getTransform();

            double yAngle = characterTransform.eulerAngles.y;
            FieldInfo fi = typeof(CloseCameraCinematic).GetField("mLastRotation");
            double fiC = Convert.ToDouble(fi);
            Debug.Log(fiC);
            float horizontalBobbing = Mathf.Clamp((float)((Convert.ToDouble(fi) - yAngle) * 0.25f), -0.5f, 0.5f);
            Vector3 newPos = characterTransform.position + Vector3.up * character.getHeight() + characterTransform.forward * 0.7f + horizontalBobbing * characterTransform.right;
            FieldInfo fi2 = typeof(CloseCameraCinematic).GetField("mFirstUpdate");
            Debug.Log(fi2);
            if (Convert.ToBoolean(fi2) == true)
            {
                cameraTransform.position = newPos;
                cameraTransform.rotation = characterTransform.rotation;
                fiC = yAngle;
                Debug.Log(fiC);
            }

            cameraTransform.position = Vector3.Lerp(cameraTransform.position, newPos, 0.1f);
            Vector3 lookAtDir = (characterTransform.position + characterTransform.forward * 1.4f + Vector3.up * (character.getHeight() * 0.85f) - cameraTransform.position).normalized;
            cameraTransform.rotation = Quaternion.RotateTowards(cameraTransform.rotation, Quaternion.LookRotation(lookAtDir), timeStep * 120f);
            fiC = yAngle;
            Debug.Log(fiC);
        }
    }
}
