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

	if ( isObject (%controlObject = %client.getControlObject ())  &&  %controlObject != %pathCam )
	{
		%client.rollercoasterPrevObj = %controlObject;
	}

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

	if ( isObject (%client.rollercoasterPrevObj) )
	{
		%client.setControlObject (%client.rollercoasterPrevObj);
	}
	else
	{
		%client.instantRespawn ();
	}

	%client.rollercoasterPrevObj = "";

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
