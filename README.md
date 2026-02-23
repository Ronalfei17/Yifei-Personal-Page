# Sonic Playground: Immersive Audio Design Project

**Project Lead:** Yifei Liu, Pranavv Jothinathan, Suhang Liu, Skyler Sun  
**Date:** December 12, 2025  
**Platform:** VR (Meta Quest 3) / Unity

---

## 1. Project Overview
**Sonic Playground** is an interactive virtual reality exhibition designed to explore how sound behaves in physical space through direct user interaction. The project adopts a museum-style layout consisting of three interconnected exhibits: **Resonance**, **Cave Acoustics**, and **Material-Dependent Sound**. 

The experience is designed to be exploratory and educational, allowing users to engage with sound as a physical and spatial phenomenon rather than just passive background audio.

<img src="./images/Sonic_Playground_Inital.png" width="50%">

*Note: This project supports hand tracking and controller interaction on Meta Quest 3.*

---

## 2. Key Exhibits

### 🎤 Exhibit 1: Resonance
**Concept** Explores how objects vibrate when exposed to specific frequencies.

**Interaction** Users record their own voice, and the system performs a real-time Fast Fourier Transform (FFT) to analyze the frequency components.

**Feedback** When the user's voice frequency matches the natural frequency of the virtual wine glass, the glass vibrates intensely and eventually shatters.

<img src="./images/Sonic_Playground_Exhibt_1.png" width="50%">

### ⛰️ Exhibit 2: Cave Acoustics
**Concept** Demonstrates the contrast between open-air and enclosed environmental acoustics.

**Experience** As users transition from the outdoor environment into a cave, they experience immediate changes in reverberation, echo, and reflection through footsteps and collisions.

**Technology** Built using the **Steam Audio** plugin for HRTF spatialization and occlusion, providing a realistic sense of scale and depth.

<img src="./images/Sonic_Playground_Exhibt_2.png" width="50%">

### 🔵 Exhibit 3: Material-Dependent Sound
**Concept** Investigates how physical properties (mass, friction, and material) influence sound generation.

**Interaction** Users can grab and roll balls made of wood, plastic, and metal to hear distinct acoustic signatures.

**Implementation** The audio pitch and volume are dynamically adjusted using `Mathf.Lerp` based on the real-time velocity and collision intensity of the objects.

<img src="./images/Sonic_Playground_Exhibt_3.png" width="50%">

---

## 3. Technical Stack
**Engine** Unity 6 (2023.3.0b8)

**Audio Spatialization** Steam Audio Plugin (for HRTF and physics-based acoustics)

**Interaction** XR Interaction Toolkit (supporting Hand Tracking)

**Core Algorithms** 
1. Real-time FFT Frequency Analysis  
2. Velocity-based Dynamic Pitch Shifting  
3. Physics Material Integration

---

## 4. Resources & Links
[Project Demo Video (Bilibili)](https://www.bilibili.com/video/BV1KJFvz4RD)  
[Detailed Design Document (PDF)](./Immersive_Audio.pdf)

---
*© 2025 Sonic Playground Development Team*