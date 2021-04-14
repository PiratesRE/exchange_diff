using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.PowerShell.RbacHostingTools;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class AdminRoleGroupRow : BaseRow
	{
		public AdminRoleGroupRow(RoleGroup adminRoleGroupObject) : base(adminRoleGroupObject)
		{
			this.RoleGroup = adminRoleGroupObject;
			this.IsEditRoleGroupAllowed = (RbacPrincipal.Current.RbacConfiguration.IsCmdletAllowedInScope("Update-RoleGroupMember", new string[]
			{
				"Members"
			}, adminRoleGroupObject, ScopeLocation.RecipientWrite) && (!RbacPrincipal.Current.IsInRole("FFO") || !this.RoleGroup.Capabilities.Contains(Capability.Partner_Managed)));
		}

		public RoleGroup RoleGroup { get; set; }

		[DataMember]
		public string Name
		{
			get
			{
				return this.RoleGroup.Name;
			}
			set
			{
				this.RoleGroup.Name = value;
			}
		}

		[DataMember]
		public string RoleGroupObjectIdentity
		{
			get
			{
				return this.RoleGroup.DataObject.Identity.ToString();
			}
			set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public string DisplayName
		{
			get
			{
				return this.RoleGroup.DataObject.Name;
			}
			set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public bool IsEditRoleGroupAllowed { get; private set; }

		[DataMember]
		public bool IsCopyAllowed
		{
			get
			{
				return !this.IsMultipleScopesScenario && this.IsEditRoleGroupAllowed && !this.RoleGroup.IsReadOnly;
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public bool IsMultipleScopesScenario
		{
			get
			{
				return this.RoleAssignments != null && this.RoleAssignments.HasMultipleScopeTypes();
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		public IEnumerable<RoleAssignmentObjectResolverRow> RoleAssignments
		{
			get
			{
				if (this.roleAssignments == null)
				{
					IEnumerable<RoleAssignmentObjectResolverRow> source = RoleAssignmentObjectResolver.Instance.ResolveObjects(this.RoleGroup.RoleAssignments);
					this.roleAssignments = from entry in source
					where !entry.IsDelegating
					select entry;
				}
				return this.roleAssignments;
			}
		}

		private IEnumerable<RoleAssignmentObjectResolverRow> roleAssignments;
	}
}
