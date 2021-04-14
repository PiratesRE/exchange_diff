using System;

namespace Microsoft.Exchange.VariantConfiguration
{
	[Flags]
	public enum VariantConfigurationBehavior
	{
		Default = 0,
		EvaluateTeams = 2,
		EvaluateFlights = 4,
		DelayLoadDataSources = 8
	}
}
