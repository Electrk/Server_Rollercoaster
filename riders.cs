function Rollercoaster::addRider ( %this, %client )
{
	if ( %client.getClassName () !$= "GameConnection" )
	{
		return false;
	}

	%cameras = %this.cameras;

	if ( %cameras.getCount () <= 0 )
	{
		return false;
	}

	%this.riders.add (%client);

	// FIXME
	%client.setControlObject (%cameras.getObject (0).pathCam);

	return true;
}

function Rollercoaster::removeRider ( %this, %client )
{
	if ( !%this.riders.isMember (%client) )
	{
		return false;
	}

	%this.riders.remove (%client);
	%client.instantRespawn ();

	return true;
}
