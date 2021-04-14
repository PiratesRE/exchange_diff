using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[Serializable]
	internal class DomainProperties
	{
		public SmtpDomain SmtpDomain
		{
			get
			{
				return this.domain;
			}
			internal set
			{
				this.domain = value;
			}
		}

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

		internal DomainProperties(SmtpDomain domain)
		{
			this.domain = domain;
		}

		private SmtpDomain domain;

		private OrganizationId organizationId;

		private LiveIdInstanceType? liveIdInstanceType;
	}
}
