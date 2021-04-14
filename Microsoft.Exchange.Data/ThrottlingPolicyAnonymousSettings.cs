using System;

namespace Microsoft.Exchange.Data
{
	internal class ThrottlingPolicyAnonymousSettings : ThrottlingPolicyBaseSettingsWithCommonAttributes
	{
		public ThrottlingPolicyAnonymousSettings()
		{
		}

		private ThrottlingPolicyAnonymousSettings(string value) : base(value)
		{
		}

		public static ThrottlingPolicyAnonymousSettings Parse(string stateToParse)
		{
			return new ThrottlingPolicyAnonymousSettings(stateToParse);
		}
	}
}
