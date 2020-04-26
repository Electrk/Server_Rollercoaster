// There's no getState() function for PathCameras so we're going to have to use this instead.
// Have I mentioned how awful the PathCamera API is??
function RollercoasterTrain::setTrainState ( %this, %state )
{
	if ( %state !$= "stop"  &&  %state !$= "forward"  &&  %state !$= "backward" )
	{
		return false;
	}

	%this.pathCam.setState (%state);
	%this.trainState = %state;

	return true;
}

function Rollercoaster::setTrainPosition ( %this, %train, %index )
{
	%maxIndex = %this.nodes.getCount () - 1;
	%index    = mClamp (%index, 0, %maxIndex);

	%this.pushCameraNodes (%train.pathCam, %index);

	%train.trainWindowStart = %index;
	%train.trainWindowEnd   = mMin (%index + $Rollercoaster::MaxNodes - 1, %maxIndex);

	%train.pathCam.setPosition (1);
	%train.setTrainState (%train.trainState);
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
