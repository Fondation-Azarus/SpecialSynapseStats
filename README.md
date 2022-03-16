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

## You want to use this system ?
This plug-in contains a lot of static method you can easily use in your own plug-ins.
Every static method has a summary explaining what it does.
All you need to do is to add a reference to the `.dll` file in your plug-in.
(If your plug-in uses SpecialSynapseStats then you should always have both in your plugin directory.) 

***

## Installation
1. [Install Synapse](https://docs.synapsesl.xyz/setup/setup).
2. Place the `.dll` file that you can download [here]() in your plug-in directory.
3. Restart/Start your server.
