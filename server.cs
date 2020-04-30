// Libraries and support functions
exec ("./lib/Support_Shapelines/server.cs");
exec ("./utility.cs");

// Constant values
exec ("./constants.cs");

// Configurable variables that can be changed by mods
exec ("./config.cs");

// Constructor + destructor
exec ("./createDestroy.cs");

// Rollercoaster pathcam node stuff
exec ("./nodes/createDestroy.cs");
exec ("./nodes/update.cs");
exec ("./nodes/debug.cs");

// Rollercoaster train stuff
exec ("./trains/createDestroy.cs");
exec ("./trains/camera.cs");
exec ("./trains/position.cs");
exec ("./trains/riders.cs");

// Rollercoaster brick stuff: datablocks, connection points, etc.
exec ("./bricks/datablocks.cs");
exec ("./bricks/plantRemove.cs");

// Miscellaneous stuff
exec ("./miscellaneous.cs");


$Rollercoaster::ClassesInitialized = true;
