datablock PathCameraData (RollercoasterCamera)
{
	placeholder_field_so_tork_doesnt_throw_a_syntax_error = "";
};

function RollercoasterCamera::onNode ( %data, %this, %node )
{
	%rollercoaster = %this.rollercoaster;
	%train         = %this.rollercoasterTrain;

	%nodes = %rollercoaster.nodes;
	%count = %nodes.getCount ();

	%nodeIndex = %train.currNodeIndex + $Rollercoaster::MaxNodes - 1;

	if ( %nodeIndex < %count  &&  %train.currNodeIndex > 0 )
	{
		%node = %nodes.getObject (%nodeIndex);
		%this.pushBack (%node.position SPC %node.rotation, %node.speed, %node.type, %node.path);
	}

	%train.currNodeIndex++;
}

function Rollercoaster::pushCameraNodes ( %this, %pathCam, %startIndex )
{
	%pathCam.reset ();

	%startIndex = defaultValue (%startIndex, 0);

	%nodes = %this.nodes;
	%count = %nodes.getCount ();
	%end   = $Rollercoaster::MaxNodes;

	for ( %i = %startIndex;  %i < %end  &&  %i < %count;  %i++ )
	{
		%node = %nodes.getObject (%i);
		%pathCam.pushBack (%node.position SPC %node.rotation, %node.speed, %node.type, %node.path);
	}
}
