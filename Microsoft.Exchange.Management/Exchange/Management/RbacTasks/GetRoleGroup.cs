using System;
using System.Linq;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Management.Common;

namespace Microsoft.Exchange.Management.RbacTasks
{
	[Cmdlet("Get", "RoleGroup", DefaultParameterSetName = "Identity")]
	public sealed class GetRoleGroup : GetRecipientBase<RoleGroupIdParameter, ADGroup>
	{
		[Parameter]
		public SwitchParameter ShowPartnerLinked
		{
			get
			{
				return (SwitchParameter)(base.Fields["ShowPartnerLinked"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["ShowPartnerLinked"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public new long UsnForReconciliationSearch
		{
			get
			{
				return base.UsnForReconciliationSearch;
			}
			set
			{
				base.UsnForReconciliationSearch = value;
			}
		}

		[Parameter(ParameterSetName = "Identity")]
		[Parameter(ParameterSetName = "AnrSet")]
		public new SwitchParameter ReadFromDomainController
		{
			get
			{
				return base.ReadFromDomainController;
			}
			set
			{
				base.ReadFromDomainController = value;
			}
		}

		protected override ObjectId RootId
		{
			get
			{
				return this.rootId;
			}
		}

		protected override QueryFilter InternalFilter
		{
			get
			{
				QueryFilter queryFilter = RoleGroupIdParameter.GetRoleGroupFilter(base.InternalFilter);
				if (!this.ShowPartnerLinked)
				{
					queryFilter = QueryFilter.AndTogether(new QueryFilter[]
					{
						new ComparisonFilter(ComparisonOperator.NotEqual, ADGroupSchema.RoleGroupType, RoleGroupType.PartnerLinked),
						queryFilter
					});
				}
				return queryFilter;
			}
		}

		protected override PropertyDefinition[] SortProperties
		{
			get
			{
				return GetRoleGroup.SortPropertiesArray;
			}
		}

		protected override RecipientType[] RecipientTypes
		{
			get
			{
				return GetRoleGroup.RecipientTypesArray;
			}
		}

		protected override RecipientTypeDetails[] InternalRecipientTypeDetails
		{
			get
			{
				return GetRoleGroup.AllowedRecipientTypeDetails;
			}
		}

		internal override ObjectSchema FilterableObjectSchema
		{
			get
			{
				return ObjectSchema.GetInstance<RoleGroupSchema>();
			}
		}

		internal new OrganizationalUnitIdParameter OrganizationalUnit
		{
			get
			{
				return null;
			}
		}

		internal new string Anr
		{
			get
			{
				return null;
			}
		}

		internal new SwitchParameter IgnoreDefaultScope
		{
			get
			{
				return new SwitchParameter(false);
			}
		}

		internal new PSCredential Credential
		{
			get
			{
				return null;
			}
		}

		protected override void InternalValidate()
		{
			if (this.Identity == null)
			{
				if (base.CurrentOrganizationId == OrganizationId.ForestWideOrgId)
				{
					this.rootId = RoleGroupCommon.RoleGroupContainerId(base.TenantGlobalCatalogSession, this.ConfigurationSession);
				}
			}
			else
			{
				IRecipientSession tenantOrRootOrgRecipientSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(base.ServerSettings.PreferredGlobalCatalog(base.TenantGlobalCatalogSession.SessionSettings.PartitionId.ForestFQDN), true, ConsistencyMode.PartiallyConsistent, base.NetCredential, ADSessionSettings.FromAccountPartitionRootOrgScopeSet(base.TenantGlobalCatalogSession.SessionSettings.PartitionId), 203, "InternalValidate", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\RBAC\\RoleGroup\\GetRoleGroup.cs");
				base.OptionalIdentityData.RootOrgDomainContainerId = RoleGroupCommon.RoleGroupContainerId(tenantOrRootOrgRecipientSession, this.ConfigurationSession);
			}
			base.InternalValidate();
		}

		protected override IConfigurable ConvertDataObjectToPresentationObject(IConfigurable dataObject)
		{
			ADGroup adgroup = (ADGroup)dataObject;
			if (null != adgroup.ForeignGroupSid)
			{
				adgroup.LinkedGroup = SecurityPrincipalIdParameter.GetFriendlyUserName(adgroup.ForeignGroupSid, new Task.TaskVerboseLoggingDelegate(base.WriteVerbose));
				adgroup.ResetChangeTracking();
			}
			RoleGroup roleGroup = RoleGroupCommon.PopulateRoleAssignmentsAndConvert(adgroup, this.ConfigurationSession);
			roleGroup.PopulateCapabilitiesProperty();
			return roleGroup;
		}

		protected override bool ShouldSkipPresentationObject(IConfigurable presentationObject)
		{
			if (base.ExchangeRunspaceConfig != null && base.ExchangeRunspaceConfig.IsDedicatedTenantAdmin)
			{
				RoleGroup roleGroup = presentationObject as RoleGroup;
				if (roleGroup != null)
				{
					return !roleGroup.Roles.Any((ADObjectId adObject) => adObject.Name.StartsWith("SSA_", StringComparison.OrdinalIgnoreCase));
				}
			}
			return false;
		}

		private ADObjectId rootId;

		private static readonly RecipientTypeDetails[] AllowedRecipientTypeDetails = new RecipientTypeDetails[]
		{
			RecipientTypeDetails.RoleGroup
		};

		private static readonly PropertyDefinition[] SortPropertiesArray = new PropertyDefinition[]
		{
			ADObjectSchema.Name,
			ADRecipientSchema.DisplayName
		};

		private static readonly RecipientType[] RecipientTypesArray = new RecipientType[]
		{
			RecipientType.Group
		};
	}
}
