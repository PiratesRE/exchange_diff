using System;
using System.Resources;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.ProvisioningCache;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[ObjectScope(new ConfigScopes[]
	{
		ConfigScopes.TenantLocal,
		ConfigScopes.TenantSubTree
	})]
	[Serializable]
	public class RoleAssignmentPolicy : MailboxPolicy, IProvisioningCacheInvalidation
	{
		internal static QueryFilter IsDefaultFilterBuilder(SinglePropertyFilter filter)
		{
			return ADObject.BoolFilterBuilder(filter, new BitMaskAndFilter(RoleAssignmentPolicySchema.Flags, 1UL));
		}

		internal static object DescriptionGetter(IPropertyBag propertyBag)
		{
			string text = (string)propertyBag[RoleAssignmentPolicySchema.RawDescription];
			if (text.StartsWith(RoleAssignmentPolicy.LocalizedRoleAssignmentPolicyDescriptionPrefix))
			{
				try
				{
					text = RoleAssignmentPolicy.resourceManager.GetString(text.Substring(1));
				}
				catch (MissingManifestResourceException)
				{
				}
			}
			return text;
		}

		internal static void DescriptionSetter(object value, IPropertyBag propertyBag)
		{
			propertyBag[RoleAssignmentPolicySchema.RawDescription] = value;
		}

		internal override ADObjectSchema Schema
		{
			get
			{
				return RoleAssignmentPolicy.schema;
			}
		}

		internal override string MostDerivedObjectClass
		{
			get
			{
				return RoleAssignmentPolicy.mostDerivedClass;
			}
		}

		internal override ExchangeObjectVersion MaximumSupportedExchangeObjectVersion
		{
			get
			{
				return RoleAssignmentPolicySchema.Exchange2009_R4;
			}
		}

		internal override bool CheckForAssociatedUsers()
		{
			QueryFilter filter = new AndFilter(new QueryFilter[]
			{
				new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.DistinguishedName, base.Id.DistinguishedName),
				new ExistsFilter(RoleAssignmentPolicySchema.AssociatedUsers)
			});
			RoleAssignmentPolicy[] array = base.Session.Find<RoleAssignmentPolicy>(null, QueryScope.SubTree, filter, null, 1);
			return array != null && array.Length > 0;
		}

		public override bool IsDefault
		{
			get
			{
				return (bool)this[RoleAssignmentPolicySchema.IsDefault];
			}
			set
			{
				this[RoleAssignmentPolicySchema.IsDefault] = value;
			}
		}

		public string Description
		{
			get
			{
				return (string)this[RoleAssignmentPolicySchema.Description];
			}
			set
			{
				this[RoleAssignmentPolicySchema.Description] = value;
			}
		}

		public MultiValuedProperty<ADObjectId> RoleAssignments
		{
			get
			{
				return (MultiValuedProperty<ADObjectId>)this[RoleAssignmentPolicySchema.RoleAssignments];
			}
			private set
			{
				this[RoleAssignmentPolicySchema.RoleAssignments] = value;
			}
		}

		public MultiValuedProperty<ADObjectId> AssignedRoles
		{
			get
			{
				return (MultiValuedProperty<ADObjectId>)this[RoleAssignmentPolicySchema.AssignedRoles];
			}
			private set
			{
				this[RoleAssignmentPolicySchema.AssignedRoles] = value;
			}
		}

		internal void PopulateRoles(Result<ExchangeRoleAssignment>[] roleAssignmentResults)
		{
			if (roleAssignmentResults != null)
			{
				foreach (Result<ExchangeRoleAssignment> result in roleAssignmentResults)
				{
					ExchangeRoleAssignment data = result.Data;
					this.RoleAssignments.Add(data.Id);
					if (data.Role != null && !this.AssignedRoles.Contains(data.Role))
					{
						this.AssignedRoles.Add(data.Role);
					}
				}
			}
		}

		bool IProvisioningCacheInvalidation.ShouldInvalidProvisioningCache(out OrganizationId orgId, out Guid[] keys)
		{
			return this.ShouldInvalidProvisioningCache(out orgId, out keys);
		}

		internal bool ShouldInvalidProvisioningCache(out OrganizationId orgId, out Guid[] keys)
		{
			orgId = null;
			keys = null;
			bool flag = false;
			if (base.OrganizationId == null)
			{
				return flag;
			}
			if (base.ObjectState == ObjectState.New || base.ObjectState == ObjectState.Deleted)
			{
				flag = true;
			}
			if (!flag && base.ObjectState == ObjectState.Changed && (base.IsChanged(RoleAssignmentPolicySchema.AssociatedUsers) || base.IsChanged(RoleAssignmentPolicySchema.Flags) || base.IsChanged(RoleAssignmentPolicySchema.IsDefault) || base.IsChanged(RoleAssignmentPolicySchema.RawDescription) || base.IsChanged(RoleAssignmentPolicySchema.Description) || base.IsChanged(RoleAssignmentPolicySchema.RoleAssignments) || base.IsChanged(RoleAssignmentPolicySchema.AssignedRoles)))
			{
				flag = true;
			}
			if (flag)
			{
				orgId = base.OrganizationId;
				keys = new Guid[2];
				keys[0] = CannedProvisioningCacheKeys.MailboxRoleAssignmentPolicyCacheKey;
				keys[1] = CannedProvisioningCacheKeys.DefaultRoleAssignmentPolicyId;
			}
			return flag;
		}

		public static readonly string DefaultRoleAssignmentPolicyName = "Default Role Assignment Policy";

		private static readonly string LocalizedRoleAssignmentPolicyDescriptionPrefix = "%RoleAssignmentPolicyDescription_";

		public static readonly string PrecannedRoleAssignmentPolicyDescription = RoleAssignmentPolicy.LocalizedRoleAssignmentPolicyDescriptionPrefix + "Default";

		private static ExchangeResourceManager resourceManager = ExchangeResourceManager.GetResourceManager("Microsoft.Exchange.Data.Directory.Strings", typeof(DirectoryStrings).Assembly);

		internal static readonly ADObjectId RdnContainer = new ADObjectId("CN=Policies,CN=RBAC");

		private static RoleAssignmentPolicySchema schema = ObjectSchema.GetInstance<RoleAssignmentPolicySchema>();

		private static string mostDerivedClass = "msExchRBACPolicy";
	}
}
