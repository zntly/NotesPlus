using HarmonyLib;
using Home.HomeScene;
using Services;

namespace NotesPlus
{
    [HarmonyPatch(typeof(HomeSceneController), "Start")]
    public class BToSReaddTPowEnabler
    {
        [HarmonyPostfix]
        public static void Postfix()
        {
            if (Service.Game?.Roles != null)
            {
                BToSReaddTPow.DoYourThing();
            }
        }
    }
}