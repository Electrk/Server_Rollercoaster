$Rollercoaster::DebugMode        = defaultValue ($Rollercoaster::DebugMode, true);
$Rollercoaster::DebugLineSize    = defaultValue ($Rollercoaster::DebugLineSize, 0.1);
$Rollercoaster::DebugLineColor   = defaultValue ($Rollercoaster::DebugLineColor, "1 0 0 0.5");
$Rollercoaster::DebugVertexColor = defaultValue ($Rollercoaster::DebugVertexColor, "1 1 0 0.5");


function Rollercoaster::drawDebugLine ( %this, %fromNode, %toNode )
{
	if ( !isObject (%fromNode)  ||  !isObject (%toNode) )
	{
		return;
	}

	if ( $Rollercoaster::DebugMode )
	{
		%line = drawLine (%fromNode.position, %toNode.position, $Rollercoaster::DebugLineColor,
			$Rollercoaster::DebugLineSize);

		if ( isObject (%fromNode.debugLine) )
		{
			%fromNode.debugLine.delete ();
		}

		%fromNode.debugLine = %line;
		%this.debugObjects.add (%line);
	}
}

function Rollercoaster::drawDebugNode ( %this, %node )
{
	if ( !isObject (%node) )
	{
		return;
	}

	if ( $Rollercoaster::DebugMode )
	{
		%size = $Rollercoaster::DebugLineSize * 1.01;

		%fromPos = vectorAdd (%node.position, -%size @ " 0 0");
		%toPos   = vectorAdd (%node.position, %size @ " 0 0");

		%line = drawLine (%fromPos, %toPos, $Rollercoaster::DebugVertexColor, %size);

		if ( isObject (%node.debugNode) )
		{
			%node.debugNode.delete ();
		}

		%node.debugNode = %line;
		%this.debugObjects.add (%line);
	}
}
