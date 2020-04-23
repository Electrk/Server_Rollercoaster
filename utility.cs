function defaultValue ( %value, %defaultValue )
{
	if ( %value $= "" )
	{
		return %defaultValue;
	}

	return %value;
}
