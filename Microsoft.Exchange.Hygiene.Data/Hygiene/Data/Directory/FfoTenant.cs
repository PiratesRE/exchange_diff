using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Hygiene.Data.Directory
{
	[ObjectScope(new ConfigScopes[]
	{
		ConfigScopes.TenantLocal,
		ConfigScopes.TenantSubTree
	})]
	[Serializable]
	internal class FfoTenant : ADObject
	{
		public override ObjectId Identity
		{
			get
			{
				return this.TenantId;
			}
		}

		internal ADObjectId TenantId
		{
			get
			{
				return this[ADObjectSchema.OrganizationalUnitRoot] as ADObjectId;
			}
			set
			{
				this[ADObjectSchema.OrganizationalUnitRoot] = value;
			}
		}

		internal string TenantName
		{
			get
			{
				return this[ADObjectSchema.RawName] as string;
			}
			set
			{
				this[ADObjectSchema.RawName] = value;
			}
		}

		internal IEnumerable<ADMiniDomain> VerifiedDomains
		{
			get
			{
				return this[FfoTenantSchema.VerifiedDomainsProp] as IEnumerable<ADMiniDomain>;
			}
			set
			{
				this[FfoTenantSchema.VerifiedDomainsProp] = value;
			}
		}

		internal IEnumerable<AssignedPlan> AssignedPlans
		{
			get
			{
				return this[FfoTenantSchema.AssignedPlansProp] as IEnumerable<AssignedPlan>;
			}
			set
			{
				this[FfoTenantSchema.AssignedPlansProp] = value;
			}
		}

		internal MultiValuedProperty<string> CompanyTags
		{
			get
			{
				return this[FfoTenantSchema.CompanyTagsProp] as MultiValuedProperty<string>;
			}
			set
			{
				this[FfoTenantSchema.CompanyTagsProp] = value;
			}
		}

		internal string C
		{
			get
			{
				return this[FfoTenantSchema.C] as string;
			}
			set
			{
				this[FfoTenantSchema.C] = value;
			}
		}

		internal string CompanyPartnership
		{
			get
			{
				return this[FfoTenantSchema.CompanyPartnershipProperty] as string;
			}
			set
			{
				this[FfoTenantSchema.CompanyPartnershipProperty] = value;
			}
		}

		internal string Description
		{
			get
			{
				return this[FfoTenantSchema.DescriptionProperty] as string;
			}
			set
			{
				this[FfoTenantSchema.DescriptionProperty] = value;
			}
		}

		internal string DisplayName
		{
			get
			{
				return this[FfoTenantSchema.DisplayName] as string;
			}
			set
			{
				this[FfoTenantSchema.DisplayName] = value;
			}
		}

		internal bool IsDirSyncRunning
		{
			get
			{
				return (bool)this[FfoTenantSchema.IsDirSyncRunning];
			}
			set
			{
				this[FfoTenantSchema.IsDirSyncRunning] = value;
			}
		}

		internal ResellerType ResellerType
		{
			get
			{
				return (ResellerType)this[FfoTenantSchema.ResellerTypeProperty];
			}
			set
			{
				this[FfoTenantSchema.ResellerTypeProperty] = value;
			}
		}

		internal string ServiceInstance
		{
			get
			{
				return this[FfoTenantSchema.ServiceInstanceProp] as string;
			}
			set
			{
				this[FfoTenantSchema.ServiceInstanceProp] = value;
			}
		}

		internal RmsoUpgradeStatus RmsoUpgradeStatus
		{
			get
			{
				return (RmsoUpgradeStatus)this[FfoTenantSchema.RmsoUpgradeStatusProp];
			}
			set
			{
				this[FfoTenantSchema.RmsoUpgradeStatusProp] = value;
			}
		}

		internal string SharepointTenantAdminUrl
		{
			get
			{
				return (string)this[FfoTenantSchema.SharepointTenantAdminUrl];
			}
			set
			{
				this[FfoTenantSchema.SharepointTenantAdminUrl] = value;
			}
		}

		internal string SharepointRootSiteUrl
		{
			get
			{
				return (string)this[FfoTenantSchema.SharepointRootSiteUrl];
			}
			set
			{
				this[FfoTenantSchema.SharepointRootSiteUrl] = value;
			}
		}

		internal string OdmsEndpointUrl
		{
			get
			{
				return (string)this[FfoTenantSchema.OdmsEndpointUrl];
			}
			set
			{
				this[FfoTenantSchema.OdmsEndpointUrl] = value;
			}
		}

		internal string MigratedTo
		{
			get
			{
				return this[FfoTenantSchema.MigratedToProp] as string;
			}
			set
			{
				this[FfoTenantSchema.MigratedToProp] = value;
			}
		}

		internal MultiValuedProperty<string> UnifiedPolicyPreReqState
		{
			get
			{
				return this[FfoTenantSchema.UnifiedPolicyPreReqState] as MultiValuedProperty<string>;
			}
			set
			{
				this[FfoTenantSchema.UnifiedPolicyPreReqState] = value;
			}
		}

		internal OrganizationStatus OrganizationStatus
		{
			get
			{
				return (OrganizationStatus)this[FfoTenantSchema.OrganizationStatusProp];
			}
			set
			{
				this[FfoTenantSchema.OrganizationStatusProp] = value;
			}
		}

		internal override ADObjectSchema Schema
		{
			get
			{
				return FfoTenant.schema;
			}
		}

		internal override string MostDerivedObjectClass
		{
			get
			{
				return FfoTenant.mostDerivedClass;
			}
		}

		internal override ExchangeObjectVersion MaximumSupportedExchangeObjectVersion
		{
			get
			{
				return ExchangeObjectVersion.Exchange2010;
			}
		}

		internal const string EmptyTagsPropertyValue = "(no tags available)";

		private static readonly FfoTenantSchema schema = ObjectSchema.GetInstance<FfoTenantSchema>();

		private static string mostDerivedClass = "FfoTenant";
	}
}
