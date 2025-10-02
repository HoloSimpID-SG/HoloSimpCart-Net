# HoloSimpID@SG Cart Bot

[![GitButler](https://img.shields.io/badge/GitButler-%23B9F4F2?logo=data%3Aimage%2Fsvg%2Bxml%3Bbase64%2CPHN2ZyB3aWR0aD0iMzkiIGhlaWdodD0iMjgiIHZpZXdCb3g9IjAgMCAzOSAyOCIgZmlsbD0ibm9uZSIgeG1sbnM9Imh0dHA6Ly93d3cudzMub3JnLzIwMDAvc3ZnIj4KPHBhdGggZD0iTTI1LjIxNDUgMTIuMTk5N0wyLjg3MTA3IDEuMzg5MTJDMS41NDI5NSAwLjc0NjUzMiAwIDEuNzE0MDYgMCAzLjE4OTQ3VjI0LjgxMDVDMCAyNi4yODU5IDEuNTQyOTUgMjcuMjUzNSAyLjg3MTA3IDI2LjYxMDlMMjUuMjE0NSAxNS44MDAzQzI2LjcxOTcgMTUuMDcyMSAyNi43MTk3IDEyLjkyNzkgMjUuMjE0NSAxMi4xOTk3WiIgZmlsbD0iYmxhY2siLz4KPHBhdGggZD0iTTEzLjc4NTUgMTIuMTk5N0wzNi4xMjg5IDEuMzg5MTJDMzcuNDU3MSAwLjc0NjUzMiAzOSAxLjcxNDA2IDM5IDMuMTg5NDdWMjQuODEwNUMzOSAyNi4yODU5IDM3LjQ1NzEgMjcuMjUzNSAzNi4xMjg5IDI2LjYxMDlMMTMuNzg1NSAxNS44MDAzQzEyLjI4MDMgMTUuMDcyMSAxMi4yODAzIDEyLjkyNzkgMTMuNzg1NSAxMi4xOTk3WiIgZmlsbD0idXJsKCNwYWludDBfcmFkaWFsXzMxMF8xMjkpIi8%2BCjxkZWZzPgo8cmFkaWFsR3JhZGllbnQgaWQ9InBhaW50MF9yYWRpYWxfMzEwXzEyOSIgY3g9IjAiIGN5PSIwIiByPSIxIiBncmFkaWVudFVuaXRzPSJ1c2VyU3BhY2VPblVzZSIgZ3JhZGllbnRUcmFuc2Zvcm09InRyYW5zbGF0ZSgxNi41NzAxIDE0KSBzY2FsZSgxOS44NjQxIDE5LjgzODMpIj4KPHN0b3Agb2Zmc2V0PSIwLjMwMTA1NiIgc3RvcC1vcGFjaXR5PSIwIi8%2BCjxzdG9wIG9mZnNldD0iMSIvPgo8L3JhZGlhbEdyYWRpZW50Pgo8L2RlZnM%2BCjwvc3ZnPgo%3D)](https://gitbutler.com/)

Using Discord.NET @ https://docs.discordnet.dev/
<br/>Running on Docker, using PostgreSQL
<br/>Equipped with Python using [uvicorn](https://www.uvicorn.org/) and [fastapi](https://fastapi.tiangolo.com/)

Discord Bot for tracking Shared Carts, because we are just that big of V-Tuber Simp and buys merch often... excluding me.

Made this because SirRu was taking too long with his Python version.
Produced by Jagerking779.

Versioning Guide:
<br/>`_._._.x` - Inconsequential Changes, Refactoring, Adding Comments
<br/>`_._.x._` - Minor Changes, Bugfixes
<br/>`_.x._._` - Major Change
<br/>`x._._._` - Remove your current version

## Where to Start

The Bot's main is located at `./discord-net/src/Main.cs`.

All Commands are located inside `./discord-net/src/command-list`, `Commands.cs` responsible for defining the commands with Discord API, meanwhile `Responses.cs` are responsible for what logic is ran for that command. More details on how to use them are inside, I tried simplifying things as much as possible.

All Python code are in `./python-uvicorn`, someone more familiar with python are welcome to reorganize it. I am a Noob.

## To call Python from C#:
Python Side:
```py
# custom.py <-- use the name
from fastapi import APIRouter
from fastapi.responses import PlainTextResponse
from pydantic import BaseModel

router = APIRouter()

class Input(BaseModel):
  a_prop: # type
  other_prop: # type

# generalize both return-type as string
@router.post("/NAME", response_class=PlainTextResponse)
def some_python_func(Input: input)

# main.py
# add these
from app.<path>.custom.py import router as <name>_router
# below app
app.include_router(<name>_router)
```
C# Side:
```cs
// Match the Json keys
var data = new JsonObject {
  ["a_prop"] = json_value_here,
  ["other_prop"] = make_as_much_as_you_need
};
// Match the name with the python's `@app.post`
var (success, value) = await Python.Invoke("NAME", data);
// success will return false on error
```
