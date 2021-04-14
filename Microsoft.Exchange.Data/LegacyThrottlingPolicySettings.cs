using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Data
{
	internal class LegacyThrottlingPolicySettings
	{
		private LegacyThrottlingPolicySettings(string value)
		{
			this.toString = value;
			this.settings = new Dictionary<string, string>();
			ThrottlingPolicyBaseSettings.InternalParse(value, this.settings);
		}

		internal bool TryGetValue(string key, out string value)
		{
			value = null;
			if (!this.settings.ContainsKey(key))
			{
				return false;
			}
			value = this.settings[key];
			return true;
		}

		public override string ToString()
		{
			return this.toString;
		}

		public static LegacyThrottlingPolicySettings Parse(string stateToParse)
		{
			return new LegacyThrottlingPolicySettings(stateToParse);
		}

		private Dictionary<string, string> settings;

		private readonly string toString;
	}
}
