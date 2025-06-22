Survival-Game-S4G
Description
Survival Game is a project developed for the S4G portfolio, where the player must collect coins while avoiding enemies within a limited time. The game features two scenes: a menu and a gameplay area, with unique mechanics and an interface. All scripts and Unity setup were created by me.
Features
Scenes:

Menu: Contains "Play Game" and "Exit" buttons. A scene with models is displayed in the background.
Game: The goal is to collect as many coins as possible while avoiding enemies within a set time.

Player Mechanics:

A stamina bar gradually fills; when full, a portion of stamina is consumed, granting a dash (up to 3 dashes).
Collecting coins restores health.

Enemies:

Deal physical damage and intimidation damage, increasing the player’s fear. When fear is maxed out, the player starts losing health.

Fear:

Decreases by 1 unit every 10 seconds if the player takes no damage.

Interface:

Icons for health, fear, and dashes on the HUD change based on the player’s stats.
Displays the number of collected coins and remaining time.
A settings button opens a panel with "Menu" and "Exit" options.

Events:

Upon death, a panel appears with options to respawn (losing 10 coins and increasing fear) or return to the menu.
At the end of the time limit, the number of collected coins is displayed along with a "Menu" button.

Technologies

Unity 6.1
C#
Animator for animations
UI System

How to Run

Download from Github: link
Open the project in Unity Hub.
Load the MainMenu scene from Assets/Scenes/.
Press Play or build for Windows/WebGL.



Contact

VSievolod Stupak | sevastupak090@gmail.com
