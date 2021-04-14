using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Web;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[KnownType(typeof(AdminRoleGroupObject))]
	[DataContract]
	public class AdminRoleGroupObject : AdminRoleGroupRow
	{
		public AdminRoleGroupObject(RoleGroup roleGroup) : base(roleGroup)
		{
		}

		[DataMember]
		public string Description
		{
			get
			{
				return base.RoleGroup.Description;
			}
			set
			{
				base.RoleGroup.Description = value;
			}
		}

		[DataMember]
		public string[] ManagedBy
		{
			get
			{
				List<string> list = new List<string>();
				foreach (ADObjectId adobjectId in base.RoleGroup.ManagedBy)
				{
					list.Add(adobjectId.ToString());
				}
				return list.ToArray();
			}
		}

		[DataMember]
		public string ManagementScopeName
		{
			get
			{
				if (ManagementScopeRow.IsDefaultScope(this.AggregatedScope.ID))
				{
					return Strings.DefaultScope;
				}
				if (ManagementScopeRow.IsMultipleScope(this.AggregatedScope.ID))
				{
					return Strings.MultipleScopeInRoleGroup;
				}
				return HttpUtility.HtmlEncode(this.AggregatedScope.ID);
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public AggregatedScope AggregatedScope
		{
			get
			{
				if (this.aggregatedScope == null)
				{
					this.aggregatedScope = new AggregatedScope();
					this.aggregatedScope.IsOrganizationalUnit = false;
					IEnumerable<RoleAssignmentObjectResolverRow> roleAssignments = base.RoleAssignments;
					if (base.IsMultipleScopesScenario)
					{
						this.aggregatedScope.ID = string.Empty;
					}
					else
					{
						foreach (RoleAssignmentObjectResolverRow roleAssignmentObjectResolverRow in roleAssignments)
						{
							if (roleAssignmentObjectResolverRow.CustomConfigWriteScope != null)
							{
								this.aggregatedScope.ID = roleAssignmentObjectResolverRow.CustomConfigWriteScope.Name;
								break;
							}
							if (roleAssignmentObjectResolverRow.CustomRecipientWriteScope != null)
							{
								this.aggregatedScope.ID = roleAssignmentObjectResolverRow.CustomRecipientWriteScope.Name;
								break;
							}
							if (roleAssignmentObjectResolverRow.RecipientOrganizationUnitScope != null)
							{
								this.aggregatedScope.IsOrganizationalUnit = true;
								this.aggregatedScope.ID = roleAssignmentObjectResolverRow.RecipientOrganizationUnitScope.ToString();
								break;
							}
						}
						if (this.aggregatedScope.ID == null)
						{
							this.aggregatedScope.ID = ManagementScopeRow.DefaultScopeId;
						}
					}
				}
				return this.aggregatedScope;
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public IList<SecurityPrincipalRow> Members
		{
			get
			{
				if (this.members == null)
				{
					this.members = new List<SecurityPrincipalRow>(SecurityPrincipalObjectResolver.Instance.ResolveObjects(base.RoleGroup.Members));
				}
				return this.members;
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public IEnumerable<ManagementRoleResolveRow> Roles
		{
			get
			{
				if (this.roleObjectIds == null)
				{
					this.InitializeRoles();
				}
				return ManagementRoleObjectResolver.Instance.ResolveObjects(this.roleObjectIds);
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		public IEnumerable<Identity> RoleIdentities
		{
			get
			{
				if (this.roleObjectIds == null)
				{
					this.InitializeRoles();
				}
				return this.roleObjectIds.ToIdentities();
			}
		}

		public string OrganizationalUnit
		{
			get
			{
				if (this.AggregatedScope.IsOrganizationalUnit)
				{
					return this.AggregatedScope.ID;
				}
				return Guid.Empty.ToString();
			}
		}

		public string ManagementScopeId
		{
			get
			{
				return this.AggregatedScope.ID;
			}
		}

		public bool IsOrganizationalUnit
		{
			get
			{
				return this.AggregatedScope.IsOrganizationalUnit;
			}
		}

		private void InitializeRoles()
		{
			Dictionary<string, ADObjectId> dictionary = new Dictionary<string, ADObjectId>();
			foreach (RoleAssignmentObjectResolverRow roleAssignmentObjectResolverRow in base.RoleAssignments)
			{
				dictionary[roleAssignmentObjectResolverRow.RoleIdentity.Name] = roleAssignmentObjectResolverRow.RoleIdentity;
			}
			this.roleObjectIds = dictionary.Values;
		}

		private AggregatedScope aggregatedScope;

		private List<SecurityPrincipalRow> members;

		private IEnumerable<ADObjectId> roleObjectIds;
	}
}
