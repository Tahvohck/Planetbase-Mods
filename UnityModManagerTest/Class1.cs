using Planetbase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityModManagerNet;

namespace UnityModManagerTest
{
    using ModEntry = UnityModManager.ModEntry;
    using ModLogger = UnityModManager.ModEntry.ModLogger;

    [EnableReloading]
    public static class TestMod
    {
        static int idxFixedUpdate = 0;
        static int idxUpdate = 0;

        public static void Load(ModEntry entryData)
        {
            entryData.Logger.Log($"Loaded from {entryData.Path}. [{entryData.Assembly.FullName}]" +
                $"\nCan be reloaded: {entryData.CanReload}");
            entryData.OnToggle = Toggle;
            entryData.OnUnload = Unload;
            entryData.OnFixedUpdate = FixedUpdate;
            entryData.OnUpdate = Update;
            entryData.OnShowGUI = ShowGUI;
            entryData.OnHideGUI = HideGUI;
            entryData.OnSaveGUI = SaveGUI;
            entryData.Logger.Log(GameManager.getInstance().getGameState().ToString());
        }

        private static void SaveGUI(ModEntry entryData)
        {
            if (idxUpdate >= 3) return;

            entryData.Logger.Log($"GUI Save");
        }

        private static void HideGUI(ModEntry entryData)
        {
            if (idxUpdate >= 3) return;

            entryData.Logger.Log($"GUI save");
        }

        private static void ShowGUI(ModEntry entryData)
        {
            if (idxUpdate >= 3) return;

            entryData.Logger.Log($"GUI show");
        }

        private static void Update(ModEntry entryData, float tDelta)
        {
            if (idxUpdate >= 3) return;

            entryData.Logger.Log($"Update thread: IDX {idxUpdate++}, arg2 {tDelta}");
        }


        private static void FixedUpdate(ModEntry entryData, float tDelta)
        {
            if (idxFixedUpdate >= 3) return;

            entryData.Logger.Log($"FixedUpdate thread: IDX {idxFixedUpdate++}, arg2 {tDelta}");
        }

        /// <summary> An example of mod toggling. </summary>
        /// <param name="onState">True: Enable mod, False: Disable Mod</param>
        /// <returns>True if the toggle was successful.</returns>
        public static bool Toggle(ModEntry entry, bool onState)
        {
            ModLogger logger = entry.Logger;
            logger.Log($"Toggled {onState}");
            return true;
        }

        public static bool Unload(ModEntry entry)
        {
            entry.Logger.Log("Unloading...");
            return true;
        }
    }
}
