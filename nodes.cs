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
