# Dungeon Slayer
[![Dungeon Slayer Gameplay](https://img.youtube.com/vi/RljzdSgymck/maxresdefault.jpg)](https://youtu.be/RljzdSgymck)

A powerful, highly-optimized 2D Action RPG / Dungeon Crawler featuring smooth controls, intense 'Hit-Stop' combat natively built in Unity, and dynamic Anime-styled action mechanics.

## Features
- **Fluid Combat System**: Fast-paced melee combat with 'Hit-Stop' and screen-shake impact juice.
- **Shadow Dash**: Speed-blitz dodging mechanic that leaves behind shadow-step afterimages.
- **Awakening System**: Players can trigger a Limit-Break state increasing their combat capabilities.
- **Dynamic Loot System**: Enemies explode with scattered loot (coins, hearts, powerups) upon death.
- **Optimized Core Architecture**: Runs cleanly with zero frame-drops using an Event-driven Stats system.

---

## 🎮 How to Play

Dungeon Slayer features **Universal Cross-Platform Input**. This means Desktop Keyboards, Xbox/PS Gamepads, and Android Touch Screens can all be plugged in and played simultaneously without changing any settings!

### Desktop (Keyboard) Controls
- **WASD / Arrow Keys**: Move left, right, and jump.
- **Spacebar**: Jump.
- **F**: Melee Sword Attack (Triggers Hit-Stop on successful strikes).
- **C**: Fire Bow (Unlimited rapid fire).
- **Left Shift**: Shadow Dash (Leaves behind afterimages).
- **R**: Activate Awakening/Limit Break (Requires maximum Power Stars. Doubles damage output for 10 seconds).

### Console (Gamepad) Controls
- **Left Analog Stick / D-Pad**: Move left and right.
- **[A] Button / [Cross]**: Jump.
- **[X] Button / [Square]**: Melee Sword Attack (Triggers Hit-Stop).
- **[Y] Button / [Triangle]**: Fire Bow (Unlimited rapid fire).
- **[RB] Right Bumper**: Shadow Dash (Leaves behind afterimages).
- **[B] Button / [Circle]**: Activate Awakening/Limit Break.
- **[Start] Button**: Pause / Open Menu.

### Mobile (Touch) Controls
- Use the **On-Screen Joystick** on the left side to move.
- Tap the **Jump Button** (Up Arrow icon) on the right side.
- Tap the **Attack / Bow Buttons** on the right side to engage in combat.
- *(Note: Thanks to input refactoring, Keyboard controls remain fully active even when the Mobile GUI is visible on screen!)*

---

## 🛠 Developer Setup

If you wish to download the source code, view the C# architecture, or modify the game yourself, follow these steps:

### 1. Required Software

- **Unity Editor**: The project has been upgraded and optimized for **Unity 6 (6000.4.1f1)**.
  - To install it automatically, use the Unity Hub and search for `6000.4.1f1`.
- **Java Development Kit (JDK)**: If building for Android, **OpenJDK 17** is required by Unity 6.
- **Code Editor**: Install **Visual Studio Code** (VS Code) with the standard C# extensions for syntax highlighting.

### 2. Installation Steps
1. Clone this repository to your local machine using Git Desktop or terminal.
2. Open **Unity Hub**, click **Add Project from Disk**, and select the `dungeonslayer` root folder.
3. Unity will resolve the packages automatically. Once the project opens, go to `Edit > Preferences > External Tools` and make sure your External Script Editor is set to Visual Studio Code.
4. Double-click the `Menu.unity` scene in the `Assets/Scenes` folder.
5. Hit the **Play** button at the top to test the project!

---

## Authorship & Credits
**Authored by: Abdus Samad Raeen**  

**Special Credits & Original Implementation:**  
Some foundational assets & architecture were originally developed by [Kailius](https://github.com/Walkator/Kailius). Huge thanks for providing the base template for this project to expand upon.

*Built with Unity.*
