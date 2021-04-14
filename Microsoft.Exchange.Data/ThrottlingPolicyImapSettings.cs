using System;

namespace Microsoft.Exchange.Data
{
	internal class ThrottlingPolicyImapSettings : ThrottlingPolicyBaseSettingsWithCommonAttributes
	{
		public ThrottlingPolicyImapSettings()
		{
		}

		private ThrottlingPolicyImapSettings(string value) : base(value)
		{
		}

		public static ThrottlingPolicyImapSettings Parse(string stateToParse)
		{
			return new ThrottlingPolicyImapSettings(stateToParse);
		}
	}
}
