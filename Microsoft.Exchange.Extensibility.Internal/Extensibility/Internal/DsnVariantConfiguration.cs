using System;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.Extensibility.Internal
{
	internal static class DsnVariantConfiguration
	{
		internal static bool SystemMessageOverridesEnabled()
		{
			return VariantConfiguration.InvariantNoFlightingSnapshot.Transport.SystemMessageOverrides.Enabled;
		}
	}
}
