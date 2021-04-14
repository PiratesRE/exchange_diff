using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[ObjectScope(new ConfigScopes[]
	{
		ConfigScopes.TenantLocal,
		ConfigScopes.TenantSubTree
	})]
	[Serializable]
	public sealed class IPAllowListProviderConfig : MessageHygieneAgentConfig
	{
		public new string Name
		{
			get
			{
				return base.Name;
			}
		}

		internal override string MostDerivedObjectClass
		{
			get
			{
				return IPAllowListProviderConfig.mostDerivedClass;
			}
		}

		public const string CanonicalName = "IPAllowListProviderConfig";

		private static string mostDerivedClass = "msExchMessageHygieneIPAllowListProviderConfig";
	}
}
