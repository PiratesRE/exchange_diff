using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.ControlPanel.WebControls;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	[KnownType(typeof(EndUserRoleRow))]
	public class EndUserRoleRow : GroupedCheckBoxTreeItem
	{
		[DataMember]
		public bool IsEndUserRole { get; private set; }

		[DataMember]
		public string MailboxPlanIndex { get; private set; }

		public EndUserRoleRow(ExchangeRole role) : base(role)
		{
			base.Name = role.Name;
			base.Description = role.Description;
			base.Group = role.RoleType.ToString();
			base.Parent = (role.IsRootRole ? null : role.RoleType.ToString());
			this.IsEndUserRole = role.IsEndUserRole;
			this.MailboxPlanIndex = role.MailboxPlanIndex;
		}
	}
}
