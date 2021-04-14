using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Directory.Management;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[KnownType(typeof(RoleGroupMembersRow))]
	public class RoleGroupMembersRow : BaseRow
	{
		public RoleGroupMembersRow(RoleGroup roleGroupObject)
		{
			this.RoleGroupObject = roleGroupObject;
		}

		protected RoleGroup RoleGroupObject { get; set; }
	}
}
