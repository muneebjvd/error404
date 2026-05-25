# Error 4-0-4 🚪👁️

![Unity](https://img.shields.io/badge/Unity-100000?style=for-the-badge&logo=unity&logoColor=white)
![C#](https://img.shields.io/badge/C%23-239120?style=for-the-badge&logo=c-sharp&logoColor=white)
![Status](https://img.shields.io/badge/Status-Completed-success?style=for-the-badge)

**🏆 1st Place Winner – GDG CodeRush 2026 Game Jam**

### 🎮 [Play Error 4-0-4 on Itch.io](https://munoob.itch.io/error404)

**Error 4-0-4** is an atmospheric, psychological horror-puzzle game built around spatial loops and dimensional shift mechanics. The game challenges players to navigate a shifting, non-Euclidean reality where progression is gated by strict environmental logic rather than conventional exploration.

---

## 📖 Overview

The world is a loop, and the loop is a gate. Players are trapped in an oppressive 3D liminal environment that seamlessly transitions into three distinct 2D "Breach" dimensions. The environment is actively hostile, evaluating player actions through a virtual logic gate system. 

Built with **100% custom C# logic** and zero pre-built script packages, the game was architecturally optimized for low-end PC hardware while maintaining a dense, eerie atmosphere.

## ✨ Key Features
* **Hybrid 2D-in-3D Gameplay:** Seamlessly shifts from first-person 3D exploration to embedded 2D retro-mechanics (Cyber Platformer, Top-Down Infiltration, Survival Gauntlet).
* **Non-Euclidean Spatial Loops:** A custom "Slam Shut" conditional door algorithm physically locks down and resets paths if the player deviates from the required 4-0-4 numerical sequence.
* **The "Savage Fail" System:** A high-stakes, punitive failure state that strategically resets room configurations and taunts the player upon defeat.
* **PS1 Retro-Horror Aesthetic:** Low-fidelity visual language, baked lightmapping, and dynamic audio occlusion create nostalgic, uncanny dread.

---

## 🛠️ Technical Architecture

This project was developed from first principles, demonstrating advanced Unity engine utilization and robust software architecture:

### State Management & Architecture
* **The Singleton Command Pattern:** Uses a persistent, scene-independent `CoreGameLogic` manager to maintain global state integrity across five distinct scenes.
* **Dynamic Reference Binding:** Implemented a custom `RefreshReferences` system that dynamically re-binds all object references upon scene load to eliminate null-reference exceptions.
* **Asynchronous Loading:** Built a custom transition handler utilizing `SceneManager.LoadSceneAsync` with Canvas overlays to eliminate frame drops and preserve atmospheric continuity.

### Custom Physics & AI 
* **Ghost AI Stalker:** A custom-coded AI entity that awakens via trigger-based events. It utilizes a **Triple-Raycast Obstacle Avoidance** system to dynamically adjust its heading vector around wall colliders, delivering relentless pursuit without relying on Unity's built-in pathfinding.
* **Custom 2D Physics Systems:** Extended Unity's 2D Rigidbody dynamics with custom variables (e.g., `slipFactor` for vehicle drifting) and implemented custom `BoxCast` ground-checking for pixel-perfect jumping in the 2D dimensions.

### Optimization
* **Low-End Hardware Target:** Architecturally optimized to run at a stable framerate on systems with 8 GB RAM and no dedicated GPU. Achieved through lean C# code, strict polygon budgets, and avoiding expensive real-time shader effects.

---

## 🎮 Controls

* **WASD / Arrow Keys:** Move / Steer
* **Spacebar:** Jump / Interact (in 2D Breaches)
* **Mouse:** Look / Camera Control

---

👨‍💻 Development & Credits

    Lead Developer & Programmer: M. Muneeb Javed

    Engine: Unity Engine

    Language: C#

    Assets: 91%+ Self-authored original assets. (~9% sourced from Unity Asset Store and freesound.org under applicable licenses).

