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

function RollercoasterTrain::addRider ( %this, %client )
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

	%client.rollercoasterPrevObj = %client.getControlObject ();
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

	if ( isObject (%prevObject = %client.rollercoasterPrevObj) )
	{
		%client.setControlObject (%prevObject);
		%client.rollercoasterPrevObj = "";
	}
	else
	{
		%client.instantRespawn ();
	}

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
	%this.setTrainPosition (%train, 0);

	return %train;
}
