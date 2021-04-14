using System;

namespace Microsoft.Exchange.Hygiene.Data
{
	[Flags]
	public enum SqlPropertyDefinitionFlags
	{
		None = 0,
		Required = 1,
		MXRecord = 2,
		Extended = 1,
		MultiValued = 2,
		ExtendedMultiValued = 3
	}
}
