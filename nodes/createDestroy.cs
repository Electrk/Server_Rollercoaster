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
	if ( isObject (%this.debugRCLineFrom) )
	{
		%this.debugRCLineFrom.delete ();
	}

	if ( isObject (%this.debugRCLineTo) )
	{
		%this.debugRCLineTo.delete ();
	}

	if ( isObject (%this.debugRCNode) )
	{
		%this.debugRCNode.delete ();
	}
}

function Rollercoaster::createNode ( %this, %transform, %speed, %type, %path )
{
	%overwriteSpeed = %speed !$= "";

	%nodes     = %this.nodes;
	%nodeCount = %nodes.getCount ();

	%tail = 0;

	if ( %nodeCount > 0 )
	{
		%tail = %nodes.getObject (%nodeCount - 1);
	}

	if ( !%overwriteSpeed )
	{
		%speed = %this.calculateNodeSpeed (%transform, %speed, %type, %path);
	}

	%node = new ScriptObject ()
	{
		superClass = RollercoasterNode;

		position = getWords (%transform, 0, 2);
		rotation = getWords (%transform, 3);

		speed = defaultValue (%speed, $Rollercoaster::Default::Speed);
		type  = defaultValue (%type, $Rollercoaster::Default::NodeType);
		path  = defaultValue (%path, $Rollercoaster::Default::NodePath);

		rollercoaster = %this;
	};

	%this.nodes.add (%node);

	%node.drawDebugNode ();

	if ( isObject (%tail) )
	{
		%tail.drawDebugLine (%node);
	}

	return %node;
}

function Rollercoaster::deleteNode ( %this, %index )
{
	%nodes     = %this.nodes;
	%nodeCount = %nodes.getCount ();

	if ( %index < 0  ||  %index >= %nodeCount )
	{
		return false;
	}

	%lastItem = %nodes.getObject (%nodeCount - 1);

	%node = %nodes.getObject (%index);
	%nodes.remove (%node);

	if ( %node != %lastItem )
	{
		// When an object gets removed from a SimSet, the last item in the SimSet simply just takes
		// its place.  We need nodes to keep their order, so we have to push it to the back again.
		//
		// Big brain problems require big brain solutions.

		%nodes.pushToBack (%lastItem);
	}

	%node.delete ();

	%nodeCount = %nodes.getCount ();

	if ( %index < %nodeCount )
	{
		if ( %index > 0 )
		{
			%this.drawDebugLine (%index - 1, %index);
		}

		if ( %index < %nodeCount - 1 )
		{
			%this.drawDebugLine (%index, %index + 1);
		}
	}

	%this.resetTrainPaths ();
}

function Rollercoaster::deleteAllNodes ( %this )
{
	%this.nodes.deleteAll ();

	%trains     = %this.trains;
	%trainCount = %trains.getCount ();

	for ( %i = 0;  %i < %trainCount;  %i++ )
	{
		%train = %trains.getObject (%i);

		%train.pathCam.reset ();
		%train.setTrainPosition (0);
	}
}

function Rollercoaster::calculateNodeSpeed ( %this, %transform, %speed, %type, %path )
{
	%nodes     = %this.nodes;
	%nodeCount = %nodes.getCount ();

	%tail = 0;

	if ( %nodeCount > 0 )
	{
		%tail  = %nodes.getObject (%nodeCount - 1);
		%speed = defaultValue (%speed, %tail.speed);
	}

	%speed = defaultValue (%speed, %this.initialSpeed);

	if ( isObject (%tail) )
	{
		%prevZ = getWord (%tail.position, 2);
		%newZ  = getWord (%transform, 2);

		%speed += %prevZ - %newZ;
	}

	return %speed;
}
