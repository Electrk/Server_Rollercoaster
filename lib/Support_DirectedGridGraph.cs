// ----------------------------------------------------------------------
//  Title:   DirectedGridGraph (Super Class)
//  Author:  Electrk
//  Version: 1
//  Updated: March 6th, 2019
// ----------------------------------------------------------------------
//  Adds a grid-based directed graph super class for ScriptObjects.
// ----------------------------------------------------------------------
//  Include this code in your own scripts as an *individual file*
//  called "Support_DirectedGridGraph.cs".  Do not modify this code.
// ----------------------------------------------------------------------
//  Dependencies:
//    + Support_Shapelines
// ----------------------------------------------------------------------
//  Example Usage:
//
//    %graph = new ScriptObject ()
//    {
//    	superClass = DirectedGridGraph;
//    	class      = MyGraphClass;
//
//      ...
//
//    	< Other properties - See the onAdd method >
//    };
//
//    %graph.addEdge (0, 0, 1, 1);
//    %graph.removeEdge (0, 0, 1, 1);
//
// ----------------------------------------------------------------------
//  Notes:
//    + If you need to add your own onAdd or onRemove method, please,
//      please, please remember to call the parent.
//
//    + If a method name is prefixed with an _underscore, that means
//      don't use it.
// ----------------------------------------------------------------------


if ( !isObject (C_SquareShape)  ||  !isFunction (drawLine) )
{
	error ("DirectedGridGraph requires Support_Shapelines!");
	return;
}

if ( $DirectedGridGraph::Version >= 1 )
{
	return;
}

$DirectedGridGraph::Version = 1;

// So you chucklefucks don't screw up the namespace parent linkage.
new ScriptObject () { superClass = DirectedGridGraph; }.delete ();


// ------------------------------------------------


$DirectedGridGraph::Default::GRAPH_WIDTH  = 16;
$DirectedGridGraph::Default::GRAPH_HEIGHT = 16;

$DirectedGridGraph::Default::DEBUG_SERVER_MESSAGES = false;
$DirectedGridGraph::Default::DEBUG_SHOW_LINES      = false;
$DirectedGridGraph::Default::DEBUG_LINE_SIZE       = 0.1;
$DirectedGridGraph::Default::DEBUG_POSITION_X      = 0;
$DirectedGridGraph::Default::DEBUG_POSITION_Y      = 0;
$DirectedGridGraph::Default::DEBUG_POSITION_Z      = 0.1;
$DirectedGridGraph::Default::DEBUG_EDGE_COLOR      = "1 0 0 0.5";
$DirectedGridGraph::Default::DEBUG_VERTEX_COLOR    = "1 1 0 0.5";


// Please, please, please remember to call Parent::onAdd if you're inheriting from 
// this super class.

function DirectedGridGraph::onAdd ( %this, %obj )
{
	if ( %this.width $= "" )
	{
		%this.width = $DirectedGridGraph::Default::GRAPH_WIDTH;
	}

	if ( %this.height $= "" )
	{
		%this.height = $DirectedGridGraph::Default::GRAPH_HEIGHT;
	}


	// Whether or not we want debug messages to be show via the talk () function.
	// Error messages will be printed regardless of what this is set to -- this just
	// determines whether or not they'll be broadcasted to the whole server.

	if ( %this.debugServerMessages $= "" )
	{
		%this.debugServerMessages = $DirectedGridGraph::Default::DEBUG_SERVER_MESSAGES;
	}


	// Whether or not debug graph graphics will be drawn.  Do not set this directly.
	// Use drawAllDebugLines () to enable them.
	// Use eraseAllDebugLines () to disable them.

	if ( %this.debugShowLines $= "" )
	{
		%this.debugShowLines = $DirectedGridGraph::Default::DEBUG_SHOW_LINES;
	}

	if ( %this.debugLineSize $= "" )
	{
		%this.debugLineSize = $DirectedGridGraph::Default::DEBUG_LINE_SIZE;
	}


	// The position where the debug graphics will be rendered in the world.
	// Do not set these directly.  Use setDebugPosition () to change them.

	if ( %this.debugPositionX $= "" )
	{
		%this.debugPositionX = $DirectedGridGraph::Default::DEBUG_POSITION_X;
	}

	if ( %this.debugPositionY $= "" )
	{
		%this.debugPositionY = $DirectedGridGraph::Default::DEBUG_POSITION_Y;
	}

	if ( %this.debugPositionZ $= "" )
	{
		%this.debugPositionZ = $DirectedGridGraph::Default::DEBUG_POSITION_Z;
	}


	// The debug graphics edge/vertex colors.  Do not set these directly.
	// Use setDebugColors () to change them.

	if ( %this.debugEdgeColor $= "" )
	{
		%this.debugEdgeColor = $DirectedGridGraph::Default::DEBUG_EDGE_COLOR;
	}

	if ( %this.debugVertexColor $= "" )
	{
		%this.debugVertexColor = $DirectedGridGraph::Default::DEBUG_VERTEX_COLOR;
	}


	// Extra checks in case someone messed with the default values.

	if ( %this.width < 2 )
	{
		%this.width = 2;
	}

	if ( %this.height < 2 )
	{
		%this.height = 2;
	}


	%this.debugObjects = new SimSet ();

	if ( %this.debugShowLines )
	{
		%this.drawAllDebugLines ();
	}
}


// Please, please, please remember to call Parent::onRemove if you're inheriting from 
// this super class.

function DirectedGridGraph::onRemove ( %this, %obj )
{
	%this.eraseAllDebugLines ();
	%this.debugObjects.delete ();
}


// ------------------------------------------------


// Adds a directed edge from a vertex (tail) to another vertex (head).
//
// @param    int fromX    The X coordinate of the vertex we're creating the edge from.
// @param    int fromY    The Y coordinate of the vertex we're creating the edge from.
// @param    int toX      The X coordinate of the vertex we're creating the edge to.
// @param    int toY      The Y coordinate of the vertex we're creating the edge to.
//
// @return   Each vertex maintains a field list of other vertices it's connected to.
//           If the edge was successfully added, or if the edge already exists, this
//           method returns the index of the head in the tail's connections list.
//           If unsuccessful, it returns an empty string.


function DirectedGridGraph::addEdge ( %this, %fromX, %fromY, %toX, %toY )
{
	if ( !%this.isValidEdge (%fromX, %fromY, %toX, %toY) )
	{
		%msg = "Invalid edge from " @ %fromX @ ", " @ %fromY @ " to " @ %toX @ ", " @ %toY;
		%this._printFunctionError ("addEdge", %msg);
		return "";
	}

	if ( %fromX == %toX  &&  %fromY == %toY )
	{
		%msg = "Same coords from " @ %fromX @ ", " @ %fromY @ " to " @ %toX @ ", " @ %toY;
		%this._printFunctionError ("addEdge", %msg);
		return "";
	}

	if ( %this.connectionIndex_[%fromX, %fromY, %toX, %toY] !$= "" )
	{
		return %this.connectionIndex_[%fromX, %fromY, %toX, %toY];
	}

	%toPos = %toX SPC %toY;

	if ( %this.connectedTo_[%fromX, %fromY] $= "" )
	{
		%this.connectedTo_[%fromX, %fromY] = %toPos;
	}
	else
	{
		%this.connectedTo_[%fromX, %fromY] = %this.connectedTo_[%fromX, %fromY] TAB %toPos;
	}

	// Store index for quick lookup so we don't have to loop through "connectedTo"

	%index = getFieldCount (%this.connectedTo_[%fromX, %fromY]) - 1;
	%this.connectionIndex_[%fromX, %fromY, %toX, %toY] = %index;

	%this._drawDebugEdge (%fromX, %fromY, %toX, %toY);

	return %index;
}


// Removes a directed edge from a vertex (tail) to another vertex (head).
//
// @param    int fromX    The X coordinate of the tail vertex.
// @param    int fromY    The Y coordinate of the tail vertex.
// @param    int toX      The X coordinate of the head vertex.
// @param    int toY      The Y coordinate of the head vertex.
//
// @return   true if an edge was removed, false if not.


function DirectedGridGraph::removeEdge ( %this, %fromX, %fromY, %toX, %toY )
{
	// We don't do an isValidEdge/isValidVertex check because we still want to remove
	// any invalid edges that might exist.

	if ( !%this._isCoordinateValue (%fromX, %fromY)  ||  !%this._isCoordinateValue (%toX, %toY) )
	{
		%msg = "Invalid edge from " @ %fromX @ ", " @ %fromY @ " to " @ %toX @ ", " @ %toY;
		%this._printFunctionError ("removeEdge", %msg);
		return false;
	}

	%edges = %this.connectedTo_[%fromX, %fromY];
	%index = %this.connectionIndex_[%fromX, %fromY, %toX, %toY];

	%toPos = %toX SPC %toY;

	// If index is blank, loop through `connectedTo` just in case the `connectionIndex`
	// somehow managed to get cleared, and remove the connection.

	if ( %index $= ""  &&  %edges !$= "" )
	{
		%count = getFieldCount (%edges);

		for ( %i = 0;  %i < %count;  %i++ )
		{
			if ( getField (%edges, %i) $= %toPos )
			{
				%msg = "Missing index from " @ %fromX @ ", " @ %fromY @ " to " @ %toX @ ", " @ %toY;
				%this._printFunctionError ("removeEdge", %msg);

				%index = %i;
				break;
			}
		}
	}

	if ( %index !$= ""  &&  %edges !$= ""  &&  getField (%edges, %index) $= %toPos )
	{
		%edges = removeField (%edges, %index);

		%this.connectedTo_[%fromX, %fromY] = %edges;
		%this.connectionIndex_[%fromX, %fromY, %toX, %toY] = "";


		// Reindex everything after our removed edge.

		%count = getFieldCount (%edges);

		for ( %i = %index;  %i < %count;  %i++ )
		{
			%conn  = getField (%edges, %i);
			%connX = getWord (%conn, 0);
			%connY = getWord (%conn, 1);

			%this.connectionIndex_[%fromX, %fromY, %connX, %connY] = %i;
		}

		%this._eraseDebugEdge (%fromX, %fromY, %toX, %toY);

		return true;
	}

	return false;
}


// Adds two directed edges between two vertices.
//
// @param    int firstX     The X coordinate of the first vertex.
// @param    int firstY     The Y coordinate of the first vertex.
// @param    int secondX    The X coordinate of the second vertex.
// @param    int secondY    The Y coordinate of the second vertex.
//
// @return   Each vertex maintains a field list of other vertices it's connected to.
//           If the edges were successfully added, this method returns the indices
//           of both vertices in the other's connections list.
//           If unsuccessful, it returns an empty string.


function DirectedGridGraph::addTwoWayEdge ( %this, %firstX, %firstY, %secondX, %secondY )
{
	%firstIndex = %this.addEdge (%firstX, %firstY, %secondX, %secondY);

	if ( %firstIndex $= "" )
	{
		return "";
	}

	%secondIndex = %this.addEdge (%secondX, %secondY, %firstX, %firstY);

	if ( %secondIndex $= "" )
	{
		// This should never happen...

		%edge = %firstX @ ", " @ %firstY @ " to " @ %secondX @ ", " @ %secondY;
		%msg  = "Edge from " @ %edge @ " is valid, but somehow the other way is not!";

		%this._printFunctionError ("addTwoWayEdge", %msg);

		return "";
	}

	return %firstIndex SPC %secondIndex;
}


// Removes two directed edges between two vertices.
//
// @param    int firstX     The X coordinate of the first vertex.
// @param    int firstY     The Y coordinate of the first vertex.
// @param    int secondX    The X coordinate of the second vertex.
// @param    int secondY    The Y coordinate of the second vertex.
//
// @return   The number of edges that were removed.


function DirectedGridGraph::removeTwoWayEdge ( %this, %firstX, %firstY, %secondX, %secondY )
{
	%edgesRemoved = 0;

	if ( %this.removeEdge (%firstX, %firstY, %secondX, %secondY) )
	{
		%edgesRemoved++;
	}

	if ( %this.removeEdge (%secondX, %secondY, %firstX, %firstY) )
	{
		%edgesRemoved++;
	}

	return %edgesRemoved;
}


// Removes all edges from a vertex, optionally in both directions.
//
// @param    int fromX                      The X coordinate of the vertex.
// @param    int fromY                      The Y coordinate of the vertex.
// @param    bool [bothDirections=false]    If true, for each edge we remove, we will
//                                          remove the edge going the other way too.
//
// @return   The number of edges that were removed.


function DirectedGridGraph::removeEdgesFrom ( %this, %fromX, %fromY, %bothDirections )
{
	if ( %bothDirections $= "" )
	{
		%bothDirections = false;
	}

	%edges = %this.connectedTo_[%fromX, %fromY];
	%count = getFieldCount (%edges);

	if ( %count <= 0 )
	{
		return 0;
	}

	%edgesRemoved = %count;

	for ( %c = 0;  %c < %count;  %c++ )
	{
		%edge = getField (%edges, %c);
		%toX  = getWord (%edge, 0);
		%toY  = getWord (%edge, 1);

		%this.connectionIndex_[%fromX, %fromY, %toX, %toY] = "";
		%this._eraseDebugEdge (%fromX, %fromY, %toX, %toY);

		// Remove edge going the other way, if we want to do that.

		if ( %bothDirections  &&  %this.removeEdge (%toX, %toY, %fromX, %fromY) )
		{
			%edgesRemoved++;
		}
	}

	%this.connectedTo_[%fromX, %fromY] = "";

	return %edgesRemoved;
}

function DirectedGridGraph::removeAllEdges ( %this )
{
	%edgesRemoved = 0;

	%width  = %this.width;
	%height = %this.height;

	for ( %x = 0;  %x < %width;  %x++ )
	{
		for ( %y = 0;  %y < %height;  %y++ )
		{
			%edgesRemoved += %this.removeEdgesFrom (%x, %y);
		}
	}

	return %edgesRemoved;
}


// Checks if there is a directed edge from one vertex (tail) to another (head).
//
// @param    int fromX    The X coordinate of the tail vertex.
// @param    int fromY    The Y coordinate of the tail vertex.
// @param    int toX      The X coordinate of the head vertex.
// @param    int toY      The Y coordinate of the head vertex.
//
// @return   true if there is, false if there isn't.


function DirectedGridGraph::hasEdge ( %this, %fromX, %fromY, %toX, %toY )
{
	if ( %this.isEdgeOutOfBounds (%fromX, %fromY, %toX, %toY) )
	{
		return false;
	}

	return %this.connectionIndex_[%fromX, %fromY, %toX, %toY] !$= "";
}


// Returns the field list of edges from a vertex.
//
// @param    int fromX    The X coordinate the vertex.
// @param    int fromY    The Y coordinate the vertex.
//
// @return   A field list of connections from the vertex and an empty string if there
//           aren't any or if the vertex is invalid.


function DirectedGridGraph::edgesFrom ( %this, %x, %y )
{
	if ( %this.isVertexOutOfBounds (%x, %y)  ||  !%this.isValidVertex (%x, %y) )
	{
		return "";
	}

	return %this.connectedTo_[%x, %y];
}

function DirectedGridGraph::isVertexOutOfBounds ( %this, %x, %y )
{
	return %x < 0  ||  %x >= %this.width  ||  %y < 0  ||  %y >= %this.height;
}

function DirectedGridGraph::isEdgeOutOfBounds ( %this, %fromX, %fromY, %toX, %toY )
{
	return %this.isVertexOutOfBounds (%fromX, %fromY)  ||  %this.isVertexOutOfBounds (%toX, %toY);
}

function DirectedGridGraph::isValidVertex ( %this, %x, %y )
{
	return %this._isCoordinateValue (%x, %y)  &&  !%this.isVertexOutOfBounds (%x, %y);
}

function DirectedGridGraph::isValidEdge ( %this, %fromX, %fromY, %toX, %toY )
{
	return %this.isValidVertex (%fromX, %fromY)  &&  %this.isValidVertex (%toX, %toY);
}


///////////////////////
// "Private" Methods //
///////////////////////


function DirectedGridGraph::_isCoordinateValue ( %this, %x, %y )
{
	return %x !$= ""  &&  %y !$= ""  &&  %this._isInt (%x)  &&  %this._isInt (%y);
}

// Don't want to require some other random function but also don't want to trample over
// any existing isInt implementations...

function DirectedGridGraph::_isInt ( %this, %value )
{
	return %value $= (%value << 0);
}


// ------------------------------------------------

///////////
// Debug //
///////////


function DirectedGridGraph::showServerErrorMessages ( %this )
{
	%this.debugServerMessages = true;
}

function DirectedGridGraph::hideServerErrorMessages ( %this )
{
	%this.debugServerMessages = false;
}


// Use this method to enable and refresh debug graphics.
// This creates a static shape grid with vertices represented by yellow dots (by default)
// and edges represented by red lines (by default).
//
// Since it's Blockland and since it's static shapes, this can get very
// laggy very quickly, so use it wisely.
//
// Only works on servers, obviously.


function DirectedGridGraph::drawAllDebugLines ( %this )
{
	// Obviously we'll only want to draw debug lines if we're hosting a server, so
	// check that we are first.

	if ( $Server::ServerType $= ""  ||  !isObject (MissionCleanup) )
	{
		return;
	}

	%this.eraseAllDebugLines ();
	%this.debugShowLines = true;

	%width  = %this.width;
	%height = %this.height;

	for ( %x = 0;  %x < %width;  %x++ )
	{
		for ( %y = 0;  %y < %height;  %y++ )
		{
			%this._drawDebugVertex (%x, %y);

			%edges = %this.connectedTo_[%x, %y];
			%count = getFieldCount (%edges);

			if ( %count <= 0 )
			{
				continue;
			}

			for ( %c = 0;  %c < %count;  %c++ )
			{
				%edge = getField (%edges, %c);
				%toX  = getWord (%edge, 0);
				%toY  = getWord (%edge, 1);

				%this._drawDebugEdge (%x, %y, %toX, %toY);
			}
		}
	}
}


// Use this to disable debug graphics.
// Only works on servers, obviously.

function DirectedGridGraph::eraseAllDebugLines ( %this )
{
	if ( $Server::ServerType $= ""  ||  !isObject (MissionCleanup) )
	{
		return;
	}

	%width  = %this.width;
	%height = %this.height;

	for ( %x = 0;  %x < %width;  %x++ )
	{
		for ( %y = 0;  %y < %height;  %y++ )
		{
			%this._eraseDebugVertex (%x, %y);

			%edges = %this.connectedTo_[%x, %y];
			%count = getFieldCount (%edges);

			if ( %count <= 0 )
			{
				continue;
			}

			for ( %c = 0;  %c < %count;  %c++ )
			{
				%edge = getField (%edges, %c);
				%toX  = getWord (%edge, 0);
				%toY  = getWord (%edge, 1);

				%this._eraseDebugEdge (%x, %y, %toX, %toY);
			}
		}
	}

	%this.debugObjects.deleteAll ();  // Delete any remaining lines we may have missed
	%this.debugShowLines = false;
}


// Use this to change the edge and vertex thickness.
// The spacing between vertices scales with the thickness so don't worry about that.

function DirectedGridGraph::setDebugLineSize ( %this, %size )
{
	if ( %size < 0.05 )
	{
		%size = 0.05;
	}

	if ( %size > 10 )
	{
		%size = 10;
	}

	%this.debugLineSize = %size;

	if ( %this.debugShowLines )
	{
		%this.drawAllDebugLines ();
	}
}


// Use this to change the edge and vertex colors.
//
// @param    decRGB [edgeColor=""]      A decimal (0-1) RGB(A) value that determines
//                                      the color of the edges for debug graphics.
//                                      Leave this blank if you just want to change 
//                                      the vertex color.
//
// @param    decRGB [vertexColor=""]    A decimal (0-1) RGB(A) value that determines
//                                      the color of the vertices for debug graphics.
//                                      Leave this blank if you just want to change 
//                                      the edge color.


function DirectedGridGraph::setDebugColors ( %this, %edgeColor, %vertexColor )
{
	%edgeColor = %this._getDebugColor (%edgeColor);

	if ( %edgeColor !$= "" )
	{
		%this.debugEdgeColor = %edgeColor;
	}

	%vertexColor = %this._getDebugColor (%vertexColor);

	if ( %vertexColor !$= "" )
	{
		%this.debugVertexColor = %vertexColor;
	}

	if ( %this.debugShowLines )
	{
		%this.drawAllDebugLines ();
	}
}


// Pretty self-explanatory.  Use this to change the position of the debug graphics.

function DirectedGridGraph::setDebugPosition ( %this, %position )
{
	%x = getWord (%position, 0);
	%y = getWord (%position, 1);
	%z = getWord (%position, 2);

	if ( %x $= "" )
	{
		if ( %this.debugPositionX $= "" )
		{
			%x = $DirectedGridGraph::Default::DEBUG_POSITION_X;
		}
		else
		{
			%x = %this.debugPositionX;
		}
	}

	if ( %y $= "" )
	{
		if ( %this.debugPositionY $= "" )
		{
			%y = $DirectedGridGraph::Default::DEBUG_POSITION_Y;
		}
		else
		{
			%y = %this.debugPositionX;
		}
	}

	if ( %z $= "" )
	{
		if ( %this.debugPositionZ $= "" )
		{
			%z = $DirectedGridGraph::Default::DEBUG_POSITION_Z;
		}
		else
		{
			%z = %this.debugPositionZ;
		}
	}

	%this.debugPositionX = %x;
	%this.debugPositionY = %y;
	%this.debugPositionZ = %z;

	if ( %this.debugShowLines )
	{
		%this.drawAllDebugLines ();
	}
}

function DirectedGridGraph::getDebugPosition ( %this )
{
	return %this.debugPositionX SPC %this.debugPositionY SPC %this.debugPositionZ;
}


///////////////////////
// "Private" Methods //
///////////////////////


// Don't use these.  For real.


function DirectedGridGraph::_printError ( %this, %message )
{
	error (%message);

	if ( %this.debugServerMessages  &&  %this._isHostingServer () )
	{
		talk (%message);
	}
}

function DirectedGridGraph::_printFunctionError ( %this, %function, %message )
{
	%this._printError (%function @ " () - " @ %message);
}

function DirectedGridGraph::_missingDebugObjectsError ( %this )
{
	%msg = "`debugObjects` missing!  Remember to call Parent::onAdd and Parent::onRemove!";
	%this._printError (%msg);
}

function DirectedGridGraph::_isHostingServer ( %this )
{
	return $Server::ServerType !$= ""  &&  isObject (MissionCleanup);
}

function DirectedGridGraph::_getDebugDrawPos ( %this, %x, %y, %offsetX, %offsetY )
{
	%multiplier = %this.debugLineSize * 10;

	%x = -((%x * %multiplier) + %offsetX);
	%y = (%y * %multiplier) + %offsetY;

	return %x + %this.debugPositionX SPC %y + %this.debugPositionY SPC %this.debugPositionZ;
}

function DirectedGridGraph::_getDebugColor ( %this, %color )
{
	%color = trim (%color);

	if ( getWordCount (%color) == 3 )
	{
		%color = %color SPC 0.5;
	}

	if ( %color $= ""  ||  getWordCount (%color) != 4 )
	{
		return "";
	}

	%colorMode = "decRGB";

	for ( %i = 0;  %i < 4  &&  %colorMode $= "decRGB";  %i++ )
	{
		%value = getWord (%color, %i);

		if ( %value > 1 )
		{
			%colorMode = "RGB";
		}
		else if ( %value < 0 )
		{
			return "";
		}
	}

	%debugColor = "";

	if ( %colorMode $= "RGB" )
	{
		for ( %i = 0;  %i < 4;  %i++ )
		{
			%value = getWord (%color, %i);

			if ( %debugColor $= "" )
			{
				%debugColor = %value / 255;
			}
			else
			{
				%debugColor = %debugColor SPC %value / 255;
			}
		}
	}
	else
	{
		%debugColor = %color;
	}

	return %debugColor;
}

function DirectedGridGraph::_drawDebugEdge ( %this, %fromX, %fromY, %toX, %toY, %lineSize, %edgeColor )
{
	if ( !%this.debugShowLines  ||  !%this._isHostingServer () )
	{
		return -1;
	}

	if ( !isObject (%this.debugObjects) )
	{
		%this._missingDebugObjectsError ();
		return -1;
	}

	if ( !isObject (%this.debugEdge_[%fromX, %fromY, %toX, %toY]) )
	{
		%fromPos = %this._getDebugDrawPos (%fromX, %fromY);
		%toPos   = %this._getDebugDrawPos (%toX, %toY);

		if ( %lineSize $= "" )
		{
			%lineSize = %this.debugLineSize;
		}

		if ( %edgeColor $= "" )
		{
			%edgeColor = %this.debugEdgeColor;
		}

		%line = drawLine (%fromPos, %toPos, %edgeColor, %lineSize);
		%line.directedGridGraph = %this;
		%line.digraphEdgeCoords = %fromX SPC %fromY TAB %toX SPC %toY;

		%this.debugEdge_[%fromX, %fromY, %toX, %toY] = %line;
		%this.debugObjects.add (%line);

		return %line;
	}

	return -1;
}

function DirectedGridGraph::_eraseDebugEdge ( %this, %fromX, %fromY, %toX, %toY )
{
	// Print the error message, but don't return -- we still want to delete the object.

	if ( !isObject (%this.debugObjects) )
	{
		%this._missingDebugObjectsError ();
	}

	%line = %this.debugEdge_[%fromX, %fromY, %toX, %toY];

	if ( isObject (%line) )
	{
		%this.debugEdge_[%fromX, %fromY, %toX, %toY] = "";
		%line.delete ();

		return true;
	}

	return false;
}

function DirectedGridGraph::_drawDebugVertex ( %this, %x, %y, %vertexSize, %vertexColor )
{
	if ( !%this.debugShowLines  ||  !%this._isHostingServer () )
	{
		return -1;
	}

	if ( !isObject (%this.debugObjects) )
	{
		%this._missingDebugObjectsError ();
		return -1;
	}

	if ( %vertexSize $= "" )
	{
		%vertexSize = %this.debugLineSize * 1.01;  // Multiply it by 1.01 to prevent z-fighting
	}

	if ( %vertexColor $= "" )
	{
		%vertexColor = %this.debugVertexColor;
	}

	%startPos = %this._getDebugDrawPos (%x, %y, 0, -%vertexSize);
	%endPos   = %this._getDebugDrawPos (%x, %y, 0, %vertexSize);

	%line = drawLine (%startPos, %endPos, %vertexColor, %vertexSize);
	%line.directedGridGraph   = %this;
	%line.digraphVertexCoords = %x SPC %y;

	%this.debugVertex_[%x, %y] = %line;
	%this.debugObjects.add (%line);

	return %line;
}

function DirectedGridGraph::_eraseDebugVertex ( %this, %x, %y )
{
	// Print the error message, but don't return -- we still want to delete the object.

	if ( !isObject (%this.debugObjects) )
	{
		%this._missingDebugObjectsError ();
	}

	%line = %this.debugVertex_[%x, %y];

	if ( isObject (%line) )
	{
		%this.debugVertex_[%x, %y] = "";
		%line.delete ();

		return true;
	}

	return false;
}
