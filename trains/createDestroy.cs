if ( !$Rollercoaster::ClassesInitialized )
{
	// Assert RollercoasterTrain as a superClass.
	new ScriptObject () { superClass = RollercoasterTrain; }.delete ();
}

// ------------------------------------------------


function RollercoasterTrain::onAdd ( %this, %obj )
{
	%this.riders = new SimSet ();
}

function RollercoasterTrain::onRemove ( %this, %obj )
{
	%this.removeAllRiders ();

	%this.riders.delete ();
	%this.pathCam.delete ();
}

function RollercoasterTrain::addRider ( %this, %client, %deleteControlObj )
{
	if ( %client.getClassName () !$= "GameConnection" )
	{
		return false;
	}

	if ( !isObject (%pathCam = %this.pathCam) )
	{
		return false;
	}

	%this.riders.add (%client);

	if ( isObject (%controlObject = %client.getControlObject ()) )
	{
		if ( %deleteControlObj )
		{
			%controlObject.delete ();
		}
		else
		{
			%client.rollercoasterPrevObj = %controlObject;
		}
	}

	%client.setControlObject (%pathCam);

	return true;
}

function RollercoasterTrain::removeRider ( %this, %client )
{
	if ( !%this.riders.isMember (%client) )
	{
		return false;
	}

	%this.riders.remove (%client);

	if ( isObject (%client.rollercoasterPrevObj) )
	{
		%client.setControlObject (%client.rollercoasterPrevObj);
	}
	else
	{
		%client.instantRespawn ();
	}

	%client.rollercoasterPrevObj = "";

	return true;
}

function RollercoasterTrain::removeAllRiders ( %this )
{
	%riders = %this.riders;

	while ( %riders.getCount () > 0 )
	{
		%this.removeRider (%riders.getObject (0));
	}
}

function Rollercoaster::createTrain ( %this )
{
	%train = new ScriptObject ()
	{
		superClass    = RollercoasterTrain;
		rollercoaster = %this;
	};

	%this.trains.add (%train);

	%pathCam = new PathCamera ()
	{
		dataBlock = RollercoasterCamera;

		position = getWords (%this.transform, 0, 2);
		rotation = getWords (%this.transform, 3);
	};

	%pathCam.rollercoaster      = %this;
	%pathCam.rollercoasterTrain = %train;

	%train.pathCam = %pathCam;

	%train.stopTrain ();
	%train.setTrainPosition (0);

	return %train;
}
