using HarmonyLib;
using Planetbase;
using System;
using System.Collections.Generic;
using UnityEngine;
using static UnityModManagerNet.UnityModManager;
using System.Reflection;
using PlanetbaseModUtilities;

namespace CharacterCam
{
    public class CharacterCam : ModBase
    {
        public static new void Init(ModEntry modEntry) => InitializeMod(new CharacterCam(), modEntry, "CharacterCam");

        public override void OnInitialized(ModEntry modEntry)
		{
            StringUtils.RegisterString("CharacterCam", "Character Cam");
            Debug.Log("[MOD] CharacterCam activated");
        }

		public override void OnUpdate(ModEntry modEntry, float timeStep)
		{
            
        }

    }
    [HarmonyPatch(typeof(Character), nameof(Character.isCloseCameraAvailable))]
    public class CharacterPatch
    {
        public static bool Prefix(ref bool __result)
        {
            __result = true;
            return false;
        }
    }
    [HarmonyPatch(typeof(CloseCameraCinematic), nameof(CloseCameraCinematic.updateCharacter))]
    public class CloseCameraCinematicPatch
    {
        private CloseCameraCinematicPatch(Selectable s, bool b) : base() { }
        public static bool Prefix(Character character, float timeStep)
        {
            
            Transform cameraTransform = CameraManager.getInstance().getTransform();
            Transform characterTransform = character.getTransform();

            double yAngle = characterTransform.eulerAngles.y;
            FieldInfo lastRotation = typeof(CloseCameraCinematic).GetField("mLastRotation");
            double fiC = Convert.ToDouble(lastRotation);
            Debug.Log(fiC);
            float horizontalBobbing = Mathf.Clamp((float)((Convert.ToDouble(lastRotation) - yAngle) * 0.25f), -0.5f, 0.5f);
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

            return false;
        }
    }
}
