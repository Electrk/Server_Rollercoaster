// We have to use these three methods to keep track of whether the train is running or not, since
// the PathCamera API provides absolutely zero getters for any of the setters it adds.
//
// Have I ever mentioned how awful the PathCamera API is??

function RollercoasterTrain::startTrain ( %this )
{
	%this.pathCam.setState ("forward");
	%this.isTrainRunning = true;

	if ( %this.trainWindowStart == 0 )
	{
		%this.onTrainBegin ();
	}
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

	// Get decimal part of index so we can set the camera in between nodes if we want to.
	%fractional = %index - mFloor (%index);

	// mClamp floors numbers so we don't need to call mFloor.
	%index = mClamp (%index, 0, %maxIndex);

	%this.pushCameraNodes (%index);

	%this.trainWindowStart = %index;
	%this.trainWindowEnd   = mMin (%index + $Rollercoaster::MaxNodes - 1, %maxIndex);

	// Position 0 of PathCamera is usually its initial transform, and it's a bit buggy and
	// unreliable, so we're just using position 1 and onward.
	%this.pathCam.setPosition (1 + %fractional);

	// pathCam.setPosition resets its state, so we need to set it again.
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

function RollercoasterTrain::onTrainBegin ( %this )
{
	// Callback
}

function RollercoasterTrain::onTrainReachEnd ( %this )
{
	// Callback
}
