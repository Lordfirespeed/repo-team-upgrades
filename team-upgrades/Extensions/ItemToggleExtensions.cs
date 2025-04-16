namespace TeamUpgrades.Extensions;

public static class ItemToggleExtensions
{
    public static PlayerAvatar GetTogglingPlayerAvatar(this ItemToggle toggle) =>
        SemiFunc.PlayerAvatarGetFromPhotonID(toggle.playerTogglePhotonID);

    public static string GetTogglingPlayerSteamId(this ItemToggle toggle) =>
        SemiFunc.PlayerGetSteamID(GetTogglingPlayerAvatar(toggle));
}
