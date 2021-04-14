using System;
using Microsoft.Exchange.Management.RbacTasks;

namespace Microsoft.Exchange.Management.ControlPanel
{
	public class SetManagementRoleAssignment : SetObjectProperties
	{
		public override string AssociatedCmdlet
		{
			get
			{
				return "Set-ManagementRoleAssignment";
			}
		}

		public override string RbacScope
		{
			get
			{
				return "@W:Organization";
			}
		}

		internal Identity OrganizationalUnit
		{
			get
			{
				return (Identity)base[RbacCommonParameters.ParameterRecipientOrganizationalUnitScope];
			}
			set
			{
				base[RbacCommonParameters.ParameterRecipientOrganizationalUnitScope] = value.ToIdParameter();
			}
		}

		internal Identity RecipientWriteScope
		{
			get
			{
				return (Identity)base[RbacCommonParameters.ParameterCustomRecipientWriteScope];
			}
			set
			{
				base[RbacCommonParameters.ParameterCustomRecipientWriteScope] = value.ToIdParameter();
			}
		}

		internal Identity ConfigWriteScope
		{
			get
			{
				return (Identity)base[RbacCommonParameters.ParameterCustomConfigWriteScope];
			}
			set
			{
				base[RbacCommonParameters.ParameterCustomConfigWriteScope] = value.ToIdParameter();
			}
		}

		internal string Name
		{
			get
			{
				return (string)base["Name"];
			}
			set
			{
				base["Name"] = value;
			}
		}
	}
}
