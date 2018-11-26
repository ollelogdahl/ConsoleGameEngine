# ConsoleGameEngine
### C# Graphics Library for drawing graphics in Windows Command Prompt
Olle Logdahl, 24 November 2018

---
**ConsoleGameEngine** is a C# library that wraps around the `System.Console` class, adding enhanced functionality for displaying graphics. Implements a new ConsoleGame abstract, a custom buffer, custom color palette, fullscreen capabilites, input handling and more.

<p align="center">
  <img src="https://imgur.com/wY3RFru.jpg" />
</p>

## Installation / Getting Started
- Download `.dll` *(Unavailable)*
- Clone git repo and build yourself
> git clone https://github.com/ollelogdahl/ConsoleGameEngine.git

<br />

After installing you'll have to:
1. Import `ConsoleGameEngine.dll` to project.
2. Reference the namespace `using ConsoleGameEngine;`

---

## Usage / Features
Library contains two main classes, `ConsoleEngine` and `ConsoleGame`
#### ConsoleEngine
Is used to draw to the screen, replacement for the `System.Console` class *(kind of)*

- Custom screen buffer, allows clearing and blitting
- Palettes, changing the 16 available colors
- Input handling
- Graphic Primitives

```c#
using ConsoleGameEngine;
...
Engine = new ConsoleEngine(windowWidth, windowHeight, fontWidth, fontHeight);

Engine.SetPixel(new Point(8, 8), ConsoleCharacter.Full, 15);

```

#### ConsoleGame
Has an instance of the `ConsoleEngine`, and implements a **Game Loop**

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

## Notes
- Color palette limited to 16 colors in a single session *(this is an internal limitation, see [MDSN](https://docs.microsoft.com/en-us/windows/console/console-screen-buffer-infoex))*
- Only **ONE** reference to a `ConsoleEngine` is allowed per session

---

## Links

- [Repository](https://github.com/ollelogdahl/ConsoleGameEngine/)
- For reporting errors, visit [Issue Tracker](https://github.com/ollelogdahl/ConsoleGameEngine/issues)!
- Related Projects / Special Thanks:
    - [olcConsoleGameEngine (c++)](https://github.com/OneLoneCoder/videos/blob/master/olcConsoleGameEngine.h) by Javidx9
    - [ColorfulConsole (C#)](http://colorfulconsole.com/) by Tom Akita

## Licensing

This project, and all code it contains, is licensed under *The Unlicense*, which can be read [here](UNLICENSE)
