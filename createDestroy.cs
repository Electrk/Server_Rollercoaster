$Rollercoaster::Default::Transform = "0 0 0 1 0 0 90";
$Rollercoaster::Default::Speed     = 2;

if ( !$Rollercoaster::ClassesInitialized )
{
	// Assert Rollercoaster as a superClass.
	new ScriptObject () { superClass = Rollercoaster; }.delete ();
}


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

function Rollercoaster_Create ( %transform, %speed )
{
	return new ScriptObject ()
	{
		superClass = Rollercoaster;

		transform    = defaultValue (%transform, $Rollercoaster::Default::Transform);
		initialSpeed = defaultValue (%speed, $Rollercoaster::Default::Speed);
	};
}
