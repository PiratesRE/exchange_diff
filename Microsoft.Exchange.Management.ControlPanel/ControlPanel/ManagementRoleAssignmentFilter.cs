using System;
using Microsoft.Exchange.Management.RbacTasks;

namespace Microsoft.Exchange.Management.ControlPanel
{
	public class ManagementRoleAssignmentFilter : WebServiceParameters
	{
		public override string AssociatedCmdlet
		{
			get
			{
				return "Get-ManagementRoleAssignment";
			}
		}

		public override string RbacScope
		{
			get
			{
				return "@R:Organization";
			}
		}

		public bool Delegating
		{
			get
			{
				return (bool)base[RbacCommonParameters.ParameterDelegating];
			}
			set
			{
				base[RbacCommonParameters.ParameterDelegating] = value;
			}
		}

		public Identity Role
		{
			get
			{
				return Identity.FromIdParameter(base[RbacCommonParameters.ParameterRole]);
			}
			set
			{
				base[RbacCommonParameters.ParameterRole] = value.ToIdParameter();
			}
		}

		public Identity RoleAssignee
		{
			get
			{
				return Identity.FromIdParameter(base[RbacCommonParameters.ParameterRoleAssignee]);
			}
			set
			{
				base[RbacCommonParameters.ParameterRoleAssignee] = value.ToIdParameter();
			}
		}
	}
}
