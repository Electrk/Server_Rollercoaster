function Rollercoaster::setRollercoasterName ( %this, %name )
{
	%name       = trim (%name);
	%objectName = "RollercoasterTrack_" @ %name;

	if ( (isObject (%objectName)  &&  %objectName.getID () != %this)  ||  %name $= "" )
	{
		return $Rollercoaster::Error::NameConflict;
	}

	%this.setName (%objectName);
	%this.rollercoasterName = %name;

	return $Rollercoaster::Error::None;
}

package Server_Rollercoaster
{
	function onMissionLoaded ()
	{
		Parent::onMissionLoaded ();

		if ( !isObject (RollercoasterSet) )
		{
			MissionCleanup.add (new SimSet (RollercoasterSet));
		}

		if ( !isObject (RollercoasterTracks) )
		{
			MissionCleanup.add (new SimSet (RollercoasterTracks));
		}
	}

	function fxDTSBrick::onPlant ( %this )
	{
		Parent::onPlant (%this);
		Rollercoaster_onBrickPlanted (%this);
	}

	function fxDTSBrick::onLoadPlant ( %this )
	{
		Parent::onLoadPlant (%this);
		Rollercoaster_onBrickPlanted (%this);
	}

	function fxDTSBrick::onRemove ( %this )
	{
		Parent::onRemove (%this);
		Rollercoaster_onBrickRemoved (%this);
	}
};
activatePackage (Server_Rollercoaster);
