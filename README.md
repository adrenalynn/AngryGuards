# Angry Guards

This mod adds new bow, crossbow and matchlock gun guards that shoot players instead of zombies. Unlock them via research. The crafter(workbench) gets the recipes for the guard spots, place those down to create guard spots.
Those guards will hurt! Not their owner, of course, only enemy players.

## Commands
**/friendly { add | list | remove } [playername]**
To add, list or remove friendly players that your guards will not shoot at. This command is usable by all players

## Settings
To adjust the damage settings copy the file **angryguards-config.json** into your world savegame folder. Damage, range and reload speed can be adjusted.

Use permission **mods.angryguards.peacekeeper** for any user/group that should never be shot at.

**shootMountedPlayers** defaults to false, players mounted on a glider will not be shot at.

## Passive mode
Regular guard mode is active, shooting all players that come in range.
You can change to **guardMode=passive** in the config file, in passive mode the guards will not shoot players on sight. Only if the target attacks NPCs or tries to change(place/delete blocks) they will start shooting.
**passiveProtectionRange** defines in what range block changes are tracked (distance from banner)

## License
The mod itself is licensed under the <a href="LICENSE">MIT License</a>.

It uses icons from <a href="https://www.flaticon.com/" title="Flaticon">www.flaticon.com</a> licensed under Creative Commons <a href="http://creativecommons.org/licenses/by/3.0/" title="Creative Commons BY 3.0" target="_blank">CC 3.0 BY</a><br>
Sun by <a href="https://www.flaticon.com/authors/dinosoftlabs" title="DinosoftLabs">DinosoftLabs</a><br>
Moon by <a href="https://www.flaticon.com/authors/swifticons" title="Swifticons">Swifticons</a><br>
Bow by <a href="http://www.freepik.com" title="Freepik">Freepik</a><br>
Crossbow by <a href="https://www.flaticon.com/authors/smashicons" title="Smashicons">Smashicons</a><br>
Swords by <a href="https://www.flaticon.com/authors/eucalyp" title="Eucalyp">Eucalyp</a><br>

