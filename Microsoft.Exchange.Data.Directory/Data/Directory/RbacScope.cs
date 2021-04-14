using System;
using System.Collections.Generic;
using System.Security.AccessControl;
using System.Security.Principal;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Security.Authorization;

namespace Microsoft.Exchange.Data.Directory
{
	[Serializable]
	internal class RbacScope : ADScope
	{
		public RbacScope(ScopeType scopeType, ManagementScope managementScope)
		{
			this.scopeType = scopeType;
			this.managementScope = managementScope;
			switch (scopeType)
			{
			case ScopeType.CustomRecipientScope:
			case ScopeType.CustomConfigScope:
			case ScopeType.PartnerDelegatedTenantScope:
			case ScopeType.ExclusiveRecipientScope:
			case ScopeType.ExclusiveConfigScope:
				if (managementScope == null)
				{
					throw new ArgumentNullException("managementScope");
				}
				if (managementScope.QueryFilter == null)
				{
					throw new ArgumentException("managementScope.QueryFilter");
				}
				return;
			}
			if (managementScope != null)
			{
				throw new ArgumentException("managementScope");
			}
		}

		public RbacScope(ScopeType scopeType, ADObjectId ouId)
		{
			this.scopeType = scopeType;
			this.ouId = ouId;
		}

		public RbacScope(ScopeType scopeType)
		{
			this.scopeType = scopeType;
		}

		public RbacScope(ScopeType scopeType, ISecurityAccessToken securityAccessToken)
		{
			this.scopeType = scopeType;
			this.securityAccessToken = securityAccessToken;
		}

		public RbacScope(ScopeType scopeType, ManagementScope managementScope, bool isFromEndUserRole) : this(scopeType, managementScope)
		{
			this.isFromEndUserRole = isFromEndUserRole;
		}

		public RbacScope(ScopeType scopeType, ADObjectId ouId, bool isFromEndUserRole) : this(scopeType, ouId)
		{
			this.isFromEndUserRole = isFromEndUserRole;
		}

		public RbacScope(ScopeType scopeType, bool isFromEndUserRole) : this(scopeType)
		{
			this.isFromEndUserRole = isFromEndUserRole;
		}

		public RbacScope(ScopeType scopeType, ISecurityAccessToken securityAccessToken, bool isFromEndUserRole) : this(scopeType, securityAccessToken)
		{
			this.isFromEndUserRole = isFromEndUserRole;
		}

		internal bool IsFromEndUserRole
		{
			get
			{
				return this.isFromEndUserRole;
			}
		}

		internal QueryFilter SelfFilter { get; set; }

		internal ADObjectId SelfRoot { get; set; }

		public ScopeType ScopeType
		{
			get
			{
				return this.scopeType;
			}
		}

		public ScopeRestrictionType ScopeRestrictionType
		{
			get
			{
				if (this.managementScope != null)
				{
					return this.managementScope.ScopeRestrictionType;
				}
				return ScopeRestrictionType.NotApplicable;
			}
		}

		public bool Exclusive
		{
			get
			{
				return this.scopeType == ScopeType.ExclusiveRecipientScope || this.scopeType == ScopeType.ExclusiveConfigScope;
			}
		}

		internal static bool IsScopeTypeSmaller(ScopeType a, ScopeType b)
		{
			return RbacScope.IsScopeTypeSmaller(a, b, false);
		}

		internal static bool IsScopeTypeSmaller(ScopeType a, ScopeType b, bool hiddenFromGAL)
		{
			switch (a)
			{
			case ScopeType.None:
				if (hiddenFromGAL)
				{
					return b != ScopeType.None && b != ScopeType.MyGAL;
				}
				return b != ScopeType.None;
			case ScopeType.NotApplicable:
			case ScopeType.Organization:
			case ScopeType.OrganizationConfig:
				return false;
			case ScopeType.MyGAL:
				if (hiddenFromGAL)
				{
					return b != ScopeType.None && b != ScopeType.MyGAL;
				}
				return b == ScopeType.Organization;
			case ScopeType.Self:
			case ScopeType.MyDirectReports:
			case ScopeType.MyDistributionGroups:
			case ScopeType.MyExecutive:
			case ScopeType.MailboxICanDelegate:
				return b == ScopeType.Organization || (!hiddenFromGAL && b == ScopeType.MyGAL);
			case ScopeType.OU:
			case ScopeType.CustomRecipientScope:
			case ScopeType.ExclusiveRecipientScope:
				return b == ScopeType.Organization;
			case ScopeType.CustomConfigScope:
			case ScopeType.PartnerDelegatedTenantScope:
			case ScopeType.ExclusiveConfigScope:
				return b == ScopeType.OrganizationConfig;
			default:
				return false;
			}
		}

		internal bool IsScopeTypeSmallerOrEqualThan(ScopeType scopeType)
		{
			return scopeType == this.scopeType || RbacScope.IsScopeTypeSmaller(this.scopeType, scopeType);
		}

		internal bool IsScopeTypeSmallerThan(ScopeType scopeType)
		{
			return this.IsScopeTypeSmallerThan(scopeType, false);
		}

		internal bool IsScopeTypeSmallerThan(ScopeType scopeType, bool hiddenFromGAL)
		{
			return RbacScope.IsScopeTypeSmaller(this.scopeType, scopeType, hiddenFromGAL);
		}

		internal void PopulateRootAndFilter(OrganizationId organizationId, IReadOnlyPropertyBag propertyBag)
		{
			if (this.Root != null || this.Filter != null)
			{
				return;
			}
			if (this.isFromEndUserRole && propertyBag == null)
			{
				throw new ArgumentNullException("propertyBag");
			}
			if (organizationId != null)
			{
				this.SelfRoot = organizationId.OrganizationalUnit;
			}
			if (propertyBag != null)
			{
				this.SelfFilter = new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.Id, propertyBag[ADObjectSchema.Id]);
			}
			switch (this.scopeType)
			{
			case ScopeType.None:
				this.Root = null;
				this.Filter = ADScope.NoObjectFilter;
				return;
			case ScopeType.NotApplicable:
				this.Root = null;
				this.Filter = null;
				return;
			case ScopeType.Organization:
				this.Root = organizationId.OrganizationalUnit;
				this.Filter = null;
				return;
			case ScopeType.MyGAL:
			{
				AddressBookBase globalAddressList = this.GetGlobalAddressList(organizationId);
				this.Root = organizationId.OrganizationalUnit;
				if (globalAddressList == null)
				{
					this.Filter = ADScope.NoObjectFilter;
					return;
				}
				this.Filter = new ComparisonFilter(ComparisonOperator.Equal, ADRecipientSchema.AddressListMembership, globalAddressList.Id);
				return;
			}
			case ScopeType.Self:
				this.Root = organizationId.OrganizationalUnit;
				this.Filter = new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.Id, propertyBag[ADObjectSchema.Id]);
				return;
			case ScopeType.MyDirectReports:
				this.Root = organizationId.OrganizationalUnit;
				this.Filter = new ComparisonFilter(ComparisonOperator.Equal, ADOrgPersonSchema.Manager, propertyBag[ADObjectSchema.Id]);
				return;
			case ScopeType.OU:
				this.Root = this.ouId;
				this.Filter = null;
				return;
			case ScopeType.CustomRecipientScope:
			case ScopeType.CustomConfigScope:
			case ScopeType.PartnerDelegatedTenantScope:
			case ScopeType.ExclusiveRecipientScope:
			case ScopeType.ExclusiveConfigScope:
				this.Root = this.managementScope.RecipientRoot;
				this.Filter = this.managementScope.QueryFilter;
				return;
			case ScopeType.MyDistributionGroups:
			{
				this.Root = organizationId.OrganizationalUnit;
				QueryFilter[] array = new QueryFilter[3];
				array[0] = new ComparisonFilter(ComparisonOperator.Equal, ADGroupSchema.ManagedBy, propertyBag[ADObjectSchema.Id]);
				array[1] = new ComparisonFilter(ComparisonOperator.Equal, ADGroupSchema.CoManagedBy, propertyBag[ADObjectSchema.Id]);
				array[2] = new CSharpFilter<IReadOnlyPropertyBag>(delegate(IReadOnlyPropertyBag obj)
				{
					ADGroup adgroup = obj as ADGroup;
					return adgroup != null && adgroup.IsExecutingUserGroupOwner;
				});
				this.Filter = new OrFilter(array);
				return;
			}
			case ScopeType.MyExecutive:
				break;
			case ScopeType.OrganizationConfig:
				this.Root = organizationId.ConfigurationUnit;
				this.Filter = null;
				return;
			case ScopeType.MailboxICanDelegate:
			{
				this.Root = organizationId.OrganizationalUnit;
				QueryFilter[] array2 = new QueryFilter[2];
				array2[0] = new ComparisonFilter(ComparisonOperator.Equal, ADRecipientSchema.MasterAccountSid, this.securityAccessToken.UserSid);
				array2[1] = new CSharpFilter<IReadOnlyPropertyBag>(delegate(IReadOnlyPropertyBag obj)
				{
					RawSecurityDescriptor rawSecurityDescriptor = ((ADObject)obj).ReadSecurityDescriptor();
					if (rawSecurityDescriptor != null)
					{
						using (AuthzContextHandle authzContext = AuthzAuthorization.GetAuthzContext(new SecurityIdentifier(this.securityAccessToken.UserSid), false))
						{
							bool[] array3 = AuthzAuthorization.CheckExtendedRights(authzContext, rawSecurityDescriptor, new Guid[]
							{
								WellKnownGuid.PersonalInfoPropSetGuid
							}, null, AccessMask.WriteProp);
							return array3[0];
						}
						return false;
					}
					return false;
				});
				this.Filter = new OrFilter(array2);
				return;
			}
			default:
				this.Root = null;
				this.Filter = ADScope.NoObjectFilter;
				break;
			}
		}

		private AddressBookBase GetGlobalAddressList(OrganizationId organizationId)
		{
			AddressBookBase globalAddressList;
			using (ClientSecurityContext clientSecurityContext = new ClientSecurityContext(this.securityAccessToken, AuthzFlags.AuthzSkipTokenGroups))
			{
				ADSessionSettings sessionSettings = ADSessionSettings.FromOrganizationIdWithoutRbacScopes(ADSystemConfigurationSession.GetRootOrgContainerIdForLocalForest(), organizationId, null, false);
				globalAddressList = AddressBookBase.GetGlobalAddressList(clientSecurityContext, DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(ConsistencyMode.IgnoreInvalid, sessionSettings, 512, "GetGlobalAddressList", "f:\\15.00.1497\\sources\\dev\\data\\src\\directory\\RbacScope.cs"), DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(null, true, ConsistencyMode.IgnoreInvalid, null, sessionSettings, ConfigScopes.TenantSubTree, 513, "GetGlobalAddressList", "f:\\15.00.1497\\sources\\dev\\data\\src\\directory\\RbacScope.cs"), true);
			}
			return globalAddressList;
		}

		internal bool IsPresentInCollection(IList<ADScope> collection)
		{
			for (int i = 0; i < collection.Count; i++)
			{
				RbacScope rbacScope = (RbacScope)collection[i];
				if (this.ScopeType == rbacScope.ScopeType && this.managementScope == rbacScope.managementScope && ADObjectId.Equals(this.ouId, rbacScope.ouId))
				{
					return true;
				}
			}
			return false;
		}

		private readonly bool isFromEndUserRole;

		private ScopeType scopeType;

		private ADObjectId ouId;

		private ManagementScope managementScope;

		private ISecurityAccessToken securityAccessToken;
	}
}
