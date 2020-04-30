function Rollercoaster_onBrickPlanted ( %brick )
{
	%db = %brick.dataBlock;

	if ( !%db.isRollercoasterTrack )
	{
		return;
	}

	%angleID = %brick.angleID;
	%pos     = %brick.position;

	%cp0 = vectorAdd (Rollercoaster_rotateConnectPoint (%db.RCConnectPoint0, %angleID), %pos);
	%cp1 = vectorAdd (Rollercoaster_rotateConnectPoint (%db.RCConnectPoint1, %angleID), %pos);

	%brick.RCConnectPoint0 = %cp0;
	%brick.RCConnectPoint1 = %cp1;

	Rollercoaster_addBrickConnectPoint (%cp0, %brick);
	Rollercoaster_addBrickConnectPoint (%cp1, %brick);
}

function Rollercoaster_onBrickRemoved ( %brick )
{
	if ( !%brick.dataBlock.isRollercoasterTrack )
	{
		return;
	}

	Rollercoaster_removeBrickConnectPoint (%brick.RCConnectPoint0, %brick);
	Rollercoaster_removeBrickConnectPoint (%brick.RCConnectPoint1, %brick);
}

function Rollercoaster_rotateConnectPoint ( %connectPoint, %angleID )
{
	%x = getWord (%connectPoint, 0);
	%y = getWord (%connectPoint, 1);
	%z = getWord (%connectPoint, 2);

	%pitch = getWord (%connectPoint, 3);
	%roll  = getWord (%connectPoint, 4);
	%yaw   = getWord (%connectPoint, 5);

	switch ( %angleID )
	{
		case 0:
			%rotatePos = 0;
			%rotateYaw = 0;

		case 1:
			%rotatePos = 90;
			%rotateYaw = 90;

		// If the connection point is parallel with the ground, a yaw of 180 can connect with 0 and
		// 270 can connect with 90, so to allow that to happen, we only have 0 and 90 yaw rotations
		// when that is the case.

		case 2:
			%rotatePos = 180;
			%rotateYaw = (%pitch == 0  &&  %roll == 0) ? 0 : 180;  // 0 if parallel with ground

		case 3:
			%rotatePos = 270;
			%rotateYaw = (%pitch == 0  &&  %roll == 0) ? 90 : 270;  // 90 if parallel with ground

		default:
			return %connectPoint;
	}

	%rotation = vectorAdd (%pitch SPC %roll SPC %yaw, "0 0 " @ %rotateYaw);

	return rotatePointDeg (%x, %y, %rotatePos) SPC %z SPC %rotation;
}

function Rollercoaster_addBrickConnectPoint ( %connectPoint, %brick )
{
	%trackPieces = $Rollercoaster::ConnectionPoints_[%connectPoint];
	%pieceCount  = getWordCount (%trackPieces);

	if ( %pieceCount <= 0 )
	{
		$Rollercoaster::ConnectionPoints_[%connectPoint] = %brick;
	}
	else if ( %pieceCount == 1  &&  %trackPieces != %brick )
	{
		$Rollercoaster::ConnectionPoints_[%connectPoint] = %trackPieces SPC %brick;
	}
	else if ( %pieceCount == 2 )
	{
		error ("addBrickConnectPoint () - Attempted to add track piece to full connection point: " @
			%connectPoint);
	}
	else if ( %pieceCount > 2 )
	{
		error ("addBrickConnectPoint () - Too many track pieces at connection point: " @ %connectPoint);
	}

	Rollercoaster_debugConnectPoint (%connectPoint);
}

function Rollercoaster_removeBrickConnectPoint ( %connectPoint, %brick )
{
	%bricksAtPos = $Rollercoaster::ConnectionPoints_[%connectPoint];
	%trackPiece0 = getWord (%bricksAtPos, 0);
	%trackPiece1 = getWord (%bricksAtPos, 1);

	if ( %trackPiece0 == %brick )
	{
		$Rollercoaster::ConnectionPoints_[%connectPoint] = %trackPiece1;
	}
	else if ( %trackPiece1 == %brick )
	{
		$Rollercoaster::ConnectionPoints_[%connectPoint] = %trackPiece0;
	}

	if ( $Rollercoaster::ConnectionPoints_[%connectPoint] $= "" )
	{
		deleteVariables ("$Rollercoaster::ConnectionPoints_" @ %connectPoint);
	}

	Rollercoaster_debugConnectPoint (%connectPoint);
}

function Rollercoaster_debugConnectPoint ( %connectPoint )
{
	%marker = $Rollercoaster::ConnectionDebug_[%connectPoint];

	if ( !$Rollercoaster::DebugMode )
	{
		if ( isObject (%marker) )
		{
			%marker.delete ();
			deleteVariables ("$Rollercoaster::ConnectionDebug_" @ %connectPoint);
		}

		return;
	}

	%count = getWordCount ($Rollercoaster::ConnectionPoints_[%connectPoint]);

	if ( !isObject (%marker) )
	{
		if ( %count <= 0 )
		{
			return;
		}

		%size = $Rollercoaster::DebugLineSize * 1.05;

		%fromPos = vectorAdd (%connectPoint, -%size @ " 0 0");
		%toPos   = vectorAdd (%connectPoint, %size @ " 0 0");

		%marker = drawLine (%fromPos, %toPos, "0 1 1 0.5", $Rollercoaster::DebugLineSize);

		$Rollercoaster::ConnectionDebug_[%connectPoint] = %marker;
	}

	if ( %count == 2 )
	{
		%marker.setNodeColor ("ALL", "1 1 1 0.5");
	}
	else if ( %count == 1 )
	{
		%marker.setNodeColor ("ALL", "0 1 1 0.5");
	}
	else
	{
		%marker.delete ();
		deleteVariables ("$Rollercoaster::ConnectionDebug_" @ %connectPoint);
	}
}
