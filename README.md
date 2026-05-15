# Survival Game – Unity C# Project

![Unity](https://img.shields.io/badge/Unity-100000?style=flat-square&logo=unity&logoColor=white)
![C#](https://img.shields.io/badge/C%23-239120?style=flat-square&logo=c-sharp&logoColor=white)
![Git](https://img.shields.io/badge/Git-F05032?style=flat-square&logo=git&logoColor=white)

A 3D survival game developed as a portfolio piece for admission to the **S4G School for Games** (Game Engineering program). The player must collect coins while managing health, stamina, and fear within a limited time. Includes enemy AI, dash mechanics, environmental interactions, and a complete UI system.

[**Play on Itch.io**](https://sevchikk.itch.io/survival-game-unity) | [**Source Code**](https://github.com/sevchikk/Unity-Survival-Game-S4G)

---

## ✨ Features

- **Player Mechanics** – walk, run, dash (up to 3 dashes with stamina consumption), health system
- **Two Stat Systems** – physical damage and intimidation (fear) affecting gameplay
- **Enemy AI** – three-state behavior (patrol, chase, attack) with timers, no coroutines
- **Environmental Interaction** – tree sway simulation with Perlin noise and uprooting via physics forces
- **Dynamic UI** – real‑time health, fear, stamina, and dash icons; configurable settings panel
- **Death & Respawn System** – lose coins upon respawn with increasing fear; time limit end screen
- **Clean Architecture** – separated concerns: `Player` (data), `PlayerDamageHandler`, `PlayerMovement`
- **Computed Properties** – `_isPlayerInRange =>` for readable state checks

---

## 🛠️ Built With

- **Unity 6.1** (C#)
- **Rigidbody physics** with `MovePosition` for smooth movement
- **Animator** for player and enemy animations
- **Procedural tree sway** using Perlin noise
- **Version control** – Git (GitHub Desktop)

---

## 🎮 Controls

| Action          | Keys                          |
| :-------------- | :---------------------------- |
| Move            | `WASD` / Arrow keys           |
| Run             | Hold `Left Shift`             |
| Dash            | `Space` (consumes stamina)    |
| Menu navigation | Mouse                         |

---

## 📦 Getting Started

### Clone the repository

```bash
git clone https://github.com/sevchikk/Unity-Survival-Game-S4G.git
```

### Open in Unity Hub

1. Open **Unity Hub**.
2. Click **Add project** → select the cloned folder.
3. Ensure Unity version **2022.3 LTS** or higher (project built with Unity 6.1).
4. Once loaded, open `Assets/Scenes/MainMenu.unity`.
5. Press **Play** in the Editor or build for your platform.

### Build the game

1. Go to `File → Build Settings`.
2. Select your target platform (Windows / Mac / Linux / WebGL).
3. Ensure both scenes (`MainMenu` and `Game`) are checked.
4. Click **Build** and choose an output folder.
5. *(For WebGL)* – set compression format to **Gzip** or **Disabled**.

---

## 📸 Screenshots

![Gameplay](Screenshots/gameplay.gif) *(add your own screenshot)*  
![Gameplay 2](Screenshots/gameplay2.png) *(add your own screenshot)*

---

## 🔗 Links

- **Playable build** – [Itch.io page](https://sevchikk.itch.io/survival-game-unity)
- **Source code** – [GitHub repository](https://github.com/sevchikk/Unity-Survival-Game-S4G)
- **S4G School for Games** – [game engineering program](https://www.school4games.net/game-engineering/)

---

## 📄 License

This project is licensed under the **MIT License** – see the [LICENSE](LICENSE) file for details.

---

**Made by Vsevolod (Seva) Stupak** – solo developer (code, design, assets).  
*July 2025*
