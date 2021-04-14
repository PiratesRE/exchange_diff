using System;
using Microsoft.Exchange.Management.RbacTasks;

namespace Microsoft.Exchange.Management.ControlPanel
{
	public class NewManagementRoleAssignment : SetManagementRoleAssignment
	{
		public sealed override string AssociatedCmdlet
		{
			get
			{
				return "New-ManagementRoleAssignment";
			}
		}

		internal Identity Policy
		{
			get
			{
				return Identity.FromIdParameter(base[RbacCommonParameters.ParameterPolicy]);
			}
			set
			{
				base[RbacCommonParameters.ParameterPolicy] = value.ToIdParameter();
			}
		}

		internal Identity Role
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

		internal string SecurityGroup
		{
			get
			{
				return (string)base[RbacCommonParameters.ParameterSecurityGroup];
			}
			set
			{
				base[RbacCommonParameters.ParameterSecurityGroup] = value;
			}
		}
	}
}
