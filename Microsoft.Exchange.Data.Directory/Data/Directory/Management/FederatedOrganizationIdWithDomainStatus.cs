using System;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Data.Directory.Management
{
	[Serializable]
	public class FederatedOrganizationIdWithDomainStatus : ADPresentationObject
	{
		public FederatedOrganizationIdWithDomainStatus()
		{
		}

		public FederatedOrganizationIdWithDomainStatus(FederatedOrganizationId dataObject) : base(dataObject)
		{
		}

		public SmtpDomain AccountNamespace
		{
			get
			{
				return (SmtpDomain)this[FederatedOrganizationIdWithDomainStatusSchema.AccountNamespace];
			}
			internal set
			{
				this[FederatedOrganizationIdWithDomainStatusSchema.AccountNamespace] = value;
			}
		}

		public MultiValuedProperty<FederatedDomain> Domains
		{
			get
			{
				return this.domains;
			}
			internal set
			{
				this.domains = value;
			}
		}

		public SmtpDomain DefaultDomain
		{
			get
			{
				return this.defaultDomain;
			}
			internal set
			{
				this.defaultDomain = value;
			}
		}

		public bool Enabled
		{
			get
			{
				return (bool)this[FederatedOrganizationIdWithDomainStatusSchema.Enabled];
			}
			internal set
			{
				this[FederatedOrganizationIdWithDomainStatusSchema.Enabled] = value;
			}
		}

		public SmtpAddress OrganizationContact
		{
			get
			{
				return (SmtpAddress)this[FederatedOrganizationIdWithDomainStatusSchema.OrganizationContact];
			}
			internal set
			{
				this[FederatedOrganizationIdWithDomainStatusSchema.OrganizationContact] = value;
			}
		}

		public ADObjectId DelegationTrustLink
		{
			get
			{
				return this[FederatedOrganizationIdWithDomainStatusSchema.DelegationTrustLink] as ADObjectId;
			}
			internal set
			{
				this[FederatedOrganizationIdWithDomainStatusSchema.DelegationTrustLink] = value;
			}
		}

		internal override ADPresentationSchema PresentationSchema
		{
			get
			{
				return FederatedOrganizationIdWithDomainStatus.schema;
			}
		}

		private static FederatedOrganizationIdWithDomainStatusSchema schema = ObjectSchema.GetInstance<FederatedOrganizationIdWithDomainStatusSchema>();

		private MultiValuedProperty<FederatedDomain> domains;

		private SmtpDomain defaultDomain;
	}
}
