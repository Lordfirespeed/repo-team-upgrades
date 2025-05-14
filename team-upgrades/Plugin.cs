using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using Photon.Pun;
using REPOLib.Modules;
using TeamUpgrades.Extensions;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TeamUpgrades;

[BepInDependency(REPOLib.MyPluginInfo.PLUGIN_GUID, REPOLib.MyPluginInfo.PLUGIN_VERSION)]
[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class Plugin : BaseUnityPlugin
{
    internal static Plugin Instance { get; private set; } = null!;
    internal new static ManualLogSource Logger { get; private set; } = null!;
    internal static readonly bool DoTeamUpgrades = true;

    internal const string MainScenePath = "Assets/Scenes/Main/Main.unity";

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

        GameObject myPrefabContainer = new GameObject("TeamUpgradesPrefabContainer") {
            hideFlags = HideFlags.HideAndDontSave,
        };
        myPrefabContainer.SetActive(false);

        GameObject myPrefab = new GameObject("TeamUpgradesManagerPrefab")
        {
            hideFlags = HideFlags.HideAndDontSave,
        };
        myPrefab.transform.parent = myPrefabContainer.transform;
        myPrefab.AddComponent<PhotonView>();
        myPrefab.AddComponent<TeamUpgradesManager>();

        var myPrefabId = $"{MyPluginInfo.PLUGIN_GUID}/team-upgrades-manager";
        NetworkPrefabs.RegisterNetworkPrefab(myPrefabId, myPrefab);

        SceneManager.sceneLoaded += (scene, loadSceneMode) => {
            if (scene.path != MainScenePath) return;
            if (!SemiFunc.IsMultiplayer()) return;
            if (!SemiFunc.IsMasterClient()) return;
            if (TeamUpgradesManager.Instance != null) return;

            PhotonNetwork.InstantiateRoomObject(myPrefabId, Vector3.zero, Quaternion.identity);
        };

        var harmony = new Harmony(Info.Metadata.GUID);
        harmony.PatchAllNestedTypes(typeof(Patches));
    }

    [SuppressMessage("ReSharper", "InconsistentNaming")]
    class Patches
    {
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

        [HarmonyPatch(typeof(REPOLibItemUpgrade))]
        static class REPOLibItemUpgradePatches
        {
            [HarmonyPatch(nameof(REPOLibItemUpgrade.Upgrade))]
            [HarmonyPrefix]
            static bool Upgrade_Prefix(ref REPOLibItemUpgrade __instance)
            {
                TeamUpgradesManager.Instance?.ApplyUpgrade(
                    __instance._itemToggle.GetTogglingPlayerSteamId(),
                    $"playerUpgrade{__instance.UpgradeId}"
                );
                return false;
            }
        }
        #endregion

        [HarmonyPatch(typeof(PhysGrabber))]
        static class PhysGrabberPatches
        {
            [HarmonyPatch(nameof(PhysGrabber.LateStart), MethodType.Enumerator)]
            [HarmonyTranspiler]
            static IEnumerable<CodeInstruction> LateStart_MoveNext_Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                var matcher = new CodeMatcher(instructions);

                matcher.MatchForward(false, [
                    new CodeMatch(OpCodes.Ldsfld, typeof(StatsManager).GetField(nameof(StatsManager.instance))),
                    new CodeMatch(OpCodes.Ldfld, typeof(StatsManager).GetField(nameof(StatsManager.playerUpgradeStrength))),
                    new CodeMatch(OpCodes.Ldarg_0),
                    new CodeMatch((instr) => instr.opcode == OpCodes.Ldfld),
                    new CodeMatch(OpCodes.Callvirt, typeof(Dictionary<string, int>).GetMethod(nameof(Dictionary<string, int>.ContainsKey))),
                    new CodeMatch((instr) => instr.opcode == OpCodes.Brfalse),
                ]);
                if (matcher.IsInvalid) throw new InvalidOperationException();
                if (matcher.InstructionAt(3).operand is not FieldInfo steamIdLocalField) throw new InvalidOperationException();
                if (matcher.InstructionAt(5).operand is not Label waitLabel) throw new InvalidOperationException();

                matcher.InsertAndAdvance([
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Ldloc_1),
                    new CodeInstruction(OpCodes.Ldfld, typeof(PhysGrabber).GetField(nameof(PhysGrabber.playerAvatar))),
                    new CodeInstruction(OpCodes.Call, typeof(SemiFunc).GetMethod(nameof(SemiFunc.PlayerGetSteamID))),
                    new CodeInstruction(OpCodes.Stfld, steamIdLocalField),
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Ldfld, steamIdLocalField),
                    new CodeInstruction(OpCodes.Brfalse, waitLabel),
                ]);
                matcher.Instruction.MoveLabelsTo(matcher.InstructionAt(-8));

                Logger.LogDebugInstructionsFrom(matcher);

                return matcher.InstructionEnumeration();
            }
        }
    }
}
