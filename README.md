# 🎧 VR Room — VR2025 Course Project

This repository contains the complete source files for our VR interactive room, built with Unity and XR Toolkit. It includes four core interactive systems, each designed and implemented by our team.

## 🔧 Core Features and Contributions

---

### 🎯 1. Shooting System (`Assets/ShootPart`)
**Description**: A sound-based shooting interaction. Hold to charge, release to shoot. Player shoots at different heights of the resonance pillar, each mapped to a musical pitch.

- **Produced scripts/assets**: 
  - all in `ShootPart/Scripts/` 
  - `Shooter.prefab`, `Bullet.prefab`, particle effects
- **Scene content**: Audio-reactive resonance pillar embedded in room
- **Unmodified assets**: 
  - Charge/Fire/Squash audio: non-license SFX found in https://on-jin.com/sound/ta.php  
  - Pillar audio: Pitch sound when hit: https://pixabay.com/sound-effects/harp-metal-97812/

---

### 🪢 2. Rope Swing System (`Assets/RopeSwing`)
**Description**: A physics-based rope swinging system allowing players to traverse space by grabbing onto swingable points. The `RopeSwing` prefab is placed under the XR rig and includes necessary references such as the player's Rigidbody, swing origin transform, and hand positions. It also incorporates a LineRenderer for visualizing the rope, body collider, and custom swing logic.

- **Produced prefabs**: `BodyCollider.prefab`, `RopeSwing.prefab`, `SwingTutorial.prefab`
- **Adapted scripts/assets**:  
  Conceptual inspiration from Valem Tutorials' rope system using `SpringJoint`: https://www.youtube.com/watch?v=8Nwy3sFcNvg. Our version uses `ConfigurableJoint` with a reworked structure, integrated pulling, rope shortening, and angular damping.  
  `ClimbableHandles` adapted from XR Toolkit demo, modified to suit our layout.
- **Unmodified assets**: Fall SFX in `RopeSwing/Sounds` from https://pixabay.com/zh/sound-effects/body-falling-to-ground-100474/

---

### 🧩 3. Puzzle Platform (`Assets/PuzzlePart`)
**Description**: A crystal puzzle platform with interactive sockets and a button, alongside a locked door. Players must place crystals in the correct order based on riddle hints to unlock the door.

- **Produced scripts/assets**:  
  - `PuzzlePart/Scripts/SocketItemAudioTrigger.cs`  
  - `PuzzlePart/Scripts/CrystalGlowController.cs`  
  - `PuzzlePart/Scripts/DoorButton.cs`  
  - `Demo/Scripts/Door.cs`  
  - Riddle clue designs
- **Adapted scripts/assets**:  
  - Puzzle logic adapted from https://youtu.be/iSYfs6NXZck?si=uBktWq3sAl0s-lww  
  - Crystal models adapted from Unity Asset Store: "Translucent Crystals"  
  - Door asset: `Free Wood Door Pack`
- **Unmodified assets**: 
  - Audio: https://pixabay.com/sound-effects/

---

### 🎵 4. Musical Instrument – Kalimba & Resonance Pillar
**Description**: Two music-based interactions: a playable kalimba (thumb piano) and tonal sound pillars reacting to shooting input.

#### Kalimba
- **Produced scripts/assets**:  
  - `Scripts/ThumbPiano.cs`, `Scripts/BoxShaker.cs`, `Scripts/LidSlider.cs`, `Scripts/MusicPuzzle.cs`
  - Prefab: `ThumbPiano_clue.prefab`  
- **Adapted assets**: `LowPolyAfricanInstruments` for kalimba model  
- **Unmodified assets**: 
  - `iPoly3D/Prototype Textures/Materials` for material design  
  - Audio: from https://pixabay.com/



#### Resonance Pillar  
Part of the shooting system. See Section 1.

---

### 5. Miscellany

#### Room Scene Construction
**Description**: Constructed the base room layout and environment using Unity Learn’s official VR asset packs. Includes walls, flooring, and furniture.
- **Adapted assets**: 
  - `Assets/_Course Library/`, found in https://learn.unity.com/pathway/vr-development

#### Falling Resume
**Description**: Automatically resets objects that fall outside the scene.
- **Produced scripts/assets**:
  - `Assets/Scripts/FallResumeTrigger.cs`
- **Unmodified assets**: 
  -  Audio:  
    - Fall: https://pixabay.com/sound-effects/magic-descend-1-259522/  
    - Resume: https://pixabay.com/sound-effects/magic-ascend-2-259523/

#### Game Process Control

##### Scene Transit 
- **Produced scripts/assets**:  
  - `ScenePortal.cs`, `BlackScreenFader.cs`, `Fader.cs`
- **Unmodified assets**: 
  - Audio: Pixabay non-license SFX

##### Ending Control  
- **Produced scripts/assets**: 
  -  `Assets/TheEnd/`
- **Unmodified assets**: 
  - Audio: Pixabay non-license SFX  
  - Font: Lacquer from https://fonts.google.com/specimen/Lacquer

---

## 📦 Shared Systems and External Dependencies

### Used as-is
- `XR/`, `XRI/`: Unity XR Interaction Toolkit (teleportation, grab, movement)
- `TextMesh Pro/`: UI text rendering
- `AN Interactive Physical Door Pack`: Background or decorative assets

---

## 📁 Project Structure Overview

```text
Assets/
├── _Course Library/        # Room scene and furniture assets
├── ShootPart/              # Shooting logic, bullets, sound pillars
├── RopeSwing/              # Rope swing scripts and prefabs
├── PuzzlePart/             # Puzzle platform logic and prefabs
├── Scripts/                # Kalimba and sound interaction scripts
├── Scenes/                 # Main Unity scenes
├── AudioClip/              # Kalimba and puzzle audio assets
├── TheEnd/                 # Ending screen logic and UI
├── XR/, XRI/               # XR Toolkit dependencies

All other folders under Assets/ not mentioned above contain either unused or redundant asset packs, or serve as generic architectural and material references during scene construction.
```

---

## 👥 Repository Access

This repository is **public** and accessible at:  
🔗 https://github.com/Veture1/VR2025/tree/main

---

## 📝 Notes

- Scripts not explicitly listed above are either Unity standard components or part of the XR Toolkit and were used as-is without modification.
- Unity version: Unity 6 (6000.0.40f1)
- To run the project, open any scene from `Scenes/` in Unity. All asset references should auto-load in a properly configured XR environment.
