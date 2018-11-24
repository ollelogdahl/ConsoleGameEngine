# ConsoleGameEngine
#### C# Graphics Library for drawing graphics in Windows Command Prompt
Olle Logdahl, 3 November 2018

---
**ConsoleGameEngine** is a C# library that wraps around the `System.Console` class, adding enhanced functionality for displaying graphics. Implements a new ConsoleGame abstract, a custom buffer, custom color palette, fullscreen capabilites, input handling and more.

*Un*license can be read [here](UNLICENSE)

## Installation
- Download `.dll` *(Unavailable)*
- Clone git repo and build yourself
> `git clone https://github.com/ollelogdahl/ConsoleGameEngine.git`

## Notes
- Color palette limited to 16 colors in a single session *(this is an internal limitation, see [MDSN](https://docs.microsoft.com/en-us/windows/console/console-screen-buffer-infoex))*
- Only **ONE** reference to a `ConsoleEngine` is allowed per session
---

## Basic Usage
- Reference the namespace `using ConsoleGameEngine;`
#### ConsoleEngine

```c#
using ConsoleGameEngine;
...
Engine = new ConsoleEngine(windowWidth, windowHeight, fontWidth, fontHeight);

Engine.SetPixel(new Point(8, 8), ConsoleCharacter.Full, 15);

```


#### ConsoleGame
```c#
using ConsoleGameEngine;
...

new AppName.Construct(windowWidth, windowHeight, fontWidth, fontHeight, FramerateMode.Unlimited);
class AppName : ConsoleGame {
  public override void Create() {
  }
  
  public override void Update() {
  }
  
  public override void Render() {
  }
}
```
