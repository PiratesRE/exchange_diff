using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[ObjectScope(new ConfigScopes[]
	{
		ConfigScopes.TenantLocal,
		ConfigScopes.TenantSubTree
	})]
	[Serializable]
	public sealed class IPAllowListConfig : MessageHygieneAgentConfig
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
				return "msExchMessageHygieneIPAllowListConfig";
			}
		}

		public const string CanonicalName = "IPAllowListConfig";

		private const string MostDerivedClass = "msExchMessageHygieneIPAllowListConfig";
	}
}
