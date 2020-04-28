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

function createRollercoaster ( %transform, %initialSpeed, %name )
{
	if ( !isObject (RollercoasterSet) )
	{
		return $Rollercoaster::Error::NoSimSet;
	}
 
	%name       = trim (%name);
	%objectName = "";

	if ( %name !$= "" )
	{
		%objectName = "Rollercoaster_" @ %name;

		if ( isObject (%objectName) )
		{
			return $Rollercoaster::Error::NameConflict;
		}
	}

	%rollercoaster = new ScriptObject (%objectName)
	{
		superClass = Rollercoaster;

		transform    = defaultValue (%transform, $Rollercoaster::Default::Transform);
		initialSpeed = defaultValue (%initialSpeed, $Rollercoaster::Default::Speed);

		rollercoasterName = %name;
	};

	RollercoasterSet.add (%rollercoaster);

	return %rollercoaster;
}
