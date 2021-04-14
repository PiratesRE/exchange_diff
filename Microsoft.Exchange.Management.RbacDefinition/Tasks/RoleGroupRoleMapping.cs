using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Management.Tasks
{
	internal class RoleGroupRoleMapping
	{
		public string RoleGroup { get; private set; }

		public RoleAssignmentDefinition[] Assignments { get; private set; }

		public RoleGroupRoleMapping(string roleGroup, RoleAssignmentDefinition[] assignments)
		{
			this.RoleGroup = roleGroup;
			this.Assignments = assignments;
		}

		public List<RoleAssignmentDefinition> GetRolesAssignments(List<string> enabledFeatures)
		{
			List<RoleAssignmentDefinition> list = new List<RoleAssignmentDefinition>();
			foreach (RoleAssignmentDefinition roleAssignmentDefinition in this.Assignments)
			{
				if (roleAssignmentDefinition.SatisfyCondition(enabledFeatures))
				{
					list.Add(roleAssignmentDefinition);
				}
			}
			return list;
		}
	}
}
