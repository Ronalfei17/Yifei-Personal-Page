[English](README.md) | [中文](README_CN.md)

# AR Cocktail Menu

**Demo Video:** [Watch here](https://www.youtube.com/shorts/217OUgr1wzI)  
**Detailed Report:** [Download PDF](AR_Cocktail_Report.pdf)  
**Platform:** Unity / AR Foundation / Mobile AR  
**Team:** Pranavv Jothinathan, Zhihan Yu, Yifei Liu, Skyler Sun

<img src="Images/hero.png" width="80%">

## 1. Project Overview

AR Cocktail Menu is a mobile augmented reality ordering prototype that turns a traditional text-based cocktail menu into an interactive 3D product presentation. After scanning a physical coaster marker, the application anchors a virtual cocktail menu onto the real table and allows users to inspect drinks through touch gestures, model switching, and a close-up 360-degree view mode.

The project focuses on realistic product visualization in AR: stable image tracking, readable gesture interaction, dynamic lighting, environmental reflections, real-time shadows, and optimized 3D assets.

## 2. Interaction Flow

Users scan the coaster marker, wait for the virtual menu to appear, swipe to switch between cocktails, short press ingredients to view component details, long press the cocktail to view basic information, and enter View Mode to inspect the model more closely.

<img src="Images/interaction_flow.png" width="80%">

## 3. Key Features

### Marker Tracking and Animated Menu Generation

The system uses AR Foundation image tracking to connect the physical coaster with the virtual menu. Instead of instantiating a heavy menu prefab only after tracking succeeds, the project pre-instantiates and hides the menu, then activates and binds it to the tracked image when the marker becomes stable. This reduces visible first-frame delay and helps the AR object feel attached to the real surface.

### Touch-Based Information Design

The interaction system converts screen touches into 3D raycasts. A short press opens ingredient information, while a long press opens basic cocktail information and triggers haptic feedback. Movement thresholds are used to avoid confusing press gestures with swipe gestures.

<img src="Images/press_interaction.png" width="80%">

### Swipe Switching and 360-Degree View Mode

Horizontal swipe gestures switch between three cocktail models. View Mode temporarily separates the current cocktail from the AR menu, places it closer to the camera, disables normal swipe switching, and lets the user rotate the drink model directly.

### Lighting, Shadows, and Reflection

The project uses AR light estimation, a transparent shadow-receiving plane, and Environment Probes to make glass materials and virtual shadows respond more convincingly to the real environment.

<img src="Images/lighting_reflection.png" width="80%">

### 3D Asset Pipeline

The team explored photogrammetry, RealityCapture, Maya optimization, double-sided materials, and shader-based liquid simulation. Because transparent glass was difficult to reconstruct reliably, the final pipeline used a bamboo base model with optimized geometry and combined it with cocktail assets inside Unity.

<img src="Images/model_pipeline.png" width="80%">

## 4. Technical Stack

- Unity
- AR Foundation
- ARTrackedImageManager
- ARImageSmoothFollow
- DOTween
- Physics raycasting
- Light Estimation API
- Environment Probe Manager
- RealityCapture
- Maya
- Stencil Shader experiments

## 5. Challenges and Solutions

- Tracking delay: reduced by pre-instantiating the menu and binding it after image tracking succeeds.
- Gesture conflict: handled by separating long press, short press, and swipe with duration and movement thresholds.
- Visual realism: improved through brightness remapping, real-time shadows, environment reflections, and material tuning.
- Asset performance: handled by reducing scanned mesh complexity while preserving visual quality.

## 6. Evaluation and Future Work

The user survey showed strong overall interest in the application, with the project receiving high scores for visual appeal and animation quality. The main improvement areas were gesture reliability, clickable target size, model texture detail, and more physically realistic liquid effects.

<img src="Images/evaluation.png" width="80%">

Future iterations could improve tracking stability, restrict View Mode rotation to avoid unexpected flipping, add more cocktails, and extend the ordering scenario with food pairing, voice interaction, or immersive audio.

## 7. Resources

- [Demo Video](https://www.youtube.com/shorts/217OUgr1wzI)
- [Detailed Technical Report](AR_Cocktail_Report.pdf)
- [Image Placement Guide](Images/IMAGE_GUIDE.md)
