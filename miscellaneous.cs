function Rollercoaster::setRollercoasterName ( %this, %name )
{
	%name       = trim (%name);
	%objectName = "Rollercoaster_" @ %name;

	if ( (isObject (%objectName)  &&  %objectName != %this)  ||  %name $= "" )
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
	}
};
activatePackage (Server_Rollercoaster);
