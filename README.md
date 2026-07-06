[English](README.md) | [中文](README_CN.md)

# LiarVR

**Demo Video:** [Watch here](https://www.youtube.com/watch?v=oPDAqGpKcfY&t=17s)  
**Detailed Report:** [Download PDF](LiarVR_Project_Report.pdf)  
**Platform:** Meta Quest 3 / Unity / Photon PUN 2 / Meta XR  
**Team:** Pranavv Jothinathan, Yifei Liu, Lintao Guo

<img src="Images/hero.png" width="80%">

## 1. Project Overview

LiarVR is a two-player virtual reality bluffing card game built around social presence, embodied interaction, and turn-based multiplayer synchronization. Instead of presenting cards as a flat digital interface, the game places both players at a shared virtual tavern table where they physically handle cards, observe the opponent's body language, and decide whether to challenge a possible bluff.

The project explores how VR can make social deduction more embodied: players do not only read card states, but also interpret gestures, timing, hesitation, and the opponent's visible actions.

## 2. My Main Contributions

- Evaluated existing VR multiplayer examples and helped decide to build a focused custom prototype from scratch.
- Built the basic two-player shared VR scene using Photon PUN 2.
- Integrated the VR camera and real-body tracking character model.
- Implemented turn-based gameplay logic with event-based control.
- Implemented card initialization, card interaction, card reset, and ownership checks.
- Debugged the card physics issue caused by gravity and Photon synchronization conflicts.
- Tested multiplayer stability, body tracking alignment, challenge logic, and card interactions.

## 3. Game Rules and Flow

Each player starts with five cards: three Queens and two Kings. On their turn, a player places a card into the play box as a claimed Queen. The other player can either continue playing or challenge the previous card. If the challenged card is truly a Queen, the challenged player wins; if it is a King, the challenger wins. A player can also win by successfully playing all five cards.

<img src="Images/game_logic.png" width="80%">

## 4. Key Features

### Two-Player VR Multiplayer

The project uses Photon PUN 2 to create a two-player shared scene. The system separates the local VR camera from the network-synchronized player prefab, allowing each headset to drive its own view while Photon synchronizes player objects and gameplay state.

### World-Space Player Guidance

Because the game combines room joining, turn-taking, physical card handling, bluffing, and challenge timing, a state-driven tutorial panel guides players during the first round. The UI is displayed in world space so it feels part of the VR environment rather than a flat overlay.

<img src="Images/tutorial_ui.png" width="80%">

### Embodied Avatar and Body Tracking

To make bluffing readable, the prototype uses a character model driven by Meta Movement SDK body tracking. This gives the opponent visible body language instead of showing only controllers or hands.

<img src="Images/body_tracking.png" width="80%">

### Turn-Based Card Logic

The round logic is handled through a MasterClient-based flow. Card play requests and challenge requests are validated centrally, then synchronized to all clients so both players share the same round state, card result, and win/lose feedback.

### Card Interaction Debugging

One major technical issue came from the conflict between VR grab physics and Photon network updates. Locally, the grabbed card behaved correctly, but remotely the card jittered. Through component-level debugging, the team traced the problem to physics/gravity conflict and adjusted the card synchronization behavior for smoother remote viewing.

<img src="Images/card_interaction.png" width="80%">

### End-Game Feedback

Victory and defeat states are reinforced through audio, UI text, and Unity Particle System effects. The winner receives a fire-based celebration effect, while the losing player receives a smoke-based feedback sequence.

<img src="Images/endgame_effects.png" width="80%">

## 5. Technical Stack

- Unity
- Meta Quest 3
- Meta XR All-in-One SDK
- Meta Movement SDK
- Photon PUN 2
- Photon View
- Photon Transform View
- Unity physics triggers
- Unity Particle System

## 6. Evaluation and Future Work

The user study showed that visual and spatial presence were relative strengths of the prototype, while interface stability and comfort were the clearest areas for improvement. Open-ended feedback also highlighted hand-based card grabbing, sound, lag, dizziness, and the desire for richer multiplayer interaction.

<img src="Images/evaluation.png" width="80%">

Future work should improve synchronization stability, reduce discomfort, add clearer turn and challenge feedback, refine card haptics and animations, and strengthen the role of body language or physiological cues in bluffing.

## 7. Resources

- [Demo Video](https://www.youtube.com/watch?v=oPDAqGpKcfY&t=17s)
- [Detailed Technical Report](LiarVR_Project_Report.pdf)
- [Image Placement Guide](Images/IMAGE_GUIDE.md)
