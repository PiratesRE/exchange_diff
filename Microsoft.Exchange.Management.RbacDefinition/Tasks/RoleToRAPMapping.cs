using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Management.Tasks
{
	internal class RoleToRAPMapping
	{
		public RoleToRAPAssignmentDefinition[] Assignments { get; private set; }

		public RoleToRAPMapping(RoleToRAPAssignmentDefinition[] assignments)
		{
			this.Assignments = assignments;
		}

		public List<RoleToRAPAssignmentDefinition> GetRoleAssignments(List<string> enabledFeatures)
		{
			List<RoleToRAPAssignmentDefinition> list = new List<RoleToRAPAssignmentDefinition>();
			foreach (RoleToRAPAssignmentDefinition roleToRAPAssignmentDefinition in this.Assignments)
			{
				if (roleToRAPAssignmentDefinition.SatisfyCondition(enabledFeatures))
				{
					list.Add(roleToRAPAssignmentDefinition);
				}
			}
			return list;
		}
	}
}
