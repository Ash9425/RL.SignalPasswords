using BepInEx;
using BepInEx.Logging;
using HarmonyLib;


namespace SignalPasswords
{
    [BepInPlugin(MOD_GUID, MOD_Name, MOD_Version)]
    public class Plugin : BaseUnityPlugin
    {
        private const string MOD_GUID = "SignalPasswords.ash.spotfire.dev";
        private const string MOD_Name = "Ash.SignalPasswords";
        private const string MOD_Version = "1.0.0";
        //private readonly Harmony _harmony = new Harmony(MOD_GUID);
        internal static ManualLogSource mls = BepInEx.Logging.Logger.CreateLogSource(MOD_GUID);


        private void Awake()
        {
            Harmony harmony = new Harmony("Ash.SignalPasswords");
            harmony.PatchAll();
            Logger.LogInfo("SignalPasswords patch by Ash has been applied.");
            mls.LogInfo("ManualLogSource Awake");
        }


        public static void SendLog(string msg)
        {
            mls.LogInfo(msg);
        }
    }
}