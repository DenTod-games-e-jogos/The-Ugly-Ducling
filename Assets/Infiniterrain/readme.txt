InfiniTerrain Overview:
InfiniTerrain (IT for short) seeks to help developers in two ways.  

First to generate nice looking random terrains.  There are multiple approaches to random terrain generation and IT uses the "diamond-square" approach.  The terrain can be generated at runtime and use immediately.  Alternately IT can output the newly generated terrain to a file to be used later.

Second, IT helps developers render large terrains efficiently.  A simple -- but inefficient -- approach to representing a terrain in Unity would be to use a grid of blocks which are each a game object.  This approach is quick to code, but as the size of the terrain grows, this method quickly runs into performance issues.  Generating just a few thousand blocks can quickly bring the rendering speed to a crawl.  This happens for two reasons.  1) Each block is a Unity game object and each of those objects has associated overhead.  In otherwords they are not "free."  2) each block is contains a small mesh.  Generating thousands of objects means thousands of small meshes that are sent to the video card which is not efficient.  Worlds can contain many millions of blocks (or tiles).  Using the cube/GameObject method to respresent millions of blocks does not scale.

An alternate approach is to combine each of the cubes/GameObjects into a much larger mesh.  Thus instead of representing a 100x100 map grid with 100x100=10,000 blocks we can represent it as a single mesh.  This has the advantage of reducing the number of unity game objects from 10,000 down to 1 and it also only sends 1 large mesh to the video card for processing instead of many small ones.  The results in *significantly* better performance and scaling.

This is what the associated Heightmap and Voxel renders do.  They convert the map data generated from the terrain generation script and build renderers using large meshes instead of individual blocks.  The result are fast, memory efficient renderers that can render extremely large terrains while maintaining excellent performance!

Important notes:
This version of InfiniTerrain was build and packaged with Unity version 5.1.2f1.  Other versions of Unity could differ in options and menus.

Package contents Overview:
The package is composed of a number of scripts located in the /scripts directory of the project:

-TerrainGenerator.cs  This can be used as a stand alone script to generate random terrain and save it to a binary file.  Conversely the script can be used in conjunction with either one of the renderers to visualize the world.

-HeightmapRenderer.cs This file is used to convert the raw terrain data into a visual world using a heightmap.  It can import the terrain data directly after running the terrain generator or load a previously saved terrain from disk.

-HeightmapController.cs This file is to interact (and modify) a heightmap world using a standard Unity firstperson controller game object.

-VoxelRenderer.cs This file is used to convert the raw terrain data into a visual world using voxel terrain.  It can import the terrain data directly after running the terrain generator or load a previously saved terrain from disk.

-VoxelController.cs This file is to interact (and modify) a voxel world using a standard Unity firstperson controller game object.

-Block.cs is a helper struct for storing block info for the voxel renderer.

The package also include two scenes in the /scenes directory of the project:

-HeightmapScene.unity this scene loads the assets to generate a heightmap world.  

-VoxelScene.unity this scene loads the assets to generate a voxel (block) world.

How to build the project using the included scenes:

-Create a new, empty project in Unity.  You can do this by selecting the menu File->New Project while in Unity.  You can then go to the menu Assets-> Import Package -> Custom Package... and then browse to the InfiniTerrain.unitypackage file to import InfiniTerrain into your project.  Next browse to Assets->Scenes in your project folder in Unity and double click on either the HeightmapScene or VoxelScene to load that scene.  You can then click play to generate a new terrain, move around in the world, and modify the terrain.

In game controls:

In both the HeightmapScene and VoxelScene you can move around in the world using the standard unity first person controller.  You can use the WASD and/or arrow keys for movement, and move the mouse for mouselook.  In addition clicking the left mouse button on the terrain in the HeightmapScene will lower the nearest terrain vertex.  Clicking the right mouse button will raise it.  Similarly, clicking the left mouse button in the VoxelScene will delete the block you clicked on.  Right clicking on a block face will create a new block adjacent to that one.

Other Notes:
-To see how fast the renderer is capable of running you need to disable vsync in your project.  Otherwise your project will be locked to the screen refresh rate which is usually 60fps.  You can do this by going to the menu Edit->Project Settings->Quality and setting "V Sync Count" to "Don't Sync" near the bottom of the list.

Version history:
1.0.0 Initial Release
