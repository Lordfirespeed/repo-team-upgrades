using System;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using UnityEngine;

namespace TeamUpgrades;

public class TeamUpgradesManager : MonoBehaviour
{
    internal static TeamUpgradesManager? Instance { get; private set; }

    private PhotonView _photonView = null!;

    void Start()
    {
        Instance = this;

        transform.parent = StatsManager.instance.transform;
        gameObject.name = "TeamUpgradesManager";
        gameObject.hideFlags &= ~HideFlags.HideAndDontSave;
        _photonView = GetComponent<PhotonView>();

        RegisterVanillaUpgradeQuantityChangedCallbacks();
        RegisterREPOLibUpgradeQuantityChangedCallbacks();
    }

    private void RegisterVanillaUpgradeQuantityChangedCallbacks()
    {
        UpgradeQuantityChanged += (sender, args) =>
        {
            // vanilla checks
            if (args.StatsKey != "playerUpgradeHealth") return;
            if (!args.IsLocalPlayer) return;

            // modded checks
            if (args.NewQuantity == args.OldQuantity) return;
            if (!args.PlayerAvatar) return;

            var quantityDelta = args.NewQuantity - args.OldQuantity;
            args.PlayerAvatar.playerHealth.maxHealth += quantityDelta * 20;
            if (quantityDelta > 0) args.PlayerAvatar.playerHealth.Heal(quantityDelta * 20, false);
        };

        UpgradeQuantityChanged += (sender, args) =>
        {
            // vanilla checks
            if (args.StatsKey != "playerUpgradeStamina") return;
            if (!args.IsLocalPlayer) return;

            // modded checks
            if (args.NewQuantity == args.OldQuantity) return;
            if (!PlayerController.instance) return;

            var quantityDelta = args.NewQuantity - args.OldQuantity;
            PlayerController.instance.EnergyStart += quantityDelta * 10f;
            PlayerController.instance.EnergyCurrent = PlayerController.instance.EnergyStart;
        };

        UpgradeQuantityChanged += (sender, args) =>
        {
            // vanilla checks
            if (args.StatsKey != "playerUpgradeSpeed") return;
            if (!args.IsLocalPlayer) return;

            // modded checks
            if (args.NewQuantity == args.OldQuantity) return;
            if (!PlayerController.instance) return;

            var quantityDelta = args.NewQuantity - args.OldQuantity;
            PlayerController.instance.SprintSpeed += quantityDelta;
            PlayerController.instance.SprintSpeedUpgrades += quantityDelta;
        };

        UpgradeQuantityChanged += (sender, args) =>
        {
            // vanilla checks
            if (args.StatsKey != "playerUpgradeStrength") return;
            if (!args.PlayerAvatar) return;

            // modded checks
            if (args.NewQuantity == args.OldQuantity) return;

            var quantityDelta = args.NewQuantity - args.OldQuantity;
            args.PlayerAvatar.physGrabber.grabStrength += quantityDelta * 0.2f;
        };

        UpgradeQuantityChanged += (sender, args) =>
        {
            // vanilla checks
            if (args.StatsKey != "playerUpgradeRange") return;
            if (!args.PlayerAvatar) return;

            // modded checks
            if (args.NewQuantity == args.OldQuantity) return;

            args.PlayerAvatar.physGrabber.grabRange += args.NewQuantity - args.OldQuantity;
        };

        UpgradeQuantityChanged += (sender, args) =>
        {
            // vanilla checks
            if (args.StatsKey != "playerUpgradeThrow") return;
            if (!args.PlayerAvatar) return;

            // modded checks
            if (args.NewQuantity == args.OldQuantity) return;

            var quantityDelta = args.NewQuantity - args.OldQuantity;
            args.PlayerAvatar.physGrabber.throwStrength += quantityDelta * 0.3f;
        };

        UpgradeQuantityChanged += (sender, args) =>
        {
            // vanilla checks
            if (args.StatsKey != "playerUpgradeLaunch") return;

            // modded checks
            if (args.NewQuantity == args.OldQuantity) return;
            if (!args.PlayerAvatar) return;

            args.PlayerAvatar.tumble.tumbleLaunch += args.NewQuantity - args.OldQuantity;
        };

        UpgradeQuantityChanged += (sender, args) =>
        {
            // vanilla checks
            if (args.StatsKey != "playerUpgradeExtraJump") return;
            if (!args.IsLocalPlayer) return;

            // modded checks
            if (args.NewQuantity == args.OldQuantity) return;
            if (!PlayerController.instance) return;

            PlayerController.instance.JumpExtra += args.NewQuantity - args.OldQuantity;
        };

        UpgradeQuantityChanged += (sender, args) =>
        {
            // vanilla checks
            if (args.StatsKey != "playerUpgradeMapPlayerCount") return;
            if (!args.IsLocalPlayer) return;

            // modded checks
            if (args.NewQuantity == args.OldQuantity) return;
            if (!args.PlayerAvatar) return;

            args.PlayerAvatar.upgradeMapPlayerCount += args.NewQuantity - args.OldQuantity;
        };
    }

    private void RegisterREPOLibUpgradeQuantityChangedCallbacks()
    {
        UpgradeQuantityChanged += (sender, args) => {
            if (!args.StatsKey.StartsWith("playerUpgrade")) return;
            var upgradeIdentifier = args.StatsKey["playerUpgrade".Length..];
            if (!REPOLib.Modules.Upgrades.TryGetUpgrade(upgradeIdentifier, out var upgrade)) return;
            if (upgrade._upgradeAction is null) return;
            if (!args.PlayerAvatar) return;

            upgrade._upgradeAction(args.PlayerAvatar, args.NewQuantity);
        };
    }

    public void ApplyUpgrade(string playerSteamId, string playerUpgradeStatsKey)
    {
        if (!SemiFunc.IsMasterClient()) return;
        var upgradeQuantities = StatsManager.instance.dictionaryOfDictionaries.GetValueOrDefault(playerUpgradeStatsKey, null);
        if (upgradeQuantities is null) return;

        if (Plugin.DoTeamUpgrades)
        {
            var oldQuantity = upgradeQuantities.Values.Max();
            AssignUpgradeQuantityToAllPlayers(playerUpgradeStatsKey, oldQuantity + 1);
        }
        else
        {
            var oldQuantity = upgradeQuantities[playerSteamId];
            AssignUpgradeQuantity(playerSteamId, playerUpgradeStatsKey, oldQuantity + 1);
        }
    }

    public event EventHandler<UpgradeQuantityChangedEventArgs>? UpgradeQuantityChanged;

    public void AssignUpgradeQuantityImmediately(string playerSteamId, string playerUpgradeStatsKey, int quantity)
    {
        var upgradeQuantities = StatsManager.instance.dictionaryOfDictionaries.GetValueOrDefault(playerUpgradeStatsKey, null);
        if (upgradeQuantities is null) return;
        var oldQuantity = upgradeQuantities[playerSteamId];
        upgradeQuantities[playerSteamId] = quantity;

        var args = new UpgradeQuantityChangedEventArgs
        {
            PlayerSteamId = playerSteamId,
            StatsKey = playerUpgradeStatsKey,
            OldQuantity = oldQuantity,
            NewQuantity = quantity,
        };
        UpgradeQuantityChanged?.Invoke(null, args);
    }

    public void AssignUpgradeQuantity(string playerSteamId, string playerUpgradeStatsKey, int quantity)
    {
        if (SemiFunc.IsMasterClientOrSingleplayer())
            AssignUpgradeQuantityImmediately(playerSteamId, playerUpgradeStatsKey, quantity);
        if (SemiFunc.IsMasterClient())
            _photonView.RPC(nameof(AssignUpgradeQuantityRPC), RpcTarget.Others, playerSteamId, playerUpgradeStatsKey, quantity);
    }

    [PunRPC]
    public void AssignUpgradeQuantityRPC(string playerSteamId, string playerUpgradeStatsKey, int quantity, PhotonMessageInfo info)
    {
        if (!info.Sender.IsMasterClient) return;
        AssignUpgradeQuantityImmediately(playerSteamId, playerUpgradeStatsKey, quantity);
    }

    public void AssignUpgradeQuantityToAllPlayersImmediately(string playerUpgradeStatsKey, int quantity)
    {
        var upgradeQuantities = StatsManager.instance.dictionaryOfDictionaries.GetValueOrDefault(playerUpgradeStatsKey, null);
        if (upgradeQuantities is null) return;

        foreach (var playerSteamId in AllPlayerSteamIds)
        {
            var oldQuantity = upgradeQuantities[playerSteamId];
            upgradeQuantities[playerSteamId] = quantity;
            var args = new UpgradeQuantityChangedEventArgs
            {
                PlayerSteamId = playerSteamId,
                StatsKey = playerUpgradeStatsKey,
                OldQuantity = oldQuantity,
                NewQuantity = quantity,
            };
            UpgradeQuantityChanged?.Invoke(null, args);
        }
    }

    public void AssignUpgradeQuantityToAllPlayers(string playerUpgradeStatsKey, int quantity)
    {
        if (SemiFunc.IsMasterClientOrSingleplayer())
            AssignUpgradeQuantityToAllPlayersImmediately(playerUpgradeStatsKey, quantity);
        if (SemiFunc.IsMasterClient())
            _photonView.RPC(nameof(AssignUpgradeQuantityToAllPlayersRPC), RpcTarget.Others, playerUpgradeStatsKey, quantity);
    }

    [PunRPC]
    public void AssignUpgradeQuantityToAllPlayersRPC(string playerUpgradeStatsKey, int quantity, PhotonMessageInfo info)
    {
        if (!info.Sender.IsMasterClient) return;
        AssignUpgradeQuantityToAllPlayersImmediately(playerUpgradeStatsKey, quantity);
    }

    private static IEnumerable<string> AllPlayerSteamIds =>
        StatsManager.instance.dictionaryOfDictionaries["playerColor"].Keys;
}
