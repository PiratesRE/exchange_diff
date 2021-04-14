using System;

namespace Microsoft.Exchange.VariantConfiguration
{
	[Flags]
	public enum VariantTypeFlags
	{
		None = 0,
		Public = 1,
		Prefix = 2,
		AllowedInSettings = 4,
		AllowedInFlights = 8,
		AllowedInTeams = 16
	}
}
