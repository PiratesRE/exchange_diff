using System;
using System.Management.Automation;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[ObjectScope(new ConfigScopes[]
	{
		ConfigScopes.TenantLocal,
		ConfigScopes.TenantSubTree
	})]
	[Serializable]
	public sealed class OnPremisesOrganization : ADConfigurationObject
	{
		internal override void InitializeSchema()
		{
		}

		internal override ADObjectSchema Schema
		{
			get
			{
				return OnPremisesOrganization.schema;
			}
		}

		internal override string MostDerivedObjectClass
		{
			get
			{
				return OnPremisesOrganization.mostDerivedClass;
			}
		}

		internal override QueryFilter ImplicitFilter
		{
			get
			{
				return new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.ObjectCategory, this.MostDerivedObjectClass);
			}
		}

		internal override ADObjectId ParentPath
		{
			get
			{
				return OnPremisesOrganization.rootId;
			}
		}

		public Guid OrganizationGuid
		{
			get
			{
				return (Guid)this[OnPremisesOrganizationSchema.OrganizationGuid];
			}
			set
			{
				this[OnPremisesOrganizationSchema.OrganizationGuid] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MultiValuedProperty<SmtpDomain> HybridDomains
		{
			get
			{
				return (MultiValuedProperty<SmtpDomain>)this[OnPremisesOrganizationSchema.HybridDomains];
			}
			set
			{
				this[OnPremisesOrganizationSchema.HybridDomains] = value;
			}
		}

		public ADObjectId InboundConnector
		{
			get
			{
				return (ADObjectId)this[OnPremisesOrganizationSchema.InboundConnectorLink];
			}
			set
			{
				this[OnPremisesOrganizationSchema.InboundConnectorLink] = value;
			}
		}

		public ADObjectId OutboundConnector
		{
			get
			{
				return (ADObjectId)this[OnPremisesOrganizationSchema.OutboundConnectorLink];
			}
			set
			{
				this[OnPremisesOrganizationSchema.OutboundConnectorLink] = value;
			}
		}

		public ADObjectId OrganizationRelationship
		{
			get
			{
				return (ADObjectId)this[OnPremisesOrganizationSchema.OrganizationRelationshipLink];
			}
			set
			{
				this[OnPremisesOrganizationSchema.OrganizationRelationshipLink] = value;
			}
		}

		public string OrganizationName
		{
			get
			{
				return (string)this[OnPremisesOrganizationSchema.OrganizationName];
			}
			set
			{
				this[OnPremisesOrganizationSchema.OrganizationName] = value;
			}
		}

		private static OnPremisesOrganizationSchema schema = ObjectSchema.GetInstance<OnPremisesOrganizationSchema>();

		private static string mostDerivedClass = "msExchOnPremisesOrganization";

		private static ADObjectId rootId = new ADObjectId("CN=On-Premises Organization");
	}
}
