using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using Photon.Pun;
using TeamUpgrades.Extensions;
using UnityEngine;

namespace TeamUpgrades;

[BepInDependency(REPOLib.MyPluginInfo.PLUGIN_GUID, REPOLib.MyPluginInfo.PLUGIN_VERSION)]
[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class Plugin : BaseUnityPlugin
{
    internal static Plugin Instance { get; private set; } = null!;
    internal new static ManualLogSource Logger { get; private set; } = null!;
    internal static readonly bool DoTeamUpgrades = true;

    public Plugin()
    {
        if (Instance is not null) {
            throw new InvalidOperationException($"{typeof(Plugin).AssemblyQualifiedName} was instantiated twice");
        }
        Instance = this;
    }

    private void Awake()
    {
        Logger = base.Logger;

        GameObject myPrefab = new GameObject("TeamUpgradesManagerPrefab")
        {
            hideFlags = HideFlags.HideAndDontSave,
        };
        myPrefab.SetActive(false);
        myPrefab.AddComponent<PhotonView>();
        myPrefab.AddComponent<TeamUpgradesManager>();

        var myPrefabId = $"{MyPluginInfo.PLUGIN_GUID}/TeamUpgradesManager";
        REPOLib.Modules.NetworkPrefabs.RegisterNetworkPrefab(myPrefabId, myPrefab);

        var harmony = new Harmony(Info.Metadata.GUID);
        harmony.PatchAllNestedTypes(typeof(Patches));
    }

    [SuppressMessage("ReSharper", "InconsistentNaming")]
    class Patches
    {
        [HarmonyPatch(typeof(NetworkConnect))]
        static class NetworkConnectPatches
        {
            [HarmonyPatch("OnCreatedRoom")]
            [HarmonyPostfix]
            static void OnCreatedRoom_Postfix()
            {
                var myPrefabId = $"{MyPluginInfo.PLUGIN_GUID}/TeamUpgradesManager";
                var instance = PhotonNetwork.InstantiateRoomObject(myPrefabId, Vector3.zero, Quaternion.identity);
                instance.SetActive(true);
            }
        }

        [HarmonyPatch(typeof(StatsManager))]
        static class StatsManagerPatches
        {
            [HarmonyPatch("PlayerAdd")]
            [HarmonyPostfix]
            static void OnPlayerAdd_Postfix(StatsManager __instance, object[] __args)
            {
                if (!DoTeamUpgrades) return;
                if (__args[0] is not string playerSteamId) return;

                foreach (Dictionary<string, int> dictionary in __instance.AllDictionariesWithPrefix("playerUpgrade"))
                {
                    dictionary[playerSteamId] = dictionary.Values.Max();
                }
            }
        }

        #region Item upgrade patches
        [HarmonyPatch(typeof(ItemUpgradePlayerHealth))]
        static class ItemUpgradePlayerHealthPatches
        {
            [HarmonyPatch("Upgrade")]
            [HarmonyPrefix]
            static bool Upgrade_Prefix(ref ItemUpgradePlayerHealth __instance)
            {
                TeamUpgradesManager.Instance?.ApplyUpgrade(
                    __instance.itemToggle.GetTogglingPlayerSteamId(),
                    "playerUpgradeHealth"
                );
                return false;
            }
        }

        [HarmonyPatch(typeof(ItemUpgradePlayerEnergy))]
        static class ItemUpgradePlayerEnergyPatches
        {
            [HarmonyPatch("Upgrade")]
            [HarmonyPrefix]
            static bool Upgrade_Prefix(ref ItemUpgradePlayerEnergy __instance)
            {
                TeamUpgradesManager.Instance?.ApplyUpgrade(
                    __instance.itemToggle.GetTogglingPlayerSteamId(),
                    "playerUpgradeStamina"
                );
                return false;
            }
        }

        [HarmonyPatch(typeof(ItemUpgradePlayerSprintSpeed))]
        static class ItemUpgradePlayerSprintSpeedPatches
        {
            [HarmonyPatch("Upgrade")]
            [HarmonyPrefix]
            static bool Upgrade_Prefix(ref ItemUpgradePlayerSprintSpeed __instance)
            {
                TeamUpgradesManager.Instance?.ApplyUpgrade(
                    __instance.itemToggle.GetTogglingPlayerSteamId(),
                    "playerUpgradeSpeed"
                );
                return false;
            }
        }

        [HarmonyPatch(typeof(ItemUpgradePlayerGrabStrength))]
        static class ItemUpgradePlayerGrabStrengthPatches
        {
            [HarmonyPatch("Upgrade")]
            [HarmonyPrefix]
            static bool Upgrade_Prefix(ref ItemUpgradePlayerGrabStrength __instance)
            {
                TeamUpgradesManager.Instance?.ApplyUpgrade(
                    __instance.itemToggle.GetTogglingPlayerSteamId(),
                    "playerUpgradeStrength"
                );
                return false;
            }
        }

        [HarmonyPatch(typeof(ItemUpgradePlayerGrabRange))]
        static class ItemUpgradePlayerGrabRangePatches
        {
            [HarmonyPatch("Upgrade")]
            [HarmonyPrefix]
            static bool Upgrade_Prefix(ref ItemUpgradePlayerGrabRange __instance)
            {
                TeamUpgradesManager.Instance?.ApplyUpgrade(
                    __instance.itemToggle.GetTogglingPlayerSteamId(),
                    "playerUpgradeRange"
                );
                return false;
            }
        }

        [HarmonyPatch(typeof(ItemUpgradePlayerGrabThrow))]
        static class ItemUpgradePlayerGrabThrowPatches
        {
            [HarmonyPatch("Upgrade")]
            [HarmonyPrefix]
            static bool Upgrade_Prefix(ref ItemUpgradePlayerGrabThrow __instance)
            {
                TeamUpgradesManager.Instance?.ApplyUpgrade(
                    __instance.itemToggle.GetTogglingPlayerSteamId(),
                    "playerUpgradeThrow"
                );
                return false;
            }
        }

        [HarmonyPatch(typeof(ItemUpgradePlayerTumbleLaunch))]
        static class ItemUpgradePlayerTumbleLaunchPatches
        {
            [HarmonyPatch("Upgrade")]
            [HarmonyPrefix]
            static bool Upgrade_Prefix(ref ItemUpgradePlayerTumbleLaunch __instance)
            {
                TeamUpgradesManager.Instance?.ApplyUpgrade(
                    __instance.itemToggle.GetTogglingPlayerSteamId(),
                    "playerUpgradeLaunch"
                );
                return false;
            }
        }

        [HarmonyPatch(typeof(ItemUpgradePlayerExtraJump))]
        static class ItemUpgradePlayerExtraJumpPatches
        {
            [HarmonyPatch("Upgrade")]
            [HarmonyPrefix]
            static bool Upgrade_Prefix(ref ItemUpgradePlayerExtraJump __instance)
            {
                TeamUpgradesManager.Instance?.ApplyUpgrade(
                    __instance.itemToggle.GetTogglingPlayerSteamId(),
                    "playerUpgradeExtraJump"
                );
                return false;
            }
        }

        [HarmonyPatch(typeof(ItemUpgradeMapPlayerCount))]
        static class ItemUpgradeMapPlayerCountPatches
        {
            [HarmonyPatch("Upgrade")]
            [HarmonyPrefix]
            static bool Upgrade_Prefix(ref ItemUpgradeMapPlayerCount __instance)
            {
                TeamUpgradesManager.Instance?.ApplyUpgrade(
                    __instance.itemToggle.GetTogglingPlayerSteamId(),
                    "playerUpgradeMapPlayerCount"
                );
                return false;
            }
        }
        #endregion
    }
}
