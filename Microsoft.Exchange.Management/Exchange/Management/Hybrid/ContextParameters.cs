using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Management.Hybrid
{
	internal class ContextParameters : IContextParameters
	{
		public ContextParameters()
		{
			this.dictionary = new Dictionary<string, object>();
		}

		public T Get<T>(string name)
		{
			object obj;
			if (!this.dictionary.TryGetValue(name, out obj))
			{
				obj = default(T);
			}
			return (T)((object)obj);
		}

		public void Set<T>(string name, T value)
		{
			this.dictionary[name] = value;
		}

		private Dictionary<string, object> dictionary;

		internal static class DefaultNames
		{
			public const string TenantCoexistenceDomain = "_hybridDomain";

			public const string HybridDomainList = "_hybridDomainList";

			public const string OnPremOrgRel = "_onPremOrgRel";

			public const string TenantOrgRel = "_tenantOrgRel";

			public const string OnPremAcceptedDomains = "_onPremAcceptedDomains";

			public const string TenantAcceptedDomains = "_tenantAcceptedDomains";

			public const string OnPremAcceptedTokenIssuerUris = "_onPremAcceptedTokenIssuerUris";

			public const string TenantAcceptedTokenIssuerUris = "_tenantAcceptedTokenIssuerUris";

			public const string ForceUpgrade = "_forceUpgrade";

			public const string SuppressOAuthWarning = "_suppressOAuthWarning";
		}
	}
}
