# Coyote Unknown

An experimental, open-source [BepInEx](https://github.com/BepInEx/BepInEx) mod bridging **DG-Lab Coyote** electrostimulation hardware to the game **Casualties Unknown**, cleanly built using the [ScavLib API](https://github.com/Kanisuko/ScavLib-API-DLL-Repository).

---

## ⚠️ IMPORTANT BETA DISCLAIMER
**This is an early-stage, experimental test release.** 

* **No Physical Hardware Testing Yet:** The developer currently **does not have a physical Coyote device on hand**. 
* **What Has Been Verified:** Only the local WebSocket server handshakes, local web-panel configuration telemetry, and in-game vital tracking calculations (such as heart rate, pain spikes, and blood loss) have been simulated and validated in code. 
* **Use At Your Own Risk:** If you decide to test this with actual hardware, please **start with extremely low safety limits (MaxStrengthA/B set to 5 - 10)** in the configurations. 

---

## Features
* **Web-Based Live Configurations:** Access a local dashboard at `http://127.0.0.1:9999/panel` via any browser or mobile device to adjust multipliers, set safety limits, and toggle modules on the fly. All changes are automatically saved directly to the game's `.cfg` configuration file.
* **ScavLib Integration:** Fully integrated with the ScavLib overlay framework. Press `F6` in-game to bring up the basic settings overlay, or using `coyote ui` in console.

---

## Requirements
* [Casualties Unknown](https://store.steampowered.com/app/4576510/_/) (Game)
* [BepInEx 5](https://github.com/BepInEx/BepInEx) (Mod Loader)
* [ScavLib API](https://github.com/Kanisuko/ScavLib-API-DLL-Repository) (v0.2.0 or higher)
* DG-Lab mobile app & e-stim hardware (for physical feedback)

---

## Installation
1. Ensure BepInEx and ScavLib are correctly installed.
2. Download the latest release and place `CoyoteUnknown.dll` into your `BepInEx/plugins/` directory.
3. Launch the game. The micro-server will automatically start hosting on port `9999`.
4. Open your browser and navigate to `http://127.0.0.1:9999` (this URL is automatically copied to your system clipboard on startup).
5. Scan the displayed QR code with your phone's official DG-Lab App to pair.

---

## Contributing
Since this project was developed without physical hardware, community testing and feedback are incredibly valuable! If you own a Coyote device and find any bugs, unexpected stimulation surges, or have telemetry optimization suggestions:
* Please open an **Issue** or submit a **Pull Request (PR)**.
* Live logs can be retrieved directly from the game's BepInEx console or the `/panel` web log viewer.

---

## License
Distributed under the **MIT License**. See `LICENSE` for more information.