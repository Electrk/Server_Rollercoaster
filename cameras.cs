datablock PathCameraData (RollercoasterPathCam)
{
	placeholder_field_so_tork_doesnt_throw_a_syntax_error = "";
};

if ( !$Rollercoaster::ClassesInitialized )
{
	// Assert RollercoasterCamera as a superClass.
	new ScriptObject () { superClass = RollercoasterCamera; }.delete ();
}

// ------------------------------------------------


function RollercoasterCamera::onAdd ( %this, %obj )
{
	// Callback
}

function RollercoasterCamera::onRemove ( %this, %obj )
{
	%this.pathCam.delete ();
}

function RollercoasterPathCam::onNode ( %data, %this, %node )
{
	%scriptObject  = %this.scriptObject;
	%rollercoaster = %scriptObject.rollercoaster;

	%nodes = %rollercoaster.nodes;
	%count = %nodes.getCount ();

	%nodeIndex = %scriptObject.currNodeIndex + $Rollercoaster::MaxNodes - 1;

	if ( %nodeIndex < %count  &&  %scriptObject.currNodeIndex > 0 )
	{
		%node = %nodes.getObject (%nodeIndex);
		%this.pushBack (%node.position SPC %node.rotation, %node.speed, %node.type, %node.path);
	}

	%scriptObject.currNodeIndex++;
}

function Rollercoaster::createCamera ( %this )
{
	%scriptObject = new ScriptObject ()
	{
		superClass    = RollercoasterCamera;
		rollercoaster = %this;
		currNodeIndex = 0;
	};

	%pathCam = new PathCamera ()
	{
		dataBlock = RollercoasterPathCam;
		position  = getWords (%this.transform, 0, 2);
		rotation  = getWords (%this.transform, 3);
	};

	%scriptObject.pathCam = %pathCam;
	%pathCam.scriptObject = %scriptObject;

	%pathCam.setState ("stop");

	%nodes = %this.nodes;
	%count = %nodes.getCount ();
	%end   = $Rollercoaster::MaxNodes;

	for ( %i = 0;  %i < %end  &&  %i < %count;  %i++ )
	{
		%node = %nodes.getObject (%i);
		%pathCam.pushBack (%node.position SPC %node.rotation, %node.speed, %node.type, %node.path);
	}

	%this.cameras.add (%scriptObject);

	return %scriptObject;
}
