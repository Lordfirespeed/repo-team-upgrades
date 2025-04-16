using System;

namespace TeamUpgrades;

public class UpgradeQuantityChangedEventArgs : EventArgs
{
    public required string PlayerSteamId { get; set; }
    public required string StatsKey { get; set; }
    public required int Quantity { get; set; }
}
