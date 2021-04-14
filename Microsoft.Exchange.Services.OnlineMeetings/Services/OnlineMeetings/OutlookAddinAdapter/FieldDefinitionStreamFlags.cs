using System;

namespace Microsoft.Exchange.Services.OnlineMeetings.OutlookAddinAdapter
{
	[Flags]
	internal enum FieldDefinitionStreamFlags
	{
		PDO_IS_CUSTOM = 1,
		PDO_REQUIRED = 2,
		PDO_PRINT_SAVEAS = 4,
		PDO_CALC_AUTO = 8,
		PDO_FT_CONCAT = 16,
		PDO_FT_SWITCH = 32,
		PDO_PRINT_SAVEAS_DEF = 64
	}
}
