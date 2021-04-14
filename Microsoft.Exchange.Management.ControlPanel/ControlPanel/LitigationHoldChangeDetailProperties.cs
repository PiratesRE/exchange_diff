using System;
using System.ServiceModel;
using System.Web.UI.WebControls;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Management.ControlPanel
{
	public class LitigationHoldChangeDetailProperties : AuditLogChangeDetailProperties
	{
		protected override string DetailsPaneId
		{
			get
			{
				return "litigationHoldChangeDetailsPane";
			}
		}

		protected override string HeaderLabelId
		{
			get
			{
				return "lblMailboxChanged";
			}
		}

		protected override string GetDetailsPaneHeader()
		{
			PowerShellResults<AdminAuditLogDetailRow> powerShellResults = base.Results as PowerShellResults<AdminAuditLogDetailRow>;
			if (powerShellResults != null && powerShellResults.Output != null && powerShellResults.Output.Length > 0 && powerShellResults.Succeeded)
			{
				return powerShellResults.Output[0].UserFriendlyObjectSelected;
			}
			return string.Empty;
		}

		protected override void RenderChanges()
		{
			PowerShellResults<AdminAuditLogDetailRow> powerShellResults = base.Results as PowerShellResults<AdminAuditLogDetailRow>;
			if (powerShellResults != null && powerShellResults.Output != null && powerShellResults.Succeeded && powerShellResults.Output.Length > 0)
			{
				Table table = new Table();
				int num = 0;
				while (num < powerShellResults.Output.Length && num < 500)
				{
					AdminAuditLogDetailRow adminAuditLogDetailRow = powerShellResults.Output[num];
					foreach (AdminAuditLogCmdletParameter adminAuditLogCmdletParameter in adminAuditLogDetailRow.AdminAuditLogEvent.CmdletParameters)
					{
						if (adminAuditLogCmdletParameter.Name.Equals("LitigationHoldEnabled", StringComparison.InvariantCultureIgnoreCase))
						{
							table.Rows.Add(base.GetDetailRowForTable(adminAuditLogDetailRow.AdminAuditLogEvent.RunDate.Value.ToUniversalTime().UtcToUserDateTimeString()));
							table.Rows.Add(base.GetDetailRowForTable(adminAuditLogDetailRow.UserFriendlyCaller));
							table.Rows.Add(base.GetDetailRowForTable(this.GetLocalizedState(adminAuditLogCmdletParameter.Value)));
							table.Rows.Add(base.GetEmptyRowForTable());
							break;
						}
					}
					num++;
				}
				if (table.Rows.Count > 0)
				{
					this.detailsPane.Controls.Add(table);
					return;
				}
				table.Dispose();
			}
		}

		private string GetLocalizedState(string value)
		{
			if (string.Equals(value, "true", StringComparison.InvariantCultureIgnoreCase))
			{
				return Strings.LitigationHoldEnabled;
			}
			if (string.Equals(value, "false", StringComparison.InvariantCultureIgnoreCase))
			{
				return Strings.LitigationHoldDisabled;
			}
			throw new FaultException(new ArgumentException("LitigationHoldValue").Message);
		}
	}
}
