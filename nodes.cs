$Rollercoaster::Default::NodeType = "Normal";
$Rollercoaster::Default::NodePath = "Linear";

$Rollercoaster::MaxNodes = 18;

if ( !$Rollercoaster::ClassesInitialized )
{
	// Assert RollercoasterNode as a superClass.
	new ScriptObject () { superClass = RollercoasterNode; }.delete ();
}

// ------------------------------------------------


function RollercoasterNode::onAdd ( %this, %obj )
{
	// Callback
}

function RollercoasterNode::onRemove ( %this, %obj )
{
	if ( isObject (%this.debugLine) )
	{
		%this.debugLine.delete ();
	}

	if ( isObject (%this.debugNode) )
	{
		%this.debugNode.delete ();
	}
}

function Rollercoaster::createNode ( %this, %transform, %speed, %type, %path )
{
	%node = new ScriptObject ()
	{
		superClass = RollercoasterNode;

		position = getWords (%transform, 0, 2);
		rotation = getWords (%transform, 3);

		speed = defaultValue (%speed, $Rollercoaster::Default::Speed);
		type  = defaultValue (%type, $Rollercoaster::Default::NodeType);
		path  = defaultValue (%path, $Rollercoaster::Default::NodePath);
	};

	%this.nodes.add (%node);
	%this.drawDebugNode (%node);

	return %node;
}

function Rollercoaster::pushNode ( %this, %transform, %speed, %type, %path )
{
	%overwriteSpeed = %speed !$= "";

	%nodes     = %this.nodes;
	%nodeCount = %nodes.getCount ();

	%tail    = 0;
	%prevPos = "";

	if ( %nodeCount > 0 )
	{
		%tail    = %nodes.getObject (%nodeCount - 1);
		%speed   = defaultValue (%speed, %tail.speed);
		%prevPos = %tail.position;
	}

	%speed = defaultValue (%speed, %this.initialSpeed);

	if ( %prevPos !$= "" )
	{
		%prevZ = getWord (%prevPos, 2);
		%newZ  = getWord (%transform, 2);

		if ( !%overwriteSpeed )
		{
			%speed += %prevZ - %newZ;
		}
	}

	%node = %this.createNode (%transform, %speed, %type, %path);

	%this.drawDebugLine (%tail, %node);

	return %node;
}
