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

function Rollercoaster::createTrain ( %this, %resetOnEnd )
{
	%train = new ScriptObject ()
	{
		superClass = RollercoasterTrain;

		resetOnEnd    = defaultValue (%resetOnEnd, true);
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
