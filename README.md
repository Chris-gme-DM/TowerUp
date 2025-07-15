+++ Tower Up +++

+++ This is a school project, concentrated on developing features to interact with and to import and use objects that were created by our own, in blender +++

+++ Overview +++
Creator: Christof Kloninger
Genre: not yet defined, Jump'n'Run probably
Vision: Cyberpunk Setting, Exploration and Puzzle Solving, Parcours, First Person Perspective
Goals: Fun experience in exploration and figuring out the controls
Contributors: Music by Jeremy Froböse (Available in the asset store), other people that helped
Supporting programs: Adobe Substance Painter, blender, Gemini AI
Sources: blender, Adobe Substance Painter 3D, Unity Asset Store, Adobe Substance Painter Asset Store


+++ General +++
The vision was a Cyberpunk-esque setting in which the player is supposed to navigate through a utility facility and find their way to the top of the tower to escape. (So far this is not yet achieved but I think I will continue on the project)
I found that I like to develop Physics related features, e.g. Player Controls and State Machine to work with different states and manipulate player movement in engaging patterns.

Dark, bleak Industrial Site. Depressing environment. The player should want to escape that place. So far the project lacks in that regard. As it stands now, the player has to find the switches to enable Interactions with the doors. 
Every switch enables/disables one door. She needs to find credit Chips as well, since gaining access to the door require certain amounts of credits for each door. Cyberpunk, dystopian capitalism running rampant.

In regard to the school assignment my interactions are WallRun, WallClimb, Switch to enable the door, the doors themselves, CreditChips the player needs to collect.
The objects i created are: Doors, Switches, basice wall ( which i don't count to the required 5 objects), Storage Box, Credit Chips, Ladder. The Gangway isn't implemented since the baking process and the way the controls turned out, made an implementation not feasable for the time being.

I used public fields in many places of the script in which it is either detremental or obsolete, but I will deal with this in the future. 

+++ Player +++
First Person Perspective
Controls were set via Input Action System in unity6. So far full control is restricted to keyboard, but Controller settings should be added in the future.
There is no Tutorial (so far, since I don't like to hold players hands, but I admit that the controls are tricky to get used to)

++ Controls ++
 + WASD - Move
 + Look - Mouse
 + SPACE - Jump                    Jump from the ground, or from the wall, if player is running or climbing along the wall
 + SHIFT - Climb                   This can initate WallRun and WallClimb, depending on how the player looks at the wall. Since I used angles to differentiate between the possible States, this requires some playing around
                                Look more or less directly at a wall to climb, to wallrun look to either side, depending which side you want to run along
 + E - Interact                    To initiate Interaction the player needs to look at the object and come into range with interactable objects.

There is an invisible Stamina System for the player to limit the use of the Wallrun and Climb features. these Interactions require stamina, which is regenerated in every other state the player can be in.


+++ Troubleshoot +++ (for my teachers)

++ Hierarchy ++
If anything went wrong with the import of the project: My hierarchy should be listed in the GameScene as follows:
===== Administrative =====
+ objects like player and several managers +
+ ===== UI =====
+ UIManager
+ Canvas
+ ===== Level =====
+ objects for level restrictions or lighting
+ ==== Door/Switch ====
+ Door and Switch Pairings 
+ ==== Misc ==== Miscellaneous
+ CreditChips 
+ StorageBoxes 
+ etc. 
+ ==== Floor/Structures ====
I hope i manage this part more carefully and add Layers of object desriptions, the current hierarchy is honestly a mess
+ BaseFloor + Thes are just the base floor tile arrays.
+ Floor + Base Floor objects
  + floor objects 
+ Floor(1) 
  + floor objects 
... I think You see where i am going with this. I thought about inner structuring of these layers, like general walls and floor tiles. A pattern that came to mind was to creat Parcours Patterns of walls and other objects, releveant to a certain pattern of obstacles.
    To reuse these patterns in other parts of the project, could be made into prefabs.
==== OuterWalls ====
+ wall structures to pen the player in 

++ Possible Adjustments ++

+ Objects +
If any objects are above "Administrative" that hints to my Editor Array Modifier doing something unexpected. That can result if any objects with said "ArrayModifier" Script persist in the temp files of the project upon closing the application. 
Identify these objects and remove the script component. If they are obviously redundant, delete/deactivate them.

+ Player Settings +
Two objects bear the mainload of information that set the player controls: Player > PlayerController and StateMachine > StateController
PlayerController: Here you can adjust all the forces that impact player movement, such as max movespeed, acceleration, jump force, etc.
If i forgot to set any default values, play around with the given Ranges.
State Controller: That's an entirely different beast, please look into the script if any values are not set.
In general I add comments in these scripts and update this document to advisable values.
If, for any reason, the "Climb" Action is not on InputSystem_Action Map. Go to Assets >> InputSystem_Actions[ActionMap] and add "Climb", set SHIFT as the button and add no interactions in the button settings

+ Layer Masks +
The LayerMasks "Ground", "Wall", "Interactable", "Scalable", "Obstacle" are sometimes crucial to the functions. Walls are walls, i hope i don't need to describe what that is, Interactbales are CreditChips, Doors, Switches, so far
Scalable are Ladders and StorageBoxes, 

+++ Design +++
Most objects, so far, were created in blender, to some i created custom textures in Substance Painter.

+++ Production +++
I identified the assignment and made a plan. To ignore my plan and incrementally work on single feautres of the game. 

++ Priorities ++
+ Player Controls + I created those first, since i had a Parcours game in mind I thought primarily on the implementation of engaging Character controls and movements for the player's enjoyment.
+ State Machine + I created a state machine that evaluates the player's circumstances and inputs constatnly and controls the state the player inhabits. It reads transforms and inputs by the player and calls the predetermined State classes. The State class itself is abstract
  Each State Script manipulates the player in the desired way and the implementation of new states is scalable at will. (Adjustments needed in PlayerController, StateController and new StateScript). It holds a lot of responsibilites, which i will think about if
  I should outsource some of the workload, but i haven't encountered errors or performance issues so far
+ InteractionManager + Was at one point a functional Collector of possible Interactable and scaled to a point in which thinks like "Hack through certain walls", "Grappling Hook Points" , [insert many ideas]... but i wrongfully accused this to be the source of my
  Interaction problem and reduced my work to ashes and implemented the easiest form of the interaction manager i can think of. Interactables are ScriptableObjects now. Boring. Sorry, I needed this.
+ Objects + Spent days to work out the objects in blender, unwrapped UV-Maps, exported those into the project and Substance Painter. I don't think i used any special techniques for this.
+ Leveldesign + A bit late, and it shows. I was exhausted and frankly out of my comfort zone. I had several sections of any given Level in mind.
  Since i needed  a lot of objects of the same prefab, i scripted an Editor Script "ArrayModifier" with the help of Gemini. The way this works: you create a new empty object on the floor you want it to originate, since the floors give a base height to all objects   on the given floor. Place the origin in the position you want objects to originate from. Add the ArrayModifier Script. Place a prefab or object you want to array in the slot. adjust the settings until you are satisfied with the array. Remove the ArrayModifier     Script to "apply" the Modifier. Do not use the arrayableObejects prefab.

+++ Thanks for playing the Demo and let me know what you think of it +++
