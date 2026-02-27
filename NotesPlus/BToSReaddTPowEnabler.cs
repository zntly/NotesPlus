using HarmonyLib;
using Home.HomeScene;
using Services;

namespace NotesPlus
{
    [HarmonyPatch(typeof(HomeSceneController), "Start")]
	public class BToSReaddTPowEnabler
	{
		private static bool didRun = false;

		[HarmonyPostfix]
		public static void Postfix()
		{
			if (didRun)
				return;

			if (!Utils.BTOS2Exists())
				return;

			didRun = true;
			BToSReaddTPow.DoYourThing();
		}
	}
}