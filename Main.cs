using ICities;
using CitiesHarmony.API;
using UnityEngine.SceneManagement;

namespace SaneSnappingSettings
{
    public class Mod : IUserMod
    {
        public string Name => "Sane Snapping Settings";
        public string Description => "Separates snapping settings for roads, paths and fences";

        public void OnEnabled()
        {
            HarmonyHelper.DoOnHarmonyReady(() => Patcher.PatchAll());
        }

        public void OnDisabled()
        {
            if (HarmonyHelper.IsHarmonyInstalled) Patcher.UnpatchAll();
        }

        public static string Identifier = "WQ.SSS/";
    }
}
