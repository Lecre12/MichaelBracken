using BepInEx.Logging;
using BepInEx;
using HarmonyLib;
using HEHE.Patch;

namespace HEHE
{
    [BepInPlugin(modGUID, modName, modVersion)]
    public class HeheBase : BaseUnityPlugin
    {
        private const string modGUID = "Lecre.HeheMod";
        private const string modName = "LC HEHEMod";
        private const string modVersion = "1.0.0.0";

        private readonly Harmony harmony = new Harmony(modGUID);

        public static HeheBase instance;

        internal ManualLogSource mls;


        void Awake()
        {

            if (instance == null)
            {
                instance = this;
            }
            mls = BepInEx.Logging.Logger.CreateLogSource(modGUID);
            mls.LogInfo("Lecre.ConchaSuMareMod is loading");

            harmony.PatchAll(typeof(HeheBase));
            harmony.PatchAll(typeof(HeheSound));
        }

    }
}
