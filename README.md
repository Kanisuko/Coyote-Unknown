# Coyote Unknown

A gameplay-ready, modular, open-source [BepInEx](https://github.com/BepInEx/BepInEx) mod bridging **DG-Lab Coyote** electrostimulation hardware to the game **Casualties Unknown**, cleanly built using the [ScavLib API](https://github.com/Kanisuko/ScavLib-API-DLL-Repository).

---

## IMPORTANT DEVELOPMENT & SAFETY DISCLAIMER
**This integration is now ready for basic regular gameplay.** 

* **Verified in Emulators:** Because the developer currently **does not have a physical Coyote device on hand**, WebSocket protocol handshakes, responsive control panel telemetry, and state-tracking calculations have been verified using software emulators. Real-world feedback and fine-tuning rely heavily on player suggestions!
* **Use At Your Own Risk:** If you decide to test this with actual hardware, please **start with extremely low safety limits (MaxStrengthA/B set to 5 - 10)** in the configurations. 

---

## Key Features

### Dedicated Biological Debuff Tracking (Pro Mode)
Exposes 5 game-specific bodily debuffs to custom multipliers and waveform configurations:
* **Radiation Sickness:** Tracks continuous radioactive exposure.
* **Hypothermia/Cold:** Adapts dynamically as body temperature drops below 35.5°C.
* **Hypoxia/Suffocation:** Escalates as blood oxygen falls below 90%.
* **Fractures & Dislocations:** Scans all 14 limbs to detect broken bones or dislocated joints.
* **Poison & Venom:** Translates toxic venoms in your bloodstream.

### Event-Driven Waveform Library
Features a rich, pre-defined physical wave dictionary (including *HeavyShock* for damage, *Sting* for pain, *Heartbeat* for stress, and *SoftBuzz* for background buzzes). Instant impact events (like losing blood or sudden pain spikes) directly fire these specific wave pulses with built-in cooldown protection (0.5s) to avoid app crashes or flooding.

### Dual Strength Control Modes
* **Follow Client:** Scales continuous output proportionally relative to the sliding limits set on your phone's App (excellent for safety).
* **Follow Mod:** Follows absolute value settings directly, capped at the App's sliding limits.

### Double Safety Protection & Global Scaling
Configurable Mod-side absolute limits (A/B) that strictly clamp output regardless of other settings, paired with a global multiplier slider to adjust overall intensity at once.

### Web-Based Double-Column Panel
Access `http://localhost:9999` to configure. When Professional Mode is toggled, the web console dynamically scales into a high-tech **double-column desktop layout**, ensuring no empty space on your screen. Save changes, toggle debuffs, and customize waveform mapping in real-time.

### Clean In-Game UI (F6)
The in-game F6 menu is locked to a simplified layout to keep gameplay clean and uninterrupted. Allows quick adjustment of global multipliers, limits, and copying the localhost browser link directly.

---

## Requirements
* [Casualties Unknown](https://store.steampowered.com/app/4576510/_/) (Game)
* [BepInEx 5](https://github.com/BepInEx/BepInEx) (Mod Loader)
* [ScavLib API](https://github.com/Kanisuko/ScavLib-API-DLL-Repository) (v0.2.2 or higher)
* DG-Lab mobile app & e-estim hardware (for physical feedback)

---

## Installation
1. Ensure BepInEx and ScavLib are correctly installed.
2. Download the latest release and place `CoyoteUnknown.dll` into your `BepInEx/plugins/` directory.
3. Launch the game. The micro-server will automatically start hosting on port `9999`.
4. Open your browser and navigate to `http://localhost:9999` (this web console URL is automatically copied to your system clipboard on startup).
5. Scan the displayed QR code with your phone's official DG-Lab App to pair.

---

## Contributing
Since this project was developed without physical hardware, community testing and feedback are incredibly valuable! If you own a Coyote device and find any bugs, unexpected stimulation surges, or have telemetry optimization suggestions:
* Please open an **Issue** or submit a **Pull Request (PR)**.
* Live logs can be retrieved directly from the game's BepInEx console or the `/panel` web log viewer.

---

## License
Distributed under the **MIT License**. See `LICENSE` for more information.