// Libraries and support functions
exec ("./lib/Support_Shapelines/server.cs");
exec ("./utility.cs");

// Constructor + destructor
exec ("./createDestroy.cs");

// Track camera nodes
exec ("./nodes.cs");

// Rollercoaster train stuff
exec ("./trains/createDestroy.cs");
exec ("./trains/camera.cs");
exec ("./trains/miscellaneous.cs");

// Debug stuff like drawing shapelines
exec ("./debug.cs");

$Rollercoaster::ClassesInitialized = true;
