using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Management.ControlPanel.WebControls;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class MailboxPlan : DropDownItemData
	{
		public MailboxPlan(MailboxPlan plan)
		{
			base.Text = plan.DisplayName;
			base.Value = plan.Name;
			base.Selected = plan.IsDefault;
			this.RoleAssignmentPolicy = plan.RoleAssignmentPolicy;
			this.MailboxPlanIndex = plan.MailboxPlanIndex;
		}

		internal ADObjectId RoleAssignmentPolicy { get; private set; }

		internal string MailboxPlanIndex { get; private set; }
	}
}
