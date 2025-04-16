using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using Photon.Pun;
using TeamUpgrades.Extensions;
using UnityEngine;
using UnityEngine.Assertions.Must;
using static TeamUpgrades.UpgradesSynchronizer;

namespace TeamUpgrades;

[BepInDependency(REPOLib.MyPluginInfo.PLUGIN_GUID, REPOLib.MyPluginInfo.PLUGIN_VERSION)]
[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class Plugin : BaseUnityPlugin
{
    internal static Plugin Instance { get; private set; } = null!;
    internal new static ManualLogSource Logger { get; private set; } = null!;

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
        public static class NetworkConnectPatches
        {
            [HarmonyPatch("OnCreatedRoom")]
            [HarmonyPostfix]
            private static void OnCreatedRoom_Postfix()
            {
                var myPrefabId = $"{MyPluginInfo.PLUGIN_GUID}/TeamUpgradesManager";
                var instance = PhotonNetwork.InstantiateRoomObject(myPrefabId, Vector3.zero, Quaternion.identity);
                instance.SetActive(true);
            }
        }
    }
}
