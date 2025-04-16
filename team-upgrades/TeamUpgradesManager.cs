using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

namespace TeamUpgrades;

public class TeamUpgradesManager : MonoBehaviour
{
    private PhotonView _photonView = null!;

    void Start()
    {
        transform.parent = StatsManager.instance.transform;
        gameObject.name = "TeamUpgradesManager";
        gameObject.hideFlags &= ~HideFlags.HideAndDontSave;
        Plugin.Logger.LogInfo("I was alive (however briefly)");

        _photonView = GetComponent<PhotonView>();
    }

    public event EventHandler<UpgradeQuantityChangedEventArgs>? UpgradeQuantityChanged;

    public void AssignUpgradeQuantityImmediately(string playerSteamId, string playerUpgradeStatsKey, int quantity)
    {
        var upgradeQuantities = StatsManager.instance.dictionaryOfDictionaries.GetValueOrDefault(playerUpgradeStatsKey, null);
        if (upgradeQuantities is null) return;
        upgradeQuantities[playerSteamId] = quantity;

        var args = new UpgradeQuantityChangedEventArgs
        {
            PlayerSteamId = playerSteamId,
            StatsKey = playerUpgradeStatsKey,
            Quantity = quantity,
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
            upgradeQuantities[playerSteamId] = quantity;
            var args = new UpgradeQuantityChangedEventArgs
            {
                PlayerSteamId = playerSteamId,
                StatsKey = playerUpgradeStatsKey,
                Quantity = quantity,
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
