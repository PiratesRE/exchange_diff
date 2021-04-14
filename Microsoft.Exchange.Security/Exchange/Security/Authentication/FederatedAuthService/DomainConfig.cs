using System;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Security.Authentication.FederatedAuthService
{
	internal class DomainConfig
	{
		public DomainConfig(string domain, LiveIdInstanceType instance, bool federated, string authURL, bool cache, LivePreferredProtocol protocol)
		{
			this.DomainName = domain;
			this.Instance = instance;
			this.IsFederated = federated;
			this.AuthURL = authURL;
			this.Cache = cache;
			this.PreferredProtocol = protocol;
			this.ClockSkew = TimeSpan.Zero;
			this.OrgId = OrganizationId.ForestWideOrgId;
		}

		public string DomainName;

		public LiveIdInstanceType Instance;

		public bool IsFederated;

		public string AuthURL;

		public bool Cache;

		public LivePreferredProtocol PreferredProtocol;

		public TimeSpan ClockSkew;

		public OrganizationId OrgId;

		public bool SyncedAD;

		public DateTime LastUpdateTime;

		public bool IsOutlookCom;
	}
}
