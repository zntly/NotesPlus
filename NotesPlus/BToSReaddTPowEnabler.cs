using HarmonyLib;
using Home.HomeScene;

namespace NotesPlus
{
    [HarmonyPatch(typeof(HomeSceneController), "Start")]
    public class BToSReaddTPowEnabler
    {
        [HarmonyPostfix]
        public static void Postfix()
        {
            if (!doIt)
            {
                doIt = true;
                if (Utils.BTOS2Exists())
                    RunReadder();
            }
        }
        private static void RunReadder()
        {
            BToSReaddTPow.DoYourThing();
        }
        public static bool doIt = false;
    }
}
