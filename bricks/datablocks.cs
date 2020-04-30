datablock fxDTSBrickData (brick1xS_RCTrackData)
{
	brickFile   = "./models/straightTrack.blb";
	category    = "RC Tracks";
	subcategory = "Straight Tracks";
	uiName      = "Straight RC Track";

	isRollercoasterTrack = true;

	//* A connection point is: x, y, z, pitch, roll, yaw *//

	RCConnectPoint0 = "-2 0 0 0 0 90";
	RCConnectPoint1 = "2 0 0 0 0 90";
};

datablock fxDTSBrickData (brick1xLT_RCTrackData)
{
	brickFile   = "./models/leftTurnTrack.blb";
	category    = "RC Tracks";
	subcategory = "Straight Tracks";
	uiName      = "Left Turn RC Track";
};

datablock fxDTSBrickData (brick1xRT_RCTrackData)
{
	brickFile   = "./models/rightTurnTrack.blb";
	category    = "RC Tracks";
	subcategory = "Straight Tracks";
	uiName      = "Right Turn RC Track";
};
