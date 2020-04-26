datablock PathCameraData (RollercoasterCamera)
{
	placeholder_field_so_tork_doesnt_throw_a_syntax_error = "";
};

function RollercoasterCamera::onNode ( %data, %this, %nodeIndex )
{
	%this.rollercoaster.shiftTrainWindow (%this.rollercoasterTrain);
}

function Rollercoaster::pushCameraNodes ( %this, %pathCam, %startIndex )
{
	%pathCam.reset ();

	%startIndex = defaultValue (%startIndex, 0);
	%endIndex   = mMin (%startIndex + $Rollercoaster::MaxNodes, %this.nodes.getCount ());

	for ( %i = %startIndex;  %i < %endIndex;  %i++ )
	{
		%this.pushCameraNode (%pathCam, %i);
	}
}

function Rollercoaster::pushCameraNode ( %this, %pathCam, %nodeIndex )
{
	%nodeIndex = mClamp (%nodeIndex, 0, %this.nodes.getCount () - 1);
	%node      = %this.nodes.getObject (%nodeIndex);

	%pathCam.pushBack (%node.position SPC %node.rotation, %node.speed, %node.type, %node.path);
}
