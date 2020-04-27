function RollercoasterTrain::addRider ( %this, %client )
{
	if ( %client.getClassName () !$= "GameConnection" )
	{
		return $Rollercoaster::Error::InvalidClassName;
	}

	%this.riders.add (%client);

	if ( isObject (%controlObject = %client.getControlObject ())  &&  %controlObject != %this.pathCam )
	{
		%client.rollercoasterPrevObj = %controlObject;
	}

	%client.setControlObject (%this.pathCam);

	return $Rollercoaster::Error::None;
}

function RollercoasterTrain::removeRider ( %this, %client )
{
	if ( !%this.riders.isMember (%client) )
	{
		return;
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
}

function RollercoasterTrain::removeAllRiders ( %this )
{
	%riders = %this.riders;

	while ( %riders.getCount () > 0 )
	{
		%this.removeRider (%riders.getObject (0));
	}
}
