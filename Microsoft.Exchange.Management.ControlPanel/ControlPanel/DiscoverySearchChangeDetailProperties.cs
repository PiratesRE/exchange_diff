using System;
using System.ServiceModel;
using System.Web.UI.WebControls;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Management.ControlPanel
{
	public class DiscoverySearchChangeDetailProperties : AuditLogChangeDetailProperties
	{
		protected override string DetailsPaneId
		{
			get
			{
				return "discoverySearchChangeDetailsPane";
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
				return powerShellResults.Output[0].SearchObjectName;
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
					string cmdletName = adminAuditLogDetailRow.AdminAuditLogEvent.CmdletName;
					table.Rows.Add(base.GetDetailRowForTable(adminAuditLogDetailRow.AdminAuditLogEvent.RunDate.Value.ToUniversalTime().UtcToUserDateTimeString()));
					table.Rows.Add(base.GetDetailRowForTable(adminAuditLogDetailRow.UserFriendlyCaller));
					table.Rows.Add(base.GetDetailRowForTable(this.GetLocalizedState(cmdletName, adminAuditLogDetailRow.AdminAuditLogEvent.CmdletParameters)));
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

		private string GetLocalizedState(string cmdlet, MultiValuedProperty<AdminAuditLogCmdletParameter> parameters)
		{
			if (cmdlet.Equals("New-MailboxSearch", StringComparison.InvariantCultureIgnoreCase))
			{
				string text = Strings.DiscoverySearchCreated;
				foreach (AdminAuditLogCmdletParameter adminAuditLogCmdletParameter in parameters)
				{
					if (adminAuditLogCmdletParameter.Name.Equals("InPlaceHoldEnabled", StringComparison.InvariantCultureIgnoreCase) && string.Equals(adminAuditLogCmdletParameter.Value, "true", StringComparison.InvariantCultureIgnoreCase))
					{
						text = text + " " + Strings.DiscoverySearchWithHold;
						break;
					}
				}
				return text;
			}
			if (cmdlet.Equals("Get-MailboxSearch", StringComparison.InvariantCultureIgnoreCase))
			{
				return Strings.DiscoverySearchRetrieved;
			}
			if (cmdlet.Equals("Remove-MailboxSearch", StringComparison.InvariantCultureIgnoreCase))
			{
				return Strings.DiscoverySearchDeleted;
			}
			if (cmdlet.Equals("Start-MailboxSearch", StringComparison.InvariantCultureIgnoreCase))
			{
				return Strings.DiscoverySearchStarted;
			}
			if (cmdlet.Equals("Stop-MailboxSearch", StringComparison.InvariantCultureIgnoreCase))
			{
				return Strings.DiscoverySearchStopped;
			}
			if (cmdlet.Equals("Set-MailboxSearch", StringComparison.InvariantCultureIgnoreCase))
			{
				foreach (AdminAuditLogCmdletParameter adminAuditLogCmdletParameter2 in parameters)
				{
					if (adminAuditLogCmdletParameter2.Name.Equals("InPlaceHoldEnabled", StringComparison.InvariantCultureIgnoreCase))
					{
						if (string.Equals(adminAuditLogCmdletParameter2.Value, "true", StringComparison.InvariantCultureIgnoreCase))
						{
							return Strings.DiscoverySearchHoldEnabled;
						}
						return Strings.DiscoverySearchHoldDisabled;
					}
				}
				return Strings.DiscoverySearchModified;
			}
			throw new FaultException(new ArgumentException("CmdletRun").Message);
		}
	}
}
