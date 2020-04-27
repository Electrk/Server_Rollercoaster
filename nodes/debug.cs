function RollercoasterNode::drawDebugLine ( %this, %toNode )
{
	if ( !isObject (%toNode) )
	{
		return $Rollercoaster::Error::ObjectNotExist;
	}

	if ( $Rollercoaster::DebugMode )
	{
		%line = drawLine (%this.position, %toNode.position, $Rollercoaster::DebugLineColor,
			$Rollercoaster::DebugLineSize);

		if ( isObject (%this.debugRCLineFrom) )
		{
			%this.debugRCLineFrom.delete ();
		}

		if ( isObject (%toNode.debugRCLineTo) )
		{
			%toNode.debugRCLineTo.delete ();
		}

		%line.debugRCType       = "line";
		%line.debugRCFromNodeSO = %this;
		%line.debugRCToNodeSO   = %toNode;

		%this.debugRCLineFrom = %line;
		%toNode.debugRCLineTo = %line;
	}

	return $Rollercoaster::Error::None;
}

function RollercoasterNode::drawDebugNode ( %this )
{
	if ( $Rollercoaster::DebugMode )
	{
		%size = $Rollercoaster::DebugLineSize * 1.1;

		%fromPos = vectorAdd (%this.position, -%size @ " 0 0");
		%toPos   = vectorAdd (%this.position, %size @ " 0 0");

		%line = drawLine (%fromPos, %toPos, $Rollercoaster::DebugVertexColor, %size);

		if ( isObject (%this.debugRCNode) )
		{
			%this.debugRCNode.delete ();
		}

		%line.debugRCType   = "node";
		%line.debugRCNodeSO = %this;

		%this.debugRCNode = %line;
	}
}

function Rollercoaster::drawDebugLine ( %this, %fromIndex, %toIndex )
{
	// Only allow drawing lines between adjacent nodes.
	if ( mAbs (%fromIndex - %toIndex) != 1 )
	{
		return;
	}

	%nodes     = %this.nodes;
	%nodeCount = %nodes.getCount ();

	if ( %fromIndex < 0  ||  %fromIndex >= %nodeCount  ||  %toIndex < 0  ||  %toIndex >= %nodeCount )
	{
		return;
	}

	%fromNode = %nodes.getObject (%fromIndex);
	%toNode   = %nodes.getObject (%toIndex);

	%fromNode.drawDebugLine (%toNode);
}

function Rollercoaster::drawDebugNode ( %this, %index )
{
	%nodes     = %this.nodes;
	%nodeCount = %nodes.getCount ();

	if ( %index < 0  ||  %index >= %nodeCount )
	{
		return;
	}

	%nodes.getObject (%index).drawDebugNode ();
}
