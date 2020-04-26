function RollercoasterTrain::startTrain ( %this )
{
	%this.pathCam.setState ("forward");
	%this.isTrainRunning = true;
}

function RollercoasterTrain::stopTrain ( %this )
{
	%this.pathCam.setState ("stop");
	%this.isTrainRunning = false;
}

function RollercoasterTrain::setTrainRunning ( %this, %running )
{
	if ( %running )
	{
		%this.startTrain ();
	}
	else
	{
		%this.stopTrain ();
	}
}

function RollercoasterTrain::setTrainPosition ( %this, %index )
{
	%rollercoaster = %this.rollercoaster;
	%maxIndex      = %rollercoaster.nodes.getCount () - 1;
	%index         = mClamp (%index, 0, %maxIndex);

	%this.pushCameraNodes (%index);

	%this.trainWindowStart = %index;
	%this.trainWindowEnd   = mMin (%index + $Rollercoaster::MaxNodes - 1, %maxIndex);

	%this.pathCam.setPosition (1);
	%this.setTrainRunning (%this.isTrainRunning);
}

function RollercoasterTrain::shiftTrainWindow ( %this )
{
	%maxIndex = %this.rollercoaster.nodes.getCount () - 1;

	if ( %this.trainWindowEnd < %maxIndex )
	{
		%this.pushCameraNode (%this.trainWindowEnd + 1);
		%this.trainWindowEnd++;
	}

	if ( %this.trainWindowStart < %maxIndex )
	{
		%this.trainWindowStart++;
	}
}
