using System;

namespace TeamUpgrades;

public class UpgradeQuantityChangedEventArgs : EventArgs
{
    public required string PlayerSteamId { get; init; }
    public required string StatsKey { get; init; }
    public required int Quantity { get; init; }
}
