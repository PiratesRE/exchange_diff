using System;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal static class GenericADUserFlightingExtensions
	{
		public static VariantConfigurationSnapshot GetConfiguration(this IGenericADUser adUser)
		{
			return VariantConfiguration.GetSnapshot(adUser.GetContext(null), null, null);
		}

		public static IConstraintProvider GetContext(this IGenericADUser adUser, ExchangeConfigurationUnit configurationUnit = null)
		{
			if (configurationUnit == null)
			{
				string rampId = string.IsNullOrEmpty(adUser.ExternalDirectoryObjectId) ? "Global" : adUser.ExternalDirectoryObjectId;
				return new GenericADUserConstraintProvider(adUser, rampId, false);
			}
			string rampId2 = ExchangeConfigurationUnitVariantConfigurationParser.GetRampId(configurationUnit);
			bool isFirstRelease = ExchangeConfigurationUnitVariantConfigurationParser.IsFirstRelease(configurationUnit);
			return new GenericADUserConstraintProvider(adUser, rampId2, isFirstRelease);
		}

		public static IConstraintProvider GetContext(this IGenericADUser adUser, string rampId, bool isFirstRelease)
		{
			return new GenericADUserConstraintProvider(adUser, rampId, isFirstRelease);
		}
	}
}
