using System;

namespace TeamUpgrades;

public class UpgradeQuantityChangedEventArgs : EventArgs
{
    public required string PlayerSteamId { get; init; }
    public required string StatsKey { get; init; }
    public required int OldQuantity { get; init; }
    public required int NewQuantity { get; init; }

    public PlayerAvatar PlayerAvatar => SemiFunc.PlayerAvatarGetFromSteamID(PlayerSteamId);

    public bool IsLocalPlayer => PlayerAvatar == SemiFunc.PlayerAvatarLocal();
}
