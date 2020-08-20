# ConsoleGameEngine
### C# Graphics Library for drawing graphics in Windows Command Prompt
Olle Logdahl, 24 November 2018

![downloads](https://img.shields.io/github/downloads/ollelogdahl/ConsoleGameEngine/total)
![licence](https://img.shields.io/github/license/ollelogdahl/ConsoleGameEngine)
![issues](https://img.shields.io/github/issues-raw/ollelogdahl/ConsoleGameEngine)

---
**ConsoleGameEngine** is a C# library that wraps around the `System.Console` class, adding enhanced 
functionality for displaying graphics. Implements a new ConsoleGame abstract, a custom buffer, custom 
color palette, fullscreen capabilites, input handling and more.

<p align="center">
  <img src="https://github.com/ollelogdahl/ConsoleGameEngine/blob/master/Media/monkeyspin.gif" />
</p>

## Installation / Getting Started
- [Download Lastest Build](https://github.com/ollelogdahl/ConsoleGameEngine/releases/)
- Clone git repo and build yourself
> git clone https://github.com/ollelogdahl/ConsoleGameEngine.git

<br />

After installing you'll have to:
1. Import `ConsoleGameEngine.dll` to project.
2. Reference the namespace `using ConsoleGameEngine;`

---

## Why?
I created this Library to make graphics more available for beginners and hobbyists alike. The first programs 
you create are usually made in the console, but when users want to migrate to actual graphics there is a steep 
learning curve. My ambition with this library is to depricate the need for psuedo-graphics in the console, 
usually done by moving the cursor, writing a short string and clearing the actual screen. Not only is that 
solution unintuitive in the long run, but also highly inefficient.

#### Uses
- retro-terminal-styled games and applications
- easy-to-use graphics library for basic and advanced graphics in games and applications
- ~~Creating heavy 3D graphics running in 4K~~

Does the last apply to you? Then sorry, *this is not the library you are looking for.*

## Usage / Features
Library contains two main classes, `ConsoleEngine` and `ConsoleGame`

- Custom character screen buffer, allows clearing and blitting to console window
- Console colors with full rgb capabilities
- Custom & premade Palettes, used for changing console window palette
- Accessing and setting pixels individually
- Functions to draw basic shapes and primitives (Triangles, Rectangles, Lines etc.)
- Writing characters to screen using plain-text and FIGlet fonts
- Multiple game loops, including fixed framerate and deltatime settings
- Point and Vector class, for int and float positions
- Setting console window settings, changing window size and running console borderless
- Input handling

#### ConsoleEngine
Is used to draw to the screen, replacement for the `System.Console` class *(kind of)*

```c#
using ConsoleGameEngine;
...
Engine = new ConsoleEngine(windowWidth, windowHeight, fontWidth, fontHeight);

Engine.SetPixel(new Point(8, 8), ConsoleCharacter.Full, 15);

```

#### ConsoleGame
Keeps an instance of the `ConsoleEngine` and implements game loops.

**Note** *Not neccessary, you could use the ConsoleEngine as is*

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

##### Try out some example games over [here](https://github.com/ollelogdahl/ConsoleGameEngine/tree/master/Examples)

## Notes
- Color palette limited to 16 colors in a single session *(this is an internal limitation, see [MDSN](https://docs.microsoft.com/en-us/windows/console/console-screen-buffer-infoex))*
- Only **ONE** reference to a `ConsoleEngine` is allowed per session
- Press *Delete Key* to close application if running in borderless
---

<p align="center">
  <img src="https://github.com/ollelogdahl/ConsoleGameEngine/blob/master/Media/cave.gif" width = 512 heigth = 384 />
</p>

## Links

- [Repository](https://github.com/ollelogdahl/ConsoleGameEngine/)
- For reporting errors, visit [Issue Tracker](https://github.com/ollelogdahl/ConsoleGameEngine/issues)!
- Related Projects:
  - [olcConsoleGameEngine (c++)](https://github.com/OneLoneCoder/videos/blob/master/olcConsoleGameEngine.h) by Javidx9
  - [ColorfulConsole (C#)](http://colorfulconsole.com/) by Tom Akita
- Special Thanks to:
  - [pinvoke.net](http://www.pinvoke.net/) by Redgate Software, for windows api documentation
  - [ScreenToGif](https://www.screentogif.com) by Nicke Manarin, for making screen-gif capturing easy :)

## Licensing

This project, and all code it contains, is licensed under *The Unlicense* and can be read [here](UNLICENSE).
