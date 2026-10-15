# Rolling Line Signal Passwords Mod

A BepInEx code mod for **Rolling Line** that adds password triggering function to vanilla signal objects.

## Features

- **Signal Passwords** — Configureable password for individual signal objects. Password will be triggered on the signal detecting any rollingstock in the range of the red or yellow. Additional vehicles do not trigger the password again. Next password trigger will happen when the signal detection range dont have any vehicles in it.
- **In-Game Configuration** — Set or edit the password through the signal's configuration UI. Empty field do not call the password trigger function. 
- **Save Support** — Passwords are saved with the signal data and restored when the map is loaded. ***Important:** Spaces are not allowed in the passwords! It breaks the save file parsing, resulting in only storing the password until the first space.*
- **Mod Integration** — Designed to work with the existing signal system and password-based prop logic. **Multiplayer functionality is not tested at this point.**

## Requirements

- Rolling Line
- BepInEx

## Installation

1. Download the latest version from the GitHub page.
2. Download and install the appropriate [BepInEx](https://github.com//BepInEx/BepInEx/releases) 5.4.23.5 or greater. (Earlier version functionality was not tested.)
3. Extract the downloaded mod files into your Rolling Line/BepInEx directory.
4. Ensure the mod's files are placed in the appropriate `BepInEx/plugins/` and `BepInEx/patchers/` folders.
5. Launch the game.

## Download

**[RL_SignalPasswords_1.0.0.zip](https://github.com/Ash9425/RL.SignalPasswords/releases/download/Published/RL_SignalPasswords_1.0.0.zip)**

## Source Code

Currently the source code is not available in the repository, but will be shared in the future.

## Compatibility

Compatibility with future Rolling Line updates is not guaranteed.
Changes to the signal system may distrupt functionality.
At the point of publishing, loss of functionality or the removal of the mod from a working save would only result in the loss of signal passwords in the save files without corruption.
Using parallel saves are recommended.

## Disclaimer

This is a community-made mod and is not affiliated with the developers of Rolling Line.
