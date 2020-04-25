function Rollercoaster::addRider ( %this, %client )
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

	%client.rollercoasterPrevObj = %client.getControlObject ();
	%client.setControlObject (%pathCam);

	return true;
}

function Rollercoaster::removeRider ( %this, %client )
{
	if ( !%this.riders.isMember (%client) )
	{
		return false;
	}

	%this.riders.remove (%client);

	if ( isObject (%prevObject = %client.rollercoasterPrevObj) )
	{
		%client.setControlObject (%prevObject);
	}
	else
	{
		%client.instantRespawn ();
	}

	return true;
}
