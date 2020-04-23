new ScriptObject () { superClass = DirectedGridGraph; class = Directed3DGridGraph; }.delete ();


// ------------------------------------------------


$Directed3DGridGraph::Default::GRAPH_DEPTH = 16;


// Please, please, please remember to call Parent::onAdd if you're inheriting from 
// this super class.

function Directed3DGridGraph::onAdd ( %this, %obj )
{
	Parent::onAdd (%this, %obj);

	%this.depth = defaultValue (%this.depth, $Directed3DGridGraph::Default::GRAPH_DEPTH);

	if ( %this.depth < 2 )
	{
		%this.depth = 2;
	}
}

// ------------------------------------------------


// Adds a directed edge from a vertex (tail) to another vertex (head).
//
// @param    int fromX    The X coordinate of the vertex we're creating the edge from.
// @param    int fromY    The Y coordinate of the vertex we're creating the edge from.
// @param    int fromZ    The Z coordinate of the vertex we're creating the edge from.
// @param    int toX      The X coordinate of the vertex we're creating the edge to.
// @param    int toY      The Y coordinate of the vertex we're creating the edge to.
// @param    int toZ      The Z coordinate of the vertex we're creating the edge to.
//
// @return   Each vertex maintains a field list of other vertices it's connected to.
//           If the edge was successfully added, or if the edge already exists, this
//           method returns the index of the head in the tail's connections list.
//           If unsuccessful, it returns an empty string.


function Directed3DGridGraph::addEdge ( %this, %fromX, %fromY, %fromZ, %toX, %toY, %toZ )
{
	if ( !%this.isValidEdge (%fromX, %fromY, %fromZ, %toX, %toY, %toZ) )
	{
		%this._printFunctionError ("addEdge", "Invalid edge from " @ 
			%fromX @ ", " @ %fromY @ ", " @ %fromZ @ " to " @ %toX @ ", " @ %toY @ ", " @ %toZ);

		return "";
	}

	if ( %fromX == %toX  &&  %fromY == %toY  &&  %fromZ == %toZ )
	{
		%this._printFunctionError ("addEdge", "Same coords from " @ 
			%fromX @ ", " @ %fromY @ ", " @ %fromZ @ " to " @ %toX @ ", " @ %toY @ ", " @ %toZ);

		return "";
	}

	if ( %this.connectionIndex_[%fromX, %fromY, %fromZ, %toX, %toY, %toZ] !$= "" )
	{
		return %this.connectionIndex_[%fromX, %fromY, %fromZ, %toX, %toY, %toZ];
	}

	%toPos = %toX SPC %toY SPC %toZ;

	if ( %this.connectedTo_[%fromX, %fromY, %fromZ] $= "" )
	{
		%this.connectedTo_[%fromX, %fromY, %fromZ] = %toPos;
	}
	else
	{
		%this.connectedTo_[%fromX, %fromY, %fromZ] = %this.connectedTo_[%fromX, %fromY, %fromZ] TAB %toPos;
	}

	// Store index for quick lookup so we don't have to loop through "connectedTo"

	%index = getFieldCount (%this.connectedTo_[%fromX, %fromY, %fromZ]) - 1;
	%this.connectionIndex_[%fromX, %fromY, %fromZ, %toX, %toY, %toZ] = %index;

	%this._drawDebugEdge (%fromX, %fromY, %fromZ, %toX, %toY, %toZ);

	return %index;
}


// Removes a directed edge from a vertex (tail) to another vertex (head).
//
// @param    int fromX    The X coordinate of the tail vertex.
// @param    int fromY    The Y coordinate of the tail vertex.
// @param    int fromZ    The Z coordinate of the tail vertex.
// @param    int toX      The X coordinate of the head vertex.
// @param    int toY      The Y coordinate of the head vertex.
// @param    int toZ      The Z coordinate of the head vertex.
//
// @return   true if an edge was removed, false if not.


function Directed3DGridGraph::removeEdge ( %this, %fromX, %fromY, %fromZ, %toX, %toY, %toZ )
{
	// We don't do an isValidEdge/isValidVertex check because we still want to remove
	// any invalid edges that might exist.

	if ( !%this._isCoordinateValue (%fromX, %fromY, %fromZ)  ||
		 !%this._isCoordinateValue (%toX, %toY, %toZ) )
	{
		%this._printFunctionError ("removeEdge", "Invalid edge from " @
			%fromX @ ", " @ %fromY @ ", " @ %fromZ @ " to " @ %toX @ ", " @ %toY @ ", " @ %toZ);

		return false;
	}

	%edges = %this.connectedTo_[%fromX, %fromY, %fromZ];
	%index = %this.connectionIndex_[%fromX, %fromY, %fromZ, %toX, %toY, %toZ];

	%toPos = %toX SPC %toY SPC %toZ;

	// If index is blank, loop through `connectedTo` just in case the `connectionIndex`
	// somehow managed to get cleared, and remove the connection.

	if ( %index $= ""  &&  %edges !$= "" )
	{
		%count = getFieldCount (%edges);

		for ( %i = 0;  %i < %count;  %i++ )
		{
			if ( getField (%edges, %i) $= %toPos )
			{
				%this._printFunctionError ("removeEdge", "Missing index from " @ 
					%fromX @ ", " @ %fromY @ ", " @ %fromZ @ " to " @ %toX @ ", " @ %toY @ ", " @ %toZ);

				%index = %i;

				break;
			}
		}
	}

	if ( %index !$= ""  &&  %edges !$= ""  &&  getField (%edges, %index) $= %toPos )
	{
		%edges = removeField (%edges, %index);

		%this.connectedTo_[%fromX, %fromY, %fromZ] = %edges;
		%this.connectionIndex_[%fromX, %fromY, %fromZ, %toX, %toY, %toZ] = "";


		// Reindex everything after our removed edge.

		%count = getFieldCount (%edges);

		for ( %i = %index;  %i < %count;  %i++ )
		{
			%conn  = getField (%edges, %i);
			%connX = getWord (%conn, 0);
			%connY = getWord (%conn, 1);
			%connZ = getWord (%conn, 2);

			%this.connectionIndex_[%fromX, %fromY, %fromZ, %connX, %connY, %connZ] = %i;
		}

		%this._eraseDebugEdge (%fromX, %fromY, %fromZ, %toX, %toY, %toZ);

		return true;
	}

	return false;
}


// Adds two directed edges between two vertices.
//
// @param    int x1    The X coordinate of the first vertex.
// @param    int y1    The Y coordinate of the first vertex.
// @param    int z1    The Z coordinate of the first vertex.
// @param    int x2    The X coordinate of the second vertex.
// @param    int y2    The Y coordinate of the second vertex.
// @param    int z2    The Z coordinate of the second vertex.
//
// @return   Each vertex maintains a field list of other vertices it's connected to.
//           If the edges were successfully added, this method returns the indices
//           of both vertices in the other's connections list.
//           If unsuccessful, it returns an empty string.


function Directed3DGridGraph::addTwoWayEdge ( %this, %x1, %y1, %z1, %x2, %y2, %z2 )
{
	%firstIndex = %this.addEdge (%x1, %y1, %z1, %x2, %y2, %z2);

	if ( %firstIndex $= "" )
	{
		return "";
	}

	%secondIndex = %this.addEdge (%x2, %y2, %z2, %x1, %y1, %z1);

	if ( %secondIndex $= "" )
	{
		// This should never happen...
		%this._printFunctionError ("addTwoWayEdge", "Edge from " @ %x1 @ ", " @ %y1 @ ", " @ %z1 @ 
			" to " @ %x2 @ ", " @ %y2 @ ", " @ %z2 @ " is valid, but somehow the other way is not!");

		return "";
	}

	return %firstIndex SPC %secondIndex;
}


// Removes two directed edges between two vertices.
//
// @param    int x1    The X coordinate of the first vertex.
// @param    int y1    The Y coordinate of the first vertex.
// @param    int z1    The Z coordinate of the first vertex.
// @param    int x2    The X coordinate of the second vertex.
// @param    int y2    The Y coordinate of the second vertex.
// @param    int z2    The Z coordinate of the second vertex.
//
// @return   The number of edges that were removed.


function Directed3DGridGraph::removeTwoWayEdge ( %this, %x1, %y1, %z1, %x2, %y2, %z2 )
{
	%edgesRemoved = 0;

	if ( %this.removeEdge (%x1, %y1, %z1, %x2, %y2, %z2) )
	{
		%edgesRemoved++;
	}

	if ( %this.removeEdge (%x2, %y2, %z2, %x1, %y1, %z1) )
	{
		%edgesRemoved++;
	}

	return %edgesRemoved;
}


// Removes all edges from a vertex, optionally in both directions.
//
// @param    int fromX                      The X coordinate of the vertex.
// @param    int fromY                      The Y coordinate of the vertex.
// @param    int fromZ                      The Z coordinate of the vertex.
// @param    bool [bothDirections=false]    If true, for each edge we remove, we will
//                                          remove the edge going the other way too.
//
// @return   The number of edges that were removed.


function Directed3DGridGraph::removeEdgesFrom ( %this, %fromX, %fromY, %fromZ, %bothDirections )
{
	if ( %bothDirections $= "" )
	{
		%bothDirections = false;
	}

	%edges = %this.connectedTo_[%fromX, %fromY, %fromZ];
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
		%toZ  = getWord (%edge, 2);

		%this.connectionIndex_[%fromX, %fromY, %fromZ, %toX, %toY, %toZ] = "";
		%this._eraseDebugEdge (%fromX, %fromY, %fromZ, %toX, %toY, %toZ);

		// Remove edge going the other way, if we want to do that.

		if ( %bothDirections  &&  %this.removeEdge (%toX, %toY, %toZ, %fromX, %fromY, %fromZ) )
		{
			%edgesRemoved++;
		}
	}

	%this.connectedTo_[%fromX, %fromY, %fromZ] = "";

	return %edgesRemoved;
}

function Directed3DGridGraph::removeAllEdges ( %this )
{
	%edgesRemoved = 0;

	%width  = %this.width;
	%height = %this.height;
	%depth  = %this.depth;

	for ( %x = 0;  %x < %width;  %x++ )
	{
		for ( %y = 0;  %y < %height;  %y++ )
		{
			for ( %z = 0;  %z < %depth;  %z++ )
			{
				%edgesRemoved += %this.removeEdgesFrom (%x, %y, %z);
			}
		}
	}

	return %edgesRemoved;
}


// Checks if there is a directed edge from one vertex (tail) to another (head).
//
// @param    int fromX    The X coordinate of the tail vertex.
// @param    int fromY    The Y coordinate of the tail vertex.
// @param    int fromZ    The Z coordinate of the tail vertex.
// @param    int toX      The X coordinate of the head vertex.
// @param    int toY      The Y coordinate of the head vertex.
// @param    int toZ      The Z coordinate of the head vertex.
//
// @return   true if there is, false if there isn't.


function Directed3DGridGraph::hasEdge ( %this, %fromX, %fromY, %fromZ, %toX, %toY, %toZ )
{
	if ( %this.isEdgeOutOfBounds (%fromX, %fromY, %fromZ, %toX, %toY, %toZ) )
	{
		return false;
	}

	return %this.connectionIndex_[%fromX, %fromY, %fromZ, %toX, %toY, %toZ] !$= "";
}


// Returns the field list of edges from a vertex.
//
// @param    int fromX    The X coordinate the vertex.
// @param    int fromY    The Y coordinate the vertex.
// @param    int fromZ    The Z coordinate the vertex.
//
// @return   A field list of connections from the vertex and an empty string if there
//           aren't any or if the vertex is invalid.


function Directed3DGridGraph::edgesFrom ( %this, %x, %y, %z )
{
	if ( %this.isVertexOutOfBounds (%x, %y, %z)  ||  !%this.isValidVertex (%x, %y, %z) )
	{
		return "";
	}

	return %this.connectedTo_[%x, %y, %z];
}

function Directed3DGridGraph::isVertexOutOfBounds ( %this, %x, %y, %z )
{
	return %x < 0  ||  %x >= %this.width  ||  %y < 0  ||  %y >= %this.height  ||
	       %z < 0  ||  %z >= %this.depth;
}

function Directed3DGridGraph::isEdgeOutOfBounds ( %this, %fromX, %fromY, %fromZ, %toX, %toY, %toZ )
{
	return %this.isVertexOutOfBounds (%fromX, %fromY, %fromZ)  ||
	       %this.isVertexOutOfBounds (%toX, %toY, %toZ);
}

function Directed3DGridGraph::isValidVertex ( %this, %x, %y, %z )
{
	return %this._isCoordinateValue (%x, %y, %z)  &&  !%this.isVertexOutOfBounds (%x, %y, %z);
}

function Directed3DGridGraph::isValidEdge ( %this, %fromX, %fromY, %fromZ, %toX, %toY, %toZ )
{
	return %this.isValidVertex (%fromX, %fromY, %fromZ)  &&  %this.isValidVertex (%toX, %toY, %toZ);
}


///////////////////////
// "Private" Methods //
///////////////////////


function Directed3DGridGraph::_isCoordinateValue ( %this, %x, %y, %z )
{
	return %x !$= ""  &&  %y !$= ""  &&  %z !$= ""  &&  %this._isInt (%x)  &&  %this._isInt (%y)  &&
	       %this._isInt (%z);
}

// Don't want to require some other random function but also don't want to trample over
// any existing isInt implementations...

function Directed3DGridGraph::_isInt ( %this, %value )
{
	return %value $= (%value << 0);
}


// ------------------------------------------------

///////////
// Debug //
///////////


function Directed3DGridGraph::showServerErrorMessages ( %this )
{
	%this.debugServerMessages = true;
}

function Directed3DGridGraph::hideServerErrorMessages ( %this )
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


function Directed3DGridGraph::drawAllDebugLines ( %this )
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
	%depth  = %this.depth;

	for ( %x = 0;  %x < %width;  %x++ )
	{
		for ( %y = 0;  %y < %height;  %y++ )
		{
			for ( %z = 0;  %z < %depth;  %z++ )
			{
				%this._drawDebugVertex (%x, %y, %z);

				%edges = %this.connectedTo_[%x, %y, %z];
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
					%toZ  = getWord (%edge, 2);

					%this._drawDebugEdge (%x, %y, %z, %toX, %toY, %toZ);
				}
			}
		}
	}
}


// Use this to disable debug graphics.
// Only works on servers, obviously.

function Directed3DGridGraph::eraseAllDebugLines ( %this )
{
	if ( !%this._isHostingServer () )
	{
		return;
	}

	%width  = %this.width;
	%height = %this.height;
	%depth  = %this.depth;

	for ( %x = 0;  %x < %width;  %x++ )
	{
		for ( %y = 0;  %y < %height;  %y++ )
		{
			for ( %z = 0;  %z < %depth;  %z++ )
			{
				%this._eraseDebugVertex (%x, %y, %z);

				%edges = %this.connectedTo_[%x, %y, %z];
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
					%toZ  = getWord (%edge, 2);

					%this._eraseDebugEdge (%x, %y, %z, %toX, %toY, %toZ);
				}
			}
		}
	}

	%this.debugObjects.deleteAll ();  // Delete any remaining lines we may have missed
	%this.debugShowLines = false;
}


// Use this to change the edge and vertex thickness.
// The spacing between vertices scales with the thickness so don't worry about that.

function Directed3DGridGraph::setDebugLineSize ( %this, %size )
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


function Directed3DGridGraph::setDebugColors ( %this, %edgeColor, %vertexColor )
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

function Directed3DGridGraph::setDebugPosition ( %this, %position )
{
	%x = getWord (%position, 0);
	%y = getWord (%position, 1);
	%z = getWord (%position, 2);

	if ( %x $= "" )
	{
		if ( %this.debugPositionX $= "" )
		{
			%x = $Directed3DGridGraph::Default::DEBUG_POSITION_X;
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
			%y = $Directed3DGridGraph::Default::DEBUG_POSITION_Y;
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
			%z = $Directed3DGridGraph::Default::DEBUG_POSITION_Z;
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

function Directed3DGridGraph::getDebugPosition ( %this )
{
	return %this.debugPositionX SPC %this.debugPositionY SPC %this.debugPositionZ;
}


///////////////////////
// "Private" Methods //
///////////////////////


// Don't use these.  For real.


function Directed3DGridGraph::_printError ( %this, %message )
{
	error (%message);

	if ( %this.debugServerMessages  &&  %this._isHostingServer () )
	{
		talk (%message);
	}
}

function Directed3DGridGraph::_printFunctionError ( %this, %function, %message )
{
	%this._printError (%function @ " () - " @ %message);
}

function Directed3DGridGraph::_missingDebugObjectsError ( %this )
{
	%msg = "`debugObjects` missing!  Remember to call Parent::onAdd and Parent::onRemove!";
	%this._printError (%msg);
}

function Directed3DGridGraph::_isHostingServer ( %this )
{
	return $Server::ServerType !$= ""  &&  isObject (MissionCleanup);
}

function Directed3DGridGraph::_getDebugDrawPos ( %this, %x, %y, %z, %offsetX, %offsetY, %offsetZ )
{
	%multiplier = %this.debugLineSize * 10;

	%x = -((%x * %multiplier) + %offsetX);
	%y = (%y * %multiplier) + %offsetY;
	%z = (%z * %multiplier) + %offsetZ;

	return %x + %this.debugPositionX SPC %y + %this.debugPositionY SPC %z + %this.debugPositionZ;
}

function Directed3DGridGraph::_getDebugColor ( %this, %color )
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

function Directed3DGridGraph::_drawDebugEdge ( %this, %fromX, %fromY, %fromZ, %toX, %toY, %toZ, %lineSize, %edgeColor )
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

	if ( !isObject (%this.debugEdge_[%fromX, %fromY, %fromZ, %toX, %toY, %toZ]) )
	{
		%fromPos = %this._getDebugDrawPos (%fromX, %fromY, %fromZ);
		%toPos   = %this._getDebugDrawPos (%toX, %toY, %toZ);

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
		%line.digraphEdgeCoords = %fromX SPC %fromY SPC %fromZ TAB %toX SPC %toY SPC %toZ;

		%this.debugEdge_[%fromX, %fromY, %fromZ, %toX, %toY, %toZ] = %line;
		%this.debugObjects.add (%line);

		return %line;
	}

	return -1;
}

function Directed3DGridGraph::_eraseDebugEdge ( %this, %fromX, %fromY, %fromZ, %toX, %toY, %toZ )
{
	// Print the error message, but don't return -- we still want to delete the object.

	if ( !isObject (%this.debugObjects) )
	{
		%this._missingDebugObjectsError ();
	}

	%line = %this.debugEdge_[%fromX, %fromY, %fromZ, %toX, %toY, %toZ];

	if ( isObject (%line) )
	{
		%this.debugEdge_[%fromX, %fromY, %fromZ, %toX, %toY, %toZ] = "";
		%line.delete ();

		return true;
	}

	return false;
}

function Directed3DGridGraph::_drawDebugVertex ( %this, %x, %y, %z, %vertexSize, %vertexColor )
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

	%startPos = %this._getDebugDrawPos (%x, %y, %z, 0, -%vertexSize, 0);
	%endPos   = %this._getDebugDrawPos (%x, %y, %z, 0, %vertexSize, 0);

	%line = drawLine (%startPos, %endPos, %vertexColor, %vertexSize);
	%line.directedGridGraph   = %this;
	%line.digraphVertexCoords = %x SPC %y SPC %z;

	%this.debugVertex_[%x, %y, %z] = %line;
	%this.debugObjects.add (%line);

	return %line;
}

function Directed3DGridGraph::_eraseDebugVertex ( %this, %x, %y, %z )
{
	// Print the error message, but don't return -- we still want to delete the object.

	if ( !isObject (%this.debugObjects) )
	{
		%this._missingDebugObjectsError ();
	}

	%line = %this.debugVertex_[%x, %y, %z];

	if ( isObject (%line) )
	{
		%this.debugVertex_[%x, %y, %z] = "";
		%line.delete ();

		return true;
	}

	return false;
}
