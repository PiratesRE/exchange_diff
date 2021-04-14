using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class SetAdminRoleGroupParameter : BaseRoleGroupParameters
	{
		[DataMember]
		public Identity[] Members { get; set; }

		[DataMember]
		public Identity[] Roles { get; set; }

		public bool IsRolesModified
		{
			get
			{
				return this.Roles != null;
			}
		}

		public override string AssociatedCmdlet
		{
			get
			{
				return "Set-RoleGroup";
			}
		}
	}
}
