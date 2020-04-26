$Rollercoaster::DebugMode        = defaultValue ($Rollercoaster::DebugMode, true);
$Rollercoaster::DebugLineSize    = defaultValue ($Rollercoaster::DebugLineSize, 0.1);
$Rollercoaster::DebugLineColor   = defaultValue ($Rollercoaster::DebugLineColor, "1 0 0 0.5");
$Rollercoaster::DebugVertexColor = defaultValue ($Rollercoaster::DebugVertexColor, "1 1 0 0.5");


function RollercoasterNode::drawDebugLine ( %this, %toNode )
{
	if ( !isObject (%toNode) )
	{
		return;
	}

	if ( $Rollercoaster::DebugMode )
	{
		%line = drawLine (%this.position, %toNode.position, $Rollercoaster::DebugLineColor,
			$Rollercoaster::DebugLineSize);

		if ( isObject (%this.debugLineFrom) )
		{
			%this.debugLineFrom.delete ();
		}

		if ( isObject (%toNode.debugLineTo) )
		{
			%toNode.debugLineTo.delete ();
		}

		%line.debugRollercoasterType = "line";

		%this.debugLineFrom = %line;
		%toNode.debugLineTo = %line;
	}
}

function RollercoasterNode::drawDebugNode ( %this )
{
	if ( $Rollercoaster::DebugMode )
	{
		%size = $Rollercoaster::DebugLineSize * 1.1;

		%fromPos = vectorAdd (%this.position, -%size @ " 0 0");
		%toPos   = vectorAdd (%this.position, %size @ " 0 0");

		%line = drawLine (%fromPos, %toPos, $Rollercoaster::DebugVertexColor, %size);

		if ( isObject (%this.debugNode) )
		{
			%this.debugNode.delete ();
		}

		%line.debugRollercoasterType = "node";

		%this.debugNode = %line;
	}
}
