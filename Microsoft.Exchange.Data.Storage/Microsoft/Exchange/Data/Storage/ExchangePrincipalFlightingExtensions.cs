using System;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage.Principal;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal static class ExchangePrincipalFlightingExtensions
	{
		public static VariantConfigurationSnapshot GetConfiguration(this IExchangePrincipal exchangePrincipal)
		{
			return VariantConfiguration.GetSnapshot(exchangePrincipal.GetContext(null), null, null);
		}

		public static IConstraintProvider GetContext(this IExchangePrincipal exchangePrincipal, ExchangeConfigurationUnit configurationUnit = null)
		{
			if (configurationUnit == null)
			{
				return new ExchangePrincipalConstraintProvider(exchangePrincipal, "Global", false);
			}
			string rampId = ExchangeConfigurationUnitVariantConfigurationParser.GetRampId(configurationUnit);
			bool isFirstRelease = ExchangeConfigurationUnitVariantConfigurationParser.IsFirstRelease(configurationUnit);
			return new ExchangePrincipalConstraintProvider(exchangePrincipal, rampId, isFirstRelease);
		}

		public static IConstraintProvider GetContext(this IExchangePrincipal exchangePrincipal, string rampId, bool isFirstRelease)
		{
			return new ExchangePrincipalConstraintProvider(exchangePrincipal, rampId, isFirstRelease);
		}
	}
}
