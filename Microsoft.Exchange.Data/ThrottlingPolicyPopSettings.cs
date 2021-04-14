using System;

namespace Microsoft.Exchange.Data
{
	internal class ThrottlingPolicyPopSettings : ThrottlingPolicyBaseSettingsWithCommonAttributes
	{
		public ThrottlingPolicyPopSettings()
		{
		}

		private ThrottlingPolicyPopSettings(string value) : base(value)
		{
		}

		public static ThrottlingPolicyPopSettings Parse(string stateToParse)
		{
			return new ThrottlingPolicyPopSettings(stateToParse);
		}
	}
}
