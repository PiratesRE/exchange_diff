using System;
using Microsoft.Exchange.Data.Directory.Recipient;

namespace Microsoft.Exchange.Management.ControlPanel
{
	public class SetRoleGroupMembersParameter : BaseRoleGroupParameters
	{
		public Identity[] Members
		{
			get
			{
				return (Identity[])base[ADGroupSchema.Members];
			}
			set
			{
				base[ADGroupSchema.Members] = value.ToIdParameters();
			}
		}

		public override string AssociatedCmdlet
		{
			get
			{
				return "Update-RoleGroupMember";
			}
		}

		public override string RbacScope
		{
			get
			{
				return "@R:Organization";
			}
		}
	}
}
