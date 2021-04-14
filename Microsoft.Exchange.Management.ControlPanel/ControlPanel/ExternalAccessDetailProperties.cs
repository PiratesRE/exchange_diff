using System;
using System.Text;
using System.Web.UI.WebControls;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Management.ControlPanel
{
	public class ExternalAccessDetailProperties : AuditLogChangeDetailProperties
	{
		protected override string DetailsPaneId
		{
			get
			{
				return "ExternalAccessDetailsPane";
			}
		}

		protected override string HeaderLabelId
		{
			get
			{
				return "lblExternalAccess";
			}
		}

		protected override string GetDetailsPaneHeader()
		{
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
					table.Rows.Add(base.GetDetailRowForTable(string.Format("<b>{0}</b>", Strings.AuditLogDateSDO)));
					table.Rows.Add(base.GetDetailRowForTable(adminAuditLogDetailRow.AdminAuditLogEvent.RunDate.Value.ToUniversalTime().UtcToUserDateTimeString()));
					table.Rows.Add(base.GetEmptyRowForTable());
					table.Rows.Add(base.GetDetailRowForTable(string.Format("<b>{0}</b>", Strings.AuditLogUserSDO)));
					table.Rows.Add(base.GetDetailRowForTable(adminAuditLogDetailRow.UserFriendlyCaller));
					table.Rows.Add(base.GetEmptyRowForTable());
					table.Rows.Add(base.GetDetailRowForTable(string.Format("<b>{0}</b>", Strings.AuditLogCmdletSDO)));
					table.Rows.Add(base.GetDetailRowForTable(adminAuditLogDetailRow.AdminAuditLogEvent.CmdletName));
					table.Rows.Add(base.GetEmptyRowForTable());
					table.Rows.Add(base.GetDetailRowForTable(string.Format("<b>{0}</b>", Strings.AuditLogCmdletParameters)));
					table.Rows.Add(base.GetDetailRowForTable(this.FormatParameters(adminAuditLogDetailRow.AdminAuditLogEvent.CmdletParameters)));
					table.Rows.Add(base.GetEmptyRowForTable());
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

		private string FormatParameters(MultiValuedProperty<AdminAuditLogCmdletParameter> parameters)
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (AdminAuditLogCmdletParameter adminAuditLogCmdletParameter in parameters)
			{
				stringBuilder.AppendFormat("{0} : {1}", adminAuditLogCmdletParameter.Name, adminAuditLogCmdletParameter.Value);
				stringBuilder.Append("<br>");
			}
			return stringBuilder.ToString();
		}
	}
}
