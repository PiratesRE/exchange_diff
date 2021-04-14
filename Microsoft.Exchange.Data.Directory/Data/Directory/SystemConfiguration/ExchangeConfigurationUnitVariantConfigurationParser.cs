using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	internal static class ExchangeConfigurationUnitVariantConfigurationParser
	{
		internal static string GetRampId(ExchangeConfigurationUnit configurationUnit)
		{
			if (configurationUnit == null)
			{
				throw new ArgumentNullException("configurationUnit");
			}
			return configurationUnit.ExternalDirectoryOrganizationId;
		}

		internal static bool IsFirstRelease(ExchangeConfigurationUnit configurationUnit)
		{
			return configurationUnit.ReleaseTrack == ReleaseTrack.FirstRelease;
		}
	}
}
