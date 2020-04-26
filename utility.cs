function defaultValue ( %value, %defaultValue )
{
	if ( %value $= "" )
	{
		return %defaultValue;
	}

	return %value;
}

function mMin ( %value1, %value2 )
{
	return (%value1 < %value2) ? %value1 : %value2;
}

function mMax ( %value1, %value2 )
{
	return (%value1 > %value2) ? %value1 : %value2;
}
