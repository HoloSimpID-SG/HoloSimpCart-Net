# HoloSimpID@SG Cart Bot

Using Discord.NET @ https://docs.discordnet.dev/
<br/>Running on Docker, using PostgreSQL

Discord Bot for tracking Shared Carts, because we are just that big of V-Tuber Simp and buys merch often... excluding me.

Made this because SirRu was taking too long with his Python version.
Produced by Jagerking779.

Versioning Guide:
<br/>`_._._.x` - Inconsequential Changes, Refactoring, Adding Comments
<br/>`_._.x._` - Minor Changes, Bugfixes
<br/>`_.x._._` - Major Change
<br/>`x._._._` - Remove your current version

## Where to Start

Application starts at `./src/Main.cs`.

Database are Initialized by scripts inside `./db-init` and communications are in `./src/DbHandler.cs`.

All Commands are located inside `./src/command-list`, `Commands.cs` responsible for defining the commands with Discord API, meanwhile `Responses.cs` are responsible for what logic is ran for that command. More details on how to use them are inside, I tried simplifying things as much as possible.

`./src/library/` are just bunch of utility functions. Can take a look if you want
