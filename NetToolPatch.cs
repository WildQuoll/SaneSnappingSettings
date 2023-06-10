using ColossalFramework.UI;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace SaneSnappingSettings
{
    [HarmonyPatch(typeof(NetTool))]
    public static class NetToolPatch
    {
        private enum NetworkType
        {
            Road,
            Path,
            Fence,
            Other
        };

        private static Dictionary<NetworkType, NetTool.Snapping> SnappingSettings = new Dictionary<NetworkType, NetTool.Snapping> {
            {NetworkType.Road, NetTool.Snapping.All },
            {NetworkType.Path, NetTool.Snapping.Angle | NetTool.Snapping.Grid },
            {NetworkType.Fence, NetTool.Snapping.Angle },
            {NetworkType.Other, NetTool.Snapping.All }
        };

        private static MethodInfo ensureCheckBoxesMethod = AccessTools.Method(typeof(SnapSettingsPanel), "EnsureCheckBoxes");

        private static NetworkType? GetNetworkType(NetInfo info)
        {
            if (info == null || info.m_netAI == null)
            {
                return null;
            }

            switch (info.m_netAI)
            {
                case RoadBaseAI r:
                case MetroTrackBaseAI m:
                case TrainTrackBaseAI t:
                    return NetworkType.Road;
                case PedestrianBridgeAI pb:
                case PedestrianPathAI pp:
                case PedestrianTunnelAI pt:
                case PedestrianWayAI pw:
                    return NetworkType.Path;
                case DecorationWallAI w:
                    return NetworkType.Fence;
                default:
                    return NetworkType.Other;
            }
        }

        [HarmonyPrefix]
        [HarmonyPatch(nameof(NetTool.Prefab), MethodType.Setter)]
        static void Prefix(NetTool __instance, NetInfo __0)
        {
            var newNetworkType = GetNetworkType(__0);
            var previousNetworkType = GetNetworkType(__instance.m_prefab);

            if (newNetworkType != previousNetworkType)
            {
                if (previousNetworkType.HasValue)
                {
                    SnappingSettings[previousNetworkType.Value] = __instance.m_snap;
                }

                if (newNetworkType.HasValue)
                {
                    __instance.m_snap = SnappingSettings[newNetworkType.Value];
                }

                var panel = UIView.library.Get<SnapSettingsPanel>("SnapSettingsPanel");
                ensureCheckBoxesMethod.Invoke(panel, null);
            }
        }
    }
}
