# Project Outer Base - 2D Sci-fi Game
# Author/Developer: Le Duy An Tran
# About
A 2D sci-fi top down pixel game built on a self-designed planet renovation story line in which humanity will eventually fly to another planet for life after the Earth's destruction. Upon landing this liveable planet - the Oatis, they met another alien civilization living here. Even though it is a planet with civilization, however, the life quality here and the infrastructure of the cities here has not advanced to high-level technology as humanity. In order to stay alive, humanity had decided to live  and grow with the Oatisians, help them achieve higher life quality. The player will be part of the community that helps the Oatisian rebuild and improve the planet, farming, building business, amusement centers, or renovating and rebuilding cities, exporing new areas and adventure quests.
# Social Media
Showcases and work progress contents are posted on Instagram, please check out at: https://www.instagram.com/8thbitboi/ 
# Mechanics Features:
I mostly used arrays to store grid information and Unity Game Engine built-in Scriptable Objects to programming these features. More about Scriptable Objects: https://docs.unity3d.com/Manual/class-ScriptableObject.html. For the game complexity, and progressing, I used Singleton techniques. These are grid-based implementations: 
1. Grid-based Cursor Visuals: The cursor will get the position of the grid and return whether or not the grid is suitable to perform the actions that player is requesting (e.g. Drop Items/ Chop/ Break/ Hoe/ Water/ Plant/ Place Item). If the grid/position can be performed such actions return in green cursor interface, otherwise it is red cursor interface.
2. Farming: Player can chop down the trees gaining different amount of woods, and seeds, or break different types of rocks giving random amount of stones. Player can also dig/hoe the ground preparing for planting trees, bushes using seeds and watering them. Then whenever it is time for the fruit to be fully grown, they can be harvested.
3. Furnitures/Object Placing/Building: The placeable objects or funitures are able to be placed and picked up again for replacement. Tables, or fences can be connectable to create larger objects. The placeable objects or furnitures are only be dropped or placed whenever the grid is allow to drop/place. 
4. Dropping Items: Items will be stored as having several types - starting items, droppable items, placeable items, etc. Dropping system is built based on the grid information, getting information from the grid whether or not the grid is of type droppable. 
5. NPCs Path Finding: The NPCs' path finding is built based on the grid and calculation of AStar Algorithm. AStar algorithm is used to search and find the shortest path with a lowest cost optimizing the performance. In the implementations, based on the grid information (returns in NPC obstacles or not), the path will be calculated between the high and low cost distance grid. 
# GUI Features:
I used Aseprite to design graphics, visuals, and interfaces for the game then imported the components to Unity Game Engine for implementations. The techniques I used are enums, class references, and events caller. There are different user-interface features to show essential game information:
1. Game Time Clock and Date, Weather: The interface will show what is the current game time, dates, and weather to the player. Game time is designed based on ticks per second, and called by events. Time will be paused between scene transitions, saving, or loading.
2. Inventory Bar: Display a summary of the player's first 12 items in the inventory. It is implemented to be flexible in positioning - based on the player position, whether it is overlapping the player or not (player is going to the bottom edge of the screen where the inventory is located), it will position to the upper screen, improve on player visibility and experience. A small visual text at the corner of inventory bar will show the amount of the item. 
3. Item Description/ Dragging: Hover over the item will pop up a short item description about are the items duties, items types, and items name. Items can be dragged or dropped through the use of Unity's IDragHandler, IBeginDragHandler, IEndDragHandler, IPointerEnterHandler, and more.
4. Pause Menu: A menu show selections of game actions (e.g. Save/Load/Quit/Ordering Inventory Items). There will be buttons to be selected, and will perform each actions by calling methods according to appropriate actions.
# Other Features:
1. Save/Load: The system is implemented using interface class containing all necessary methods for saving and loading. Every objects, scripts, or components that are to be saved and loaded (e.g. inventory, NPCs/player's position, scenes, etc.) have to inherit the class. The saved objects are also to be generated with different uniques ID and through those IDs to retrieve the data to be loaded again. 
2. Quit: Simply quit the application without any errors.
# Installations:
The game demo build is available at: https://leduyantran.itch.io/project-outer-base-in-progress.
1. Download the zip file.
2. Run the .exe type file to play test the game.
3. Press ESC key to open the Pause Menu and quit the game.
# Instructions:
Player Movements: W, A, S, D keys or Up, Down, Right, Left keys.
Player Interactions: Left click to chop, break, place, harvest, select prompts. Space to interact with objects such as beds.
Game Dev Control: T to advance time, and G to skip day (sleep by interacting with bed can also skip day and save the game automatically).
Save/Load/Quit/Inventory: ESC key to open inventory tab or game tab that can choose between Save, Load, and Quit functions.
