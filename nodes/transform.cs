function RollercoasterNode::getTransform ( %this )
{
	return %this.position SPC %this.rotation;
}

function RollercoasterNode::setTransform ( %this, %transform, %updateTrains )
{
	%updateTrains = defaultValue (%updateTrains, true);

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

	if ( !%updateTrains )
	{
		return;
	}

	// Now we have to fix the paths of all the train cameras by rebuilding the entire path since
	// there's no way of updating PathCamera nodes.
	//
	// Have I mentioned how awful the PathCamera API is???

	%trains     = %this.rollercoaster.trains;
	%trainCount = %trains.getCount ();

	for ( %i = 0;  %i < %trainCount;  %i++ )
	{
		%train         = %trains.getObject (%i);
		%trainPosition = %train.trainWindowStart;

		%train.pushCameraNodes (%trainPosition);
		%train.setTrainPosition (%trainPosition);
	}
}
