************************************
*        TERRAIN GRID SYSTEM       *
* (C) Copyright 2015-2016 Kronnect * 
*            README FILE           *
************************************


How to use this asset
---------------------
Firstly, you should run the Demo Scene provided to get an idea of the overall functionality.
Later, you should read the documentation and experiment with the tool.

Hint: to use the asset, drag the TerrainGridSystem prefab from Resources/Prefabs folder to your scene and assign your terrain to it.


Demo Scene
----------
There's one demo scene, located in "Demos" folder. Just go there from Unity, open "Demo1" scene and run it.


Documentation/API reference
---------------------------
The PDF is located in the Doc folder. It contains instructions on how to use the prefab and the API so you can control it from your code.


Support
-------
Please read the documentation PDF and browse/play with the demo scene and sample source code included before contacting us for support :-)

* Support: contact@kronnect.me
* Website-Forum: http://kronnect.me
* Twitter: @KronnectGames


Future updates
--------------

All our assets follow an incremental development process by which a few beta releases are published on our support forum (kronnect.com).
We encourage you to signup and engage our forum. The forum is the primary support and feature discussions medium.

Of course, all updates of Terrain Grid System will be eventually available on the Asset Store.


Version history
---------------

Version 4.4 Current Release

New Features:
- Added RespectOtherUI to prevent pointer interactions when it's over an UI element in front of the grid

Fixes:
- Fixed depth offset parameter not being applied correctly
- Fixed texture list empty in Editor
- Fixed Unity 5.5 compatibility issues

Version 4.3 16-NOV-2016

New Features:
- New events: OnTerritoryHighlight/OnCellHighlight with option to cancel highlight
- New territoryDisputedFrontiersColor property.
- Ability to set individual territory frontier color using TerritorySetFrontierColor
- Editor changes are now registered so Unity asks for changes to be saved

Version 4.2 26-SEP-2016

New Features:
- Ability to define territories by using a color texture

Improvements:
- Support for enclaves (territories surrounding other territories)
- Public API reorganization


Version 4.1 - 20-SEP-2016

Improvements:
- Ability to add color textures to define territories
- New demo scene #13

Version 4.0 - 10-SEP-2016

New Features:
  - A* Path Finding system. New demo scene 12.

Improvements:
  - Faster cells and territory line shaders
  - Added new properties to the inspector to control near clip fade amount and falloff
  - PathFinding now works with POT and non POT grid sizes
  
Fixes:
  - Fixed near clip fade effect with orthographic camera

Version 3.2 - 30-AUG-2016

Improvements:
  - Ability to add two or more grids to same terrain
  - New Elevation Base property to allow set higher heights to grid over terrain
  - Updated demo scene 6b showing how to get the row/column of the cell beneath a gameobject and fade neigbhours/range of cells
  
Fixes:
  - Minor fix to CellGetAtPoint method which returned null when positions passed crossed the terrain


Version 3.1 - 24-JUL-2016

Improvements:
  - New option in inspector to specify the minimum distance to camera for a cell to be selectable

Fixes:
  - Changed hexagonal topology so all rows contains same number of cells


Version 3.0 - 02-JUL-2016

New features:
  - New grid editor section with option to load configurations


Version 2.2 - 23-JUN-2016

New features:
  - New demo scene #11 showing how to transfer cells to another territory
  - Ability to hide territories outer borders
  
Improvements:
  - New API: CellSetTerritory.
  - Redraw() method now accepts a reuseTerrainData parameter to speed up updates if terrain has not changed

Fixes:
  - Fixed lower boundary of territory in hexagonal grid
  

Version 2.1 - 03-JUN-2016

New features:
  - New demo scene #10 showing how to position the grid inside the terrain using gridCenter and gridScale properties.
  
Improvements:
  - Cells will be visible if at least one vertex if visible when applying mask.
  - Canvas texture now works with territories also


Version 2.0 - 24-MAY-2016

New features:
  - Added mask property to define cells visibility.
  
Improvements:
  - CellGetAtPosition now can accept world space coordinates.
  - Option to prevent highlighting of invisible cells
  
Fixes:
  - Fixed bug in territory frontiers line shader
  

Version 1.4.0 - 12-MAY-2016

New features:
  - Added grid center and scale properties.

Fixes:
  - CellGetPosition and CellGetVertexPosition now takes into account terrain height

Version 1.3.2 - 15-APR-2016

Fixes:
  - Fixed compatibility with Orthographic Camera
  - Fixed CellMerge out of bounds error


Version 1.3.1 - 04-APR-2016

New features:
  - Grid configuration now supports specifying exact column and row number for box and hexagonal topologies
  - Added new Demo7 scene.

Improvements:
  - Added cellRowCount, cellColumnCount
  - Added CelGetAtPosition(column, row)
  - Added CellGetVertexCount, CellGetVertexPosition

Fixes:
  - Fixed CellGetCenterPosition in stand-alone mode


Version 1.2 - 04-JAN-2016

New features:
  - Added new Demo5 scene with cell fading example.
  - Can fade out cells and territories with a single function call.
  - Added new Demo6 scene with cell position and vertices locating example.
 
Improvements:
  - Some internal performance optimizations


Version 1.1 - 26-NOV-2015

New features:
  - Added new Demo3 and Demo4 scenes.
  - Can assign a canvas texture for all cells
  
Improvements:
  - Added new events: OnTerrainEnter, OnTerrainExit, OnTerrainClick, OnCellEnter, OnCellExit, OnCellClick.
  - Added cell visibility field.
  - Added CellSetTag and CellGetWithTag methods.
  - Cells can be customized with individual or canvas textures.
  

Version 1.0 - Initial launch 7/10/2015







