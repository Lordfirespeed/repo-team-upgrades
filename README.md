# Team Upgrades (Done Right) [<img align="right" width="128" height="128" src="https://github.com/Lordfirespeed/repo-team-upgrades/raw/main/team-upgrades/assets/icons/icon.png">](https://thunderstore.io/c/repo/p/Lordfirespeed/Team_Upgrades/)

[![GitHub Actions Workflow Status](https://img.shields.io/github/actions/workflow/status/Lordfirespeed/repo-team-upgrades/build.yml?style=for-the-badge&logo=github)](https://github.com/Lordfirespeed/repo-team-upgrades/actions/workflows/build.yml)
[![Release Version](https://img.shields.io/github/v/release/Lordfirespeed/repo-team-upgrades?style=for-the-badge&logo=github)](https://github.com/Lordfirespeed/repo-team-upgrades/releases)
[![Thunderstore Version](https://img.shields.io/thunderstore/v/Lordfirespeed/Team_Upgrades?style=for-the-badge&logo=thunderstore&logoColor=white)](https://thunderstore.io/c/repo/p/Lordfirespeed/Team_Upgrades/)
[![Thunderstore Downloads](https://img.shields.io/thunderstore/dt/Lordfirespeed/Team_Upgrades?style=for-the-badge&logo=thunderstore&logoColor=white)](https://thunderstore.io/c/repo/p/Lordfirespeed/Team_Upgrades/)

A `R.E.P.O` mod for applying upgrades to all players.

## Another 'team upgrades' mod? Why?

The others don't work.

- [EvilCheetah's Team Upgrades](https://thunderstore.io/c/repo/p/EvilCheetah/TeamUpgrades/):
  - Doesn't apply upgrades to players that 'late join' a saved game.
- [Traktool's Shared Upgrades](https://thunderstore.io/c/repo/p/Traktool/SharedUpgrades/):
  - Doesn't apply upgrades to players that 'late join' a saved game until a level is completed.
  - Health upgrades don't heal all players, only the player that applied the upgrade.
- [NastyPablo's LateJoinSharedUpgrades](https://thunderstore.io/c/repo/p/NastyPablo/LateJoinSharedUpgradesByNastyPablo/):
  - Newly purchased upgrades are duplicated for 1 level, because player upgrade quantities are set to purchase quantities
    *before* upgrade items have been applied.
  - Health upgrades don't heal all players, only the player that applied the upgrade.
- [SharkLucas' SyncHostUpgrades](https://thunderstore.io/c/repo/p/SharkLucas/SyncHostUpgrades/):
  - Checks for necessary updates **every frame**, which is hugely wasteful.
  - Doesn't implement own RPCs.
- [hairo's SharedUpgrades](https://thunderstore.io/c/repo/p/hairo/SharedUpgrades/):
  - Doesn't assign upgrades to players that late-join a saved game.
  - Doesn't implement own RPCs.
- [FluxTeam's SharingIsCaring](https://thunderstore.io/c/repo/p/FluxTeam/SharingIsCaring/):
  - Checks for necessary updates every 5 seconds (by default, configurable interval), which is wasteful.
- [DuckyVR's UpgradesForAll](https://thunderstore.io/c/repo/p/DuckyVR/UpgradesForAll/):
  - Doesn't assign upgrades to players that late-join a saved game
  - Doesn't implement own RPCs.
- [TGO's SyncUpgrades](https://thunderstore.io/c/repo/p/TGO/SyncUpgrades/):
  - Doesn't implement own RPCs.
  - To apply 1 upgrade, all upgrade quantities are re-sent to all players over the network, which is wasteful.

## How's it work?

This mod doesn't try to be host-only.
**All players must have the mod installed, or it won't work right.**
All players must also have the **same version** of the mod installed, or it probably won't work right.

This requirement may frustrate you, but it's not possible to ensure team upgrades work seamlessly otherwise.
If you really need a host-only mod, have a look at the mods linked above.

## Acknowledgements

Icon from [Noto Emoji](https://github.com/googlefonts/noto-emoji/blob/main/png/512/emoji_u1f465.png).
