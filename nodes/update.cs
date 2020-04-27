function RollercoasterNode::getTransform ( %this )
{
	return %this.position SPC %this.rotation;
}

function RollercoasterNode::setTransform ( %this, %transform )
{
	%position = getWords (%transform, 0, 2);

	%this.position = %position;
	%this.rotation = getWords (%transform, 3);

	if ( isObject (%this.debugRCNode) )
	{
		%this.debugRCNode.setTransform (%position);
	}

	if ( isObject (%lineFrom = %this.debugRCLineFrom) )
	{
		%this.drawDebugLine (%lineFrom.debugRCToNodeSO);
	}

	if ( isObject (%lineTo = %this.debugRCLineTo) )
	{
		%lineTo.debugRCFromNodeSO.drawDebugLine (%this);
	}

	// Now we have to fix the paths of all the train cameras by rebuilding the entire path since
	// there's no way of updating PathCamera nodes.
	//
	// Have I mentioned how awful the PathCamera API is???

	%this.rollercoaster.resetTrainPaths ();
}

function RollercoasterNode::updateProperties ( %this, %speed, %type, %path )
{
	%this.speed = defaultValue (%speed, %this.speed);
	%this.type  = defaultValue (%type, %this.type);
	%this.path  = defaultValue (%path, %this.path);

	%this.rollercoaster.resetTrainPaths ();
}
