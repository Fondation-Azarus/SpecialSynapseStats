# SpecialSynapseStats
A Synapse 2 plug-in that adds a XP and a stats system.

**TO RESPECT THE 8.11 [VSR](https://scpslgame.com/Verified_server_rules.pdf) RULE THIS PLUG-IN WON'T STORE ANY DATA FROM PLAYERS WHO ENABLED "Do Not Track" (DNT) UNTIL IT RECEIVES CONSENT FROM THE PLAYER VIA THE `.ConsentDNT` COMMAND.**

***

## Description
This plug-in has a lot of configs, make sure to take a look.

With this plug-in you can :
- [x] Know the first time a player loged in.
- [x] Know how much did a player play on your server (in minutes).
- [x] Configure how much XP should a player be awarded for certain actions.
- [x] Configure how much XP is needed to get a level.
- [x] Configure something similar to a "prestige" system with "bigLevels".
- [x] Know how much kills/damages did a player.
- [x] Know how many times did a player escape.
- [x] Know how many times died a player and how much damages he received.
- [x] Know how many games played a player.
- [x] Know how much time was a player banned in total (in minutes) and how many times he was kicked and banned.
- [x] Know how many times did a player use a specific item.
- [x] Know how many times did the player spawn as a specific role.
- [x] Configure whether SCP should be able to escape or not.
- [ ] Choose the round decimal.
- [ ] Cook a baguette.
- [x] Add translations.

And more...

***

## SSS Profile
The SSS Profile is a list of datas that each connected player has, it contains, but is not limited to, all the datas listed above.
It can be accessed by the `.SSSProfile` command by other player as well as the player itself.
The player can see all his datas but other players (except moderators with the `vanilla.PlayerSensitiveDataAccess` permission) can't see his private datas (which you can configure).

***

## Commands
This plug-in brings 3 new commands :
| Command | Function |
| :-------------: | :------ |
| .ConsentDNT | Allows the server to store datas about the player even if the player enabled "DoNotTrack" |
| .RevokeConsentDNT | Revokes the player's consent to store data and deletes all his already existing datas. |
| .SSSProfile {Someone's nickname or ID} {'staff' if you want to see someone else's private stats} | Shows a SSS profile. Only players with the "vanilla.PlayerSensitiveDataAccess" permission can use the "staff" argument. |

***

## Configs
| Config name | Type | Description |
| :-------------: | :----: | :------ |
| disabled | bool | Is the plug-in disabled ? If it is other plug-ins will still be able to add datas/XP.

Default configs :
```[SpecialSynapseStats]
{
# Before modifying configs, you should read the github, if you have any questions you can always contact me on Discord ! Plug-in disabled ? :
disabled: false
# What's your UTC ? :
utc: 1
# XP multiplicator :
expMultiplier: 1
# XP needed for first level :
firstLevelExpNeeded: 200
# Should this plug-in use 'qXpLevel' (if false it will use 'rXpLevel')
xpRorQ: false
# Adds to the current level's XP needed to get the next one's :
rExpLevel: 30
# Multiplies the current level's XP needed to get the next one's :
qExpLevel: 1.04999995
# How many levels are necessary to get one big level :
nmbrLevelNeededBigLevel: 35
# The amount of XP a specific role or team should get for a specific action :
listExpRewardsRoleID:
- roleID: 1
  team: SCP
  survivalExp: 5
  escapeExp: 100
  escapeAssistExp: 0
  captureExp: 0
  roleIDKillExp:
    6: 20
    15: 35
  teamKillExp:
    MTF: 40
    SCP: 80
  scpKillAssistExp: 0
  scpEscapeExp: 0
  warheadExp: 0
- roleID: 6
  team: SCP
  survivalExp: 6
  escapeExp: 100
  escapeAssistExp: 0
  captureExp: 0
  roleIDKillExp:
    1: 10
  teamKillExp:
    CHI: 40
    SCP: 100
  scpKillAssistExp: 50
  scpEscapeExp: 0
  warheadExp: 0
- roleID: 15
  team: SCP
  survivalExp: 7
  escapeExp: 0
  escapeAssistExp: 50
  captureExp: 50
  roleIDKillExp:
    1: 8
  teamKillExp:
    CHI: 35
    SCP: 100
  scpKillAssistExp: 50
  scpEscapeExp: 0
  warheadExp: 0
- roleID: 0
  team: MTF
  survivalExp: 7
  escapeExp: 0
  escapeAssistExp: 60
  captureExp: 35
  roleIDKillExp:
    1: 10
  teamKillExp:
    CHI: 30
    SCP: 100
  scpKillAssistExp: 50
  scpEscapeExp: 0
  warheadExp: 0
- roleID: 0
  team: CHI
  survivalExp: 7
  escapeExp: 0
  escapeAssistExp: 20
  captureExp: 42
  roleIDKillExp:
    6: 5
    15: 20
  teamKillExp:
    MTF: 30
    SCP: 10
  scpKillAssistExp: 0
  scpEscapeExp: 0
  warheadExp: 0
- roleID: 0
  team: SCP
  survivalExp: 2.5
  escapeExp: 30
  escapeAssistExp: 0
  captureExp: 0
  roleIDKillExp:
    1: 5
    6: 5
    15: 5
  teamKillExp:
    CHI: 3
    MTF: 5
  scpKillAssistExp: 0
  scpEscapeExp: 0
  warheadExp: 0
- roleID: 9
  team: SCP
  survivalExp: 2.5
  escapeExp: 30
  escapeAssistExp: 0
  captureExp: 0
  roleIDKillExp:
    1: 5
    6: 5
    15: 5
  teamKillExp:
    CHI: 3
    MTF: 5
  scpKillAssistExp: 0
  scpEscapeExp: 0
  warheadExp: 0
- roleID: 5
  team: SCP
  survivalExp: 5
  escapeExp: 50
  escapeAssistExp: 0
  captureExp: 0
  roleIDKillExp:
    1: 4
    6: 4
    15: 4
  teamKillExp:
    CHI: 3
    MTF: 4
  scpKillAssistExp: 0
  scpEscapeExp: 0
  warheadExp: 0
- roleID: 3
  team: SCP
  survivalExp: 2
  escapeExp: 10
  escapeAssistExp: 0
  captureExp: 0
  roleIDKillExp:
  teamKillExp:
  scpKillAssistExp: 0
  scpEscapeExp: 0
  warheadExp: 0
- roleID: 7
  team: SCP
  survivalExp: 2
  escapeExp: 0
  escapeAssistExp: 0
  captureExp: 0
  roleIDKillExp:
  teamKillExp:
  scpKillAssistExp: 0
  scpEscapeExp: 0
  warheadExp: 0
- roleID: 17
  team: SCP
  survivalExp: 2.5
  escapeExp: 50
  escapeAssistExp: 0
  captureExp: 0
  roleIDKillExp:
    1: 5
    6: 5
    15: 5
  teamKillExp:
    CHI: 4
    MTF: 5
  scpKillAssistExp: 0
  scpEscapeExp: 0
  warheadExp: 0
- roleID: 16
  team: SCP
  survivalExp: 2.5
  escapeExp: 50
  escapeAssistExp: 0
  captureExp: 0
  roleIDKillExp:
    1: 5
    6: 5
    15: 5
  teamKillExp:
    CHI: 4
    MTF: 5
  scpKillAssistExp: 0
  scpEscapeExp: 0
  warheadExp: 0
# XP given to connected players in general :
expGeneralPerMinute: 7.5
# Should Facility Guards be considered as a part of the MTF team ? :
guardMTF: false
# Can a SCP escape ? :
scpEscape: true
# Must the escaped person's allies be at the surface to receive assist escape XP ? :
assistEscapeSurfaceZone: false
# XP given to SCP-106 when he grabs someone :
scp106GrabExp: 6
# Should killing SCP other than instances count as a kill assist on SCP for SCP Foundation staff ? :
warheadScpKillAssist: true
# A list of stats that should be private :
privateStats:
- ConsentDNT
- Teamkill(s)
- Kick(s)
- Ban(s)
- Total ban duration
}```

***

## You want to use this system ?
This plug-in contains a lot of static method you can easily use in your own plug-ins (like `AddDataFloat` or `AddExp`). 
Every static method has a summary explaining what it does.
All you need to do is to add a reference to the `.dll` file in your plug-in (on Visual Studio you can simply right-click on "References" > "Add").
(If your plug-in uses SpecialSynapseStats then you should always have both `.dll` files in your plugin directory.) 

***

## Installation
1. [Install Synapse](https://docs.synapsesl.xyz/setup/setup).
2. Place the `.dll` file that you can download [here]() in your plug-in directory.
3. Restart/Start your server.
