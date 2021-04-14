using System;
using Microsoft.Exchange.Data.Directory.SystemConfiguration.ConfigurationSettings;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class AdDriverConfigImpl : ConfigBase<AdDriverConfigSchema>
	{
		public static void Refresh()
		{
			ConfigBase<AdDriverConfigSchema>.Provider.GetDiagnosticInfo(DiagnosableParameters.Create("invokescan", false, false, null));
		}
	}
}
