function Rollercoaster::setTrainPosition ( %this, %train, %index )
{
	%index     = mClamp (0, %this.nodes.getCount () - 1);
	%pathCam   = %train.pathCam;
	%currIndex = %train.currNodeIndex;

	// PathCameras can only have so many nodes before they start popping nodes from the front,
	// so we must keep it in the window and occasionally rebuild their path when necessary.
	if ( %index < %currIndex  ||  %index >= %currIndex + $Rollercoaster::MaxNodes )
	{
		%this.pushCameraNodes (%pathCam, %index);
	}

	%train.currNodeIndex = %index;

	// Position 0 for PathCameras is their initial transform, so we need to offset by 1.
	%pathCam.setPosition (%index + 1);
}
