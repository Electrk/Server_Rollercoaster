$Rollercoaster::Default::Transform = "0 0 0 1 0 0 90";
$Rollercoaster::Default::Speed     = 2;

if ( !$Rollercoaster::ClassesInitialized )
{
	// Assert Rollercoaster as a superClass.
	new ScriptObject () { superClass = Rollercoaster; }.delete ();
}


function Rollercoaster::onAdd ( %this, %obj )
{
	%this.nodes        = new SimSet ();
	%this.cameras      = new SimSet ();
	%this.riders       = new SimSet ();
	%this.debugObjects = new SimSet ();
}

function Rollercoaster::onRemove ( %this, %obj )
{
	%this.debugObjects.deleteAll ();
	%this.debugObjects.delete ();

	%this.nodes.deleteAll ();
	%this.nodes.delete ();

	%this.cameras.deleteAll ();
	%this.cameras.delete ();

	%riders     = %this.riders;
	%riderCount = %riders.getCount ();

	for ( %i = 0;  %i < %riderCount;  %i++ )
	{
		%riders.getObject (%i).instantRespawn ();
	}

	%riders.delete ();
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
