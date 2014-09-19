L2Apf
=====

Lineage 2 (C4) Artificial Player / Framework (alpha)

##Packages

###Core
Server protocol abstraction layer library. Implements program API to interation with game server. See [Macro.cs](https://github.com/JulianMorris/L2Apf/blob/master/Macro/Macro.cs) for usage example.

###Macro
Runnable package. Implements *game chat*-managed bot. Bot uses chat like a command-line tool. See [more](https://github.com/JulianMorris/L2Apf/blob/master/MACRO.md).

###Script
Class library. Implements simple script-managed bot. See [examples](https://github.com/JulianMorris/L2Apf/blob/master/examples) folder for usage example.

##Requirements

Application requires a l2jc4 server for launch e.g.
- http://svn.l2jserver.com/branches/L2_GameServer_c4/
- http://svn.l2jdp.com/branches/C4_Datapack/datapack_development/

System folder with 660 protocol version for game client can be found here:
- http://rghost.ru/51542194