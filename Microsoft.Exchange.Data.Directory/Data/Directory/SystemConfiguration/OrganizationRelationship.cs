using System;
using Microsoft.Exchange.Net.WSTrust;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[ObjectScope(new ConfigScopes[]
	{
		ConfigScopes.TenantLocal,
		ConfigScopes.TenantSubTree
	})]
	[Serializable]
	public class OrganizationRelationship : ADConfigurationObject
	{
		public MultiValuedProperty<SmtpDomain> DomainNames
		{
			get
			{
				return (MultiValuedProperty<SmtpDomain>)this[OrganizationRelationshipSchema.DomainNames];
			}
			set
			{
				this[OrganizationRelationshipSchema.DomainNames] = value;
			}
		}

		public bool FreeBusyAccessEnabled
		{
			get
			{
				return (bool)this[OrganizationRelationshipSchema.FreeBusyAccessEnabled];
			}
			set
			{
				this[OrganizationRelationshipSchema.FreeBusyAccessEnabled] = value;
			}
		}

		public FreeBusyAccessLevel FreeBusyAccessLevel
		{
			get
			{
				return (FreeBusyAccessLevel)this[OrganizationRelationshipSchema.FreeBusyAccessLevel];
			}
			set
			{
				this[OrganizationRelationshipSchema.FreeBusyAccessLevel] = value;
			}
		}

		public ADObjectId FreeBusyAccessScope
		{
			get
			{
				return (ADObjectId)this[OrganizationRelationshipSchema.FreeBusyAccessScope];
			}
			set
			{
				this[OrganizationRelationshipSchema.FreeBusyAccessScope] = value;
			}
		}

		public bool MailboxMoveEnabled
		{
			get
			{
				return (bool)this[OrganizationRelationshipSchema.MailboxMoveEnabled];
			}
			set
			{
				this[OrganizationRelationshipSchema.MailboxMoveEnabled] = value;
			}
		}

		public bool DeliveryReportEnabled
		{
			get
			{
				return (bool)this[OrganizationRelationshipSchema.DeliveryReportEnabled];
			}
			set
			{
				this[OrganizationRelationshipSchema.DeliveryReportEnabled] = value;
			}
		}

		public bool MailTipsAccessEnabled
		{
			get
			{
				return (bool)this[OrganizationRelationshipSchema.MailTipsAccessEnabled];
			}
			set
			{
				this[OrganizationRelationshipSchema.MailTipsAccessEnabled] = value;
			}
		}

		public MailTipsAccessLevel MailTipsAccessLevel
		{
			get
			{
				return (MailTipsAccessLevel)this[OrganizationRelationshipSchema.MailTipsAccessLevel];
			}
			set
			{
				this[OrganizationRelationshipSchema.MailTipsAccessLevel] = value;
			}
		}

		public ADObjectId MailTipsAccessScope
		{
			get
			{
				return (ADObjectId)this[OrganizationRelationshipSchema.MailTipsAccessScope];
			}
			set
			{
				this[OrganizationRelationshipSchema.MailTipsAccessScope] = value;
			}
		}

		public bool PhotosEnabled
		{
			get
			{
				return (bool)this[OrganizationRelationshipSchema.PhotosEnabled];
			}
			set
			{
				this[OrganizationRelationshipSchema.PhotosEnabled] = value;
			}
		}

		public Uri TargetApplicationUri
		{
			get
			{
				return (Uri)this[OrganizationRelationshipSchema.TargetApplicationUri];
			}
			set
			{
				this[OrganizationRelationshipSchema.TargetApplicationUri] = value;
			}
		}

		public Uri TargetSharingEpr
		{
			get
			{
				return (Uri)this[OrganizationRelationshipSchema.TargetSharingEpr];
			}
			set
			{
				this[OrganizationRelationshipSchema.TargetSharingEpr] = value;
			}
		}

		public Uri TargetOwaURL
		{
			get
			{
				return (Uri)this[OrganizationRelationshipSchema.TargetOwaURL];
			}
			set
			{
				this[OrganizationRelationshipSchema.TargetOwaURL] = value;
			}
		}

		public Uri TargetAutodiscoverEpr
		{
			get
			{
				return (Uri)this[OrganizationRelationshipSchema.TargetAutodiscoverEpr];
			}
			set
			{
				this[OrganizationRelationshipSchema.TargetAutodiscoverEpr] = value;
			}
		}

		public SmtpAddress OrganizationContact
		{
			get
			{
				return (SmtpAddress)this[OrganizationRelationshipSchema.OrganizationContact];
			}
			set
			{
				this[OrganizationRelationshipSchema.OrganizationContact] = value;
			}
		}

		public bool Enabled
		{
			get
			{
				return (bool)this[OrganizationRelationshipSchema.Enabled];
			}
			set
			{
				this[OrganizationRelationshipSchema.Enabled] = value;
			}
		}

		public bool ArchiveAccessEnabled
		{
			get
			{
				return (bool)this[OrganizationRelationshipSchema.ArchiveAccessEnabled];
			}
			set
			{
				this[OrganizationRelationshipSchema.ArchiveAccessEnabled] = value;
			}
		}

		internal override ADObjectSchema Schema
		{
			get
			{
				return OrganizationRelationship.SchemaObject;
			}
		}

		internal override string MostDerivedObjectClass
		{
			get
			{
				return "msExchFedSharingRelationship";
			}
		}

		internal override ADObjectId ParentPath
		{
			get
			{
				return FederatedOrganizationId.Container;
			}
		}

		internal override ExchangeObjectVersion MaximumSupportedExchangeObjectVersion
		{
			get
			{
				return ExchangeObjectVersion.Exchange2010;
			}
		}

		internal bool IsValidForRequestDispatcher()
		{
			return !(this.TargetApplicationUri == null) && (!(this.TargetAutodiscoverEpr == null) || !(this.TargetSharingEpr == null));
		}

		internal TokenTarget GetTokenTarget()
		{
			Uri targetApplicationUri = this.TargetApplicationUri;
			if (targetApplicationUri == null)
			{
				throw new OrganizationRelationshipMissingTargetApplicationUriException();
			}
			return new TokenTarget(TokenTarget.Fix(targetApplicationUri));
		}

		internal const string TaskNoun = "OrganizationRelationship";

		internal const string LdapName = "msExchFedSharingRelationship";

		private static readonly OrganizationRelationshipSchema SchemaObject = ObjectSchema.GetInstance<OrganizationRelationshipSchema>();
	}
}
