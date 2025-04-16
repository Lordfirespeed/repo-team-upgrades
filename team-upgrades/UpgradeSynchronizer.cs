using System;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;

namespace TeamUpgrades;

internal static class UpgradesSynchronizer
{
	private static Dictionary<string, int> _upgradeQuantities = new();

    static IList<string> GetPlayerUpgradeStatsKeys()
    {
        StatsManager? statsManager = StatsManager.instance;
        if (statsManager is null) throw new InvalidOperationException();
        return statsManager.dictionaryOfDictionaries.Keys
            .Where((key) => key.StartsWith("playerUpgrade"))
            .ToList();
    }

    static Dictionary<string, int>? GetPlayerUpgradeQuantities(string playerUpgradeStatKey)
    {
        StatsManager? statsManager = StatsManager.instance;
        if (statsManager is null) throw new InvalidOperationException();
        return statsManager.dictionaryOfDictionaries.GetValueOrDefault(playerUpgradeStatKey, null);
    }

    static int GetMaxPlayerUpgradeQuantity(string playerUpgradeStatKey)
    {
        var playerUpgradeQuantities = GetPlayerUpgradeQuantities(playerUpgradeStatKey);
        if (playerUpgradeQuantities is null) return 0;
        return playerUpgradeQuantities.Values.Max();
    }

    static void AssignPlayerUpgradeQuantity(string playerUpgradeStatKey, int quantity)
    {
        StatsManager? statsManager = StatsManager.instance;
        if (statsManager is null) throw new InvalidOperationException();

        Plugin.Logger.LogDebug($"Assigning quantity of {playerUpgradeStatKey} to {quantity}");
        statsManager.DictionaryFill(playerUpgradeStatKey, quantity);
    }

	public static void SynchronizeUpgrades()
    {
        if (!LevelGenerator.Instance.Generated) return;
        if (SemiFunc.MenuLevel()) return;
        if (!PhotonNetwork.IsMasterClient) return;

        var playerUpgradeStatKeys = GetPlayerUpgradeStatsKeys();

		Plugin.Logger.LogDebug($"Found upgrade keys: {string.Join(", ", playerUpgradeStatKeys)}");
		foreach (string playerUpgradeStatKey in playerUpgradeStatKeys)
        {
            var maxPlayerUpgradeQuantity = GetMaxPlayerUpgradeQuantity(playerUpgradeStatKey);
            if (maxPlayerUpgradeQuantity <= 0)
            {
                Plugin.Logger.LogDebug($"No upgrades found for key {playerUpgradeStatKey}");
                continue;
            }

            _upgradeQuantities[playerUpgradeStatKey] = maxPlayerUpgradeQuantity;
            Plugin.Logger.LogDebug($"Calculated max value for {playerUpgradeStatKey}: {maxPlayerUpgradeQuantity}");
		}

        foreach (var (playerUpgradeStatKey, quantity) in _upgradeQuantities)
        {
            if (playerUpgradeStatKey is null) continue;
            AssignPlayerUpgradeQuantity(playerUpgradeStatKey, quantity);
        }
		SemiFunc.StatSyncAll();
        Plugin.Logger.LogInfo("Upgrades synced for all players");
	}

	public static void ResetPreviousMaxValues()
    {
        if (!PhotonNetwork.IsMasterClient) return;
        _upgradeQuantities.Clear();
        Plugin.Logger.LogInfo("Previous max upgrades values reset");
    }

    public static void SuppressExceptions<T>(Action action) where T: Exception
    {
        try
        {
            action();
        }
        catch (T e)
        {
            LogException(e);
        }
    }

	public static void LogException(Exception? ex)
    {
        if (ex is null) return;
        if (ex.InnerException is not null)
        {
            LogException(ex.InnerException);
            Plugin.Logger.LogDebug("The above exception was the direct cause of the following exception:");
        }
        Plugin.Logger.LogDebug($"Traceback: {ex.StackTrace}\n{ex.GetType().FullName} - {ex.Message}");
    }
}
