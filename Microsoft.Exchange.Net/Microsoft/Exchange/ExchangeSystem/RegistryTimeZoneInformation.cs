using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.ExchangeSystem
{
	internal sealed class RegistryTimeZoneInformation
	{
		public RegistryTimeZoneInformation(string keyName, string displayName, string standardName, string daylightName, string muiStandardName, REG_TIMEZONE_INFO regInfo)
		{
			this.KeyName = keyName;
			this.DisplayName = displayName;
			this.StandardName = standardName;
			this.DaylightName = daylightName;
			this.MuiStandardName = muiStandardName;
			this.RegInfo = regInfo;
		}

		public IList<RegistryTimeZoneRule> Rules
		{
			get
			{
				if (this.rules == null)
				{
					this.rules = new List<RegistryTimeZoneRule>(2);
				}
				return this.rules;
			}
			internal set
			{
				this.rules = value;
			}
		}

		public readonly string KeyName = string.Empty;

		public readonly string DisplayName = string.Empty;

		public readonly string StandardName = string.Empty;

		public readonly string DaylightName = string.Empty;

		public readonly string MuiStandardName = string.Empty;

		public readonly REG_TIMEZONE_INFO RegInfo;

		private IList<RegistryTimeZoneRule> rules;
	}
}
