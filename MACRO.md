L2Apf
=====

Lineage 2 (C4) Artificial Player / Framework

##Macro

###Command-line arguments

- /login:&lt;path/to/file.xml&gt; - defines XML file contains authorization data. Example [here](https://github.com/JulianMorris/L2Apf/blob/master/Macro/Script/test.ls). **Required**.
- /global:{true|false} - specify bot to execute commands from common chat tabs or private messages only.
- /auth:{true|false} - specify bot to execute commands from any player or owners list only.
- /owners:&lt;player1&gt;[,&lt;playerN&gt;] - specify owners list.

###Game-chat commands

- /moveto => order bot to move to his target
- /moveto me => order bot to move to leader position
- /moveto my => order bot to move to leader destination
- /travel &lt;x y z&gt; => order bot to move to xyz coordinates
- /target my => order bot to assign leader target
- /target me => order bot to get leader as target
- /target npc => order bot to get nearest npc as target
- /target "&lt;name&gt;" => order bot to get player with name &lt;name&gt; as target
- /cancel => order bot to discard his target
- /attack => order bot to attack his target
- /pickup => order bot to pickup nearest item
- /target self => order bot to get self as target
- /useskill &lt;skillid&gt; => order bot to use skill with id &lt;skillid&gt;
- /useitem &lt;itemid&gt; => order bot to use item with id &lt;itemid&gt;
- /itemlist => print all item id & count pairs of all items in inventory
- /dropitem <id> [count] => order bot to drop some item amount from inventory
- /return {place} => order bot to return to nearest town (or other) after death
- /info {loc|level|sp|hp|mp} => request specified value from bot
- /action {sit|stand} => order bot to use sit or stand action
- /follow {fast|full} => order bot to follow to his target
? /restart <name>
