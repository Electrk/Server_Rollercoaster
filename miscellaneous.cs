function Rollercoaster::setRollercoasterName ( %this, %name )
{
	if ( !isObject (RollercoasterSet) )
	{
		return $Rollercoaster::Error::NoSimSet;
	}

	%name       = trim (%name);
	%objectName = "RollercoasterTrack_" @ %name;

	if ( isObject (%objectName)  ||  %name $= "" )
	{
		return $Rollercoaster::Error::NameConflict;
	}

	%this.setName (%objectName);
	%this.displayName = %name;

	return %rollercoaster;
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
	}
};
activatePackage (Server_Rollercoaster);
