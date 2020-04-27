if ( !$Rollercoaster::ClassesInitialized )
{
	// Assert Rollercoaster as a superClass.
	new ScriptObject () { superClass = Rollercoaster; }.delete ();
}

// ------------------------------------------------


function Rollercoaster::onAdd ( %this, %obj )
{
	%this.nodes  = new SimSet ();
	%this.trains = new SimSet ();
}

function Rollercoaster::onRemove ( %this, %obj )
{
	%this.nodes.deleteAll ();
	%this.nodes.delete ();

	%this.trains.deleteAll ();
	%this.trains.delete ();
}

function createRollercoaster ( %name, %transform, %speed )
{
	if ( !isObject (RollercoasterSet) )
	{
		return $Rollercoaster::Error::NoSimSet;
	}

	%name       = trim (%name);
	%objectName = "Rollercoaster_" @ %name;

	if ( isObject (%objectName)  ||  %name $= "" )
	{
		return $Rollercoaster::Error::NameConflict;
	}

	%rollercoaster = new ScriptObject (%objectName)
	{
		superClass = Rollercoaster;

		transform    = defaultValue (%transform, $Rollercoaster::Default::Transform);
		initialSpeed = defaultValue (%speed, $Rollercoaster::Default::Speed);

		rollercoasterName = %name;
	};

	RollercoasterSet.add (%rollercoaster);

	return %rollercoaster;
}
