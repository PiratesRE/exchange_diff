using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Security
{
	internal class ExtendedProtectionConfig
	{
		public ExtendedProtectionConfig(int policySetting, HashSet<string> acceptedServiceSpns, bool extendedProtectionTlsTerminatedAtProxyScenario)
		{
			this.policySetting = (ExtendedProtectionPolicySetting)policySetting;
			if (this.policySetting != ExtendedProtectionPolicySetting.None && extendedProtectionTlsTerminatedAtProxyScenario && (acceptedServiceSpns == null || acceptedServiceSpns.Count == 0))
			{
				throw new ArgumentException("acceptedServiceSpns must not be empty if extendedProtectionTlsTerminatedAtProxyScenario is set to true");
			}
			this.acceptedServiceSpns = acceptedServiceSpns;
			this.extendedProtectionTlsTerminatedAtProxyScenario = extendedProtectionTlsTerminatedAtProxyScenario;
		}

		public ExtendedProtectionPolicySetting PolicySetting
		{
			get
			{
				return this.policySetting;
			}
		}

		public bool ExtendedProtectionTlsTerminatedAtProxyScenario
		{
			get
			{
				return this.extendedProtectionTlsTerminatedAtProxyScenario;
			}
		}

		public bool IsValidTargetName(string targetName)
		{
			return this.acceptedServiceSpns != null && this.acceptedServiceSpns.Contains(targetName);
		}

		private readonly ExtendedProtectionPolicySetting policySetting;

		private readonly bool extendedProtectionTlsTerminatedAtProxyScenario;

		private readonly HashSet<string> acceptedServiceSpns;

		public static readonly ExtendedProtectionConfig NoExtendedProtection = new ExtendedProtectionConfig(0, null, false);
	}
}
