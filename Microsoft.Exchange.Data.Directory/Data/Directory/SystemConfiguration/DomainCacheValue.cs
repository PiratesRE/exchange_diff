using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[Serializable]
	internal class DomainCacheValue
	{
		public OrganizationId OrganizationId
		{
			get
			{
				return this.organizationId;
			}
			internal set
			{
				this.organizationId = value;
			}
		}

		public LiveIdInstanceType? LiveIdInstanceType
		{
			get
			{
				return this.liveIdInstanceType;
			}
			internal set
			{
				this.liveIdInstanceType = value;
			}
		}

		public AuthenticationType? AuthenticationType { get; internal set; }

		internal DomainCacheValue()
		{
		}

		private OrganizationId organizationId;

		private LiveIdInstanceType? liveIdInstanceType;
	}
}
