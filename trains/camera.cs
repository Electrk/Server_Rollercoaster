datablock PathCameraData (RollercoasterCamera)
{
	placeholder_field_so_tork_doesnt_throw_a_syntax_error = "";
};

function RollercoasterCamera::onNode ( %data, %this, %nodeIndex )
{
	%train = %this.rollercoasterTrain;

	%train.shiftTrainWindow ();

	if ( %train.trainWindowStart >= %this.rollercoaster.nodes.getCount () - 1 )
	{
		if ( %train.resetOnEnd )
		{
			%train.stopTrain ();
			%train.setTrainPosition (0);
		}

		%train.onTrainReachEnd ();
	}
}

function RollercoasterTrain::pushCameraNodes ( %this, %startIndex )
{
	%this.pathCam.reset ();

	%startIndex = defaultValue (%startIndex, 0);
	%endIndex   = mMin (%startIndex + $Rollercoaster::MaxNodes, %this.rollercoaster.nodes.getCount ());

	for ( %i = %startIndex;  %i < %endIndex;  %i++ )
	{
		%this.pushCameraNode (%i);
	}
}

function RollercoasterTrain::pushCameraNode ( %this, %nodeIndex )
{
	%rollercoaster = %this.rollercoaster;
	%nodeIndex     = mClamp (%nodeIndex, 0, %rollercoaster.nodes.getCount () - 1);
	%node          = %rollercoaster.nodes.getObject (%nodeIndex);

	%this.pathCam.pushBack (%node.position SPC %node.rotation, %node.speed, %node.type, %node.path);
}
