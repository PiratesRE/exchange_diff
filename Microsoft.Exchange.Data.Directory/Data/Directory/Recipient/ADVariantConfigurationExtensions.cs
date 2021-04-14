using System;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.Data.Directory.Recipient
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal static class ADVariantConfigurationExtensions
	{
		public static IConstraintProvider GetContext(this ADUser user, ExchangeConfigurationUnit configurationUnit = null)
		{
			if (configurationUnit == null)
			{
				return new ADUserConstraintProvider(user, "Global", false);
			}
			string rampId = ExchangeConfigurationUnitVariantConfigurationParser.GetRampId(configurationUnit);
			bool isFirstRelease = ExchangeConfigurationUnitVariantConfigurationParser.IsFirstRelease(configurationUnit);
			return new ADUserConstraintProvider(user, rampId, isFirstRelease);
		}

		public static IConstraintProvider GetContext(this ADUser user, string rampId, bool isFirstRelease)
		{
			return new ADUserConstraintProvider(user, rampId, isFirstRelease);
		}

		public static IConstraintProvider GetContext(this MiniRecipient recipient, ExchangeConfigurationUnit configurationUnit = null)
		{
			if (configurationUnit == null)
			{
				return new MiniRecipientConstraintProvider(recipient, "Global", false);
			}
			string rampId = ExchangeConfigurationUnitVariantConfigurationParser.GetRampId(configurationUnit);
			bool isFirstRelease = ExchangeConfigurationUnitVariantConfigurationParser.IsFirstRelease(configurationUnit);
			return new MiniRecipientConstraintProvider(recipient, rampId, isFirstRelease);
		}

		public static IConstraintProvider GetContext(this MiniRecipient recipient, string rampId, bool isFirstRelease)
		{
			return new MiniRecipientConstraintProvider(recipient, rampId, isFirstRelease);
		}
	}
}
