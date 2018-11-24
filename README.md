# ConsoleGameEngine
#### C# Graphics Library for drawing graphics in Windows Command Prompt
Olle Logdahl, 3 November 2018

---
**ConsoleGameEngine** is a C# library that wraps around the `System.Console` class, adding enhanced functionality for displaying graphics. Implements a new ConsoleGame abstract, a custom buffer, custom color palette, fullscreen capabilites, input handling and more.

## Installation
- Download .dll *(Unavailable)*
- Clone git repo and build yourself
> git clone https://github.com/ollelogdahl/ConsoleGameEngine.git

## Basic Usage
- Reference the namespace `using ConsoleGameEngine;`
A reference to the ConsoleEngine object is always neccessary, but **NEVER** use more than one in the same application.
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
