datablock PathCameraData (RollercoasterCamera)
{
	placeholder_field_so_tork_doesnt_throw_a_syntax_error = "";
};

// ------------------------------------------------


function RollercoasterCamera::onNode ( %data, %this, %node )
{
	%rollercoaster = %this.rollercoaster;

	%nodes = %rollercoaster.nodes;
	%count = %nodes.getCount ();

	%nodeIndex = %this.currNodeIndex + $Rollercoaster::MaxNodes - 1;

	if ( %nodeIndex < %count  &&  %this.currNodeIndex > 0 )
	{
		%node = %nodes.getObject (%nodeIndex);
		%this.pushBack (%node.position SPC %node.rotation, %node.speed, %node.type, %node.path);
	}

	%this.currNodeIndex++;
}

function Rollercoaster::createCamera ( %this )
{
	if ( isObject (%this.pathCam) )
	{
		%this.pathCam.delete ();
	}

	%pathCam = new PathCamera ()
	{
		dataBlock = RollercoasterCamera;
		position  = getWords (%this.transform, 0, 2);
		rotation  = getWords (%this.transform, 3);
	};

	%pathCam.currNodeIndex = 0;
	%pathCam.rollercoaster = %this;

	%pathCam.setState ("stop");

	%nodes = %this.nodes;
	%count = %nodes.getCount ();
	%end   = $Rollercoaster::MaxNodes;

	for ( %i = 0;  %i < %end  &&  %i < %count;  %i++ )
	{
		%node = %nodes.getObject (%i);
		%pathCam.pushBack (%node.position SPC %node.rotation, %node.speed, %node.type, %node.path);
	}

	%this.pathCam = %pathCam;

	%riders     = %this.riders;
	%riderCount = %riders.getCount ();

	for ( %i = 0;  %i < %riderCount;  %i++ )
	{
		%riders.getObject (%i).setControlObject (%pathCam);
	}

	return %pathCam;
}
