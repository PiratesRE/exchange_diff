using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.ControlPanel
{
	public class ManagementRoleAssignment : BaseRow
	{
		internal Identity RoleAssignmentId { get; private set; }

		internal ADObjectId Role { get; private set; }

		internal string Name { get; private set; }

		internal RoleAssignmentDelegationType DelegationType { get; private set; }

		public ManagementRoleAssignment(ExchangeRoleAssignmentPresentation assignment) : base(assignment.ToIdentity(), assignment)
		{
			this.Role = assignment.Role;
			this.RoleAssignmentId = assignment.ToIdentity();
			this.Name = assignment.Name;
			this.DelegationType = assignment.RoleAssignmentDelegationType;
		}
	}
}
