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

function Rollercoaster::setTrainPosition ( %this, %train, %index )
{
	%maxIndex = %this.nodes.getCount () - 1;
	%index    = mClamp (%index, 0, %maxIndex);

	%this.pushCameraNodes (%train.pathCam, %index);

	%train.trainWindowStart = %index;
	%train.trainWindowEnd   = mMin (%index + $Rollercoaster::MaxNodes - 1, %maxIndex);

	%train.pathCam.setPosition (1);
	%train.setTrainRunning (%train.isTrainRunning);
}

function Rollercoaster::shiftTrainWindow ( %this, %train )
{
	%maxIndex = %this.nodes.getCount () - 1;

	if ( %train.trainWindowEnd < %maxIndex )
	{
		%this.pushCameraNode (%train.pathCam, %train.trainWindowEnd + 1);
		%train.trainWindowEnd++;
	}

	if ( %train.trainWindowStart < %maxIndex )
	{
		%train.trainWindowStart++;
	}
}
