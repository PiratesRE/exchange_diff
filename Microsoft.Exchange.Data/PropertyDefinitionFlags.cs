using System;

namespace Microsoft.Exchange.Data
{
	[Flags]
	internal enum PropertyDefinitionFlags
	{
		None = 0,
		ReadOnly = 1,
		MultiValued = 2,
		Calculated = 4,
		FilterOnly = 8,
		Mandatory = 16,
		PersistDefaultValue = 32,
		WriteOnce = 64,
		Binary = 128,
		TaskPopulated = 256
	}
}
