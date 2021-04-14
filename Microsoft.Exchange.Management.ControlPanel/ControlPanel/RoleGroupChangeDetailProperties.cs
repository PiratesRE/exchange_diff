using System;
using System.ServiceModel;
using System.Text;
using System.Web.Security.AntiXss;
using System.Web.UI.WebControls;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Management.ControlPanel
{
	public class RoleGroupChangeDetailProperties : AuditLogChangeDetailProperties
	{
		protected override string DetailsPaneId
		{
			get
			{
				return "roleGroupChangeDetailsPane";
			}
		}

		protected override string HeaderLabelId
		{
			get
			{
				return "lblRoleGroupChanged";
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
			if (powerShellResults != null && powerShellResults.Output != null && powerShellResults.Output.Length > 0 && powerShellResults.Succeeded)
			{
				Table table = new Table();
				int num = 0;
				while (num < powerShellResults.Output.Length && num < 500)
				{
					AdminAuditLogDetailRow adminAuditLogDetailRow = powerShellResults.Output[num];
					table.Rows.Add(base.GetDetailRowForTable(adminAuditLogDetailRow.AdminAuditLogEvent.RunDate.Value.ToUniversalTime().UtcToUserDateTimeString()));
					table.Rows.Add(base.GetDetailRowForTable(adminAuditLogDetailRow.UserFriendlyCaller));
					string text = null;
					string text2 = null;
					foreach (AdminAuditLogCmdletParameter adminAuditLogCmdletParameter in adminAuditLogDetailRow.AdminAuditLogEvent.CmdletParameters)
					{
						if (adminAuditLogCmdletParameter.Name.Equals("Members", StringComparison.InvariantCultureIgnoreCase) || adminAuditLogCmdletParameter.Name.Equals("Member", StringComparison.InvariantCultureIgnoreCase))
						{
							text = AuditHelper.MakeUserFriendly(adminAuditLogCmdletParameter.Value);
						}
						if (adminAuditLogCmdletParameter.Name.Equals("Roles", StringComparison.InvariantCultureIgnoreCase) || adminAuditLogCmdletParameter.Name.Equals("Role", StringComparison.InvariantCultureIgnoreCase))
						{
							text2 = adminAuditLogCmdletParameter.Value;
						}
					}
					if (adminAuditLogDetailRow.AdminAuditLogEvent.CmdletName.Equals("New-RoleGroup", StringComparison.InvariantCultureIgnoreCase) || adminAuditLogDetailRow.AdminAuditLogEvent.CmdletName.Equals("Remove-RoleGroup", StringComparison.InvariantCultureIgnoreCase))
					{
						table.Rows.Add(base.GetDetailRowForTable(this.GetLocalizedAction(adminAuditLogDetailRow.AdminAuditLogEvent.CmdletName)));
					}
					if (text != null)
					{
						StringBuilder stringBuilder = new StringBuilder();
						if (adminAuditLogDetailRow.AdminAuditLogEvent.CmdletName.Equals("Update-RoleGroupMember", StringComparison.InvariantCultureIgnoreCase))
						{
							stringBuilder.Append(Strings.UpdatedMembers);
						}
						else
						{
							stringBuilder.Append(Strings.AddedMembers);
						}
						stringBuilder.Append(AntiXssEncoder.HtmlEncode(text, false));
						table.Rows.Add(base.GetDetailRowForTable(stringBuilder.ToString()));
					}
					if (text2 != null)
					{
						StringBuilder stringBuilder2 = new StringBuilder();
						stringBuilder2.Append(Strings.AddedRoles);
						stringBuilder2.Append(text2);
						table.Rows.Add(base.GetDetailRowForTable(stringBuilder2.ToString()));
					}
					table.Rows.Add(base.GetEmptyRowForTable());
					num++;
				}
				if (table.Rows.Count > 0)
				{
					this.detailsPane.Controls.Add(table);
				}
			}
		}

		private string GetLocalizedAction(string cmdlet)
		{
			if (string.Equals(cmdlet, "Add-RoleGroupMember", StringComparison.InvariantCultureIgnoreCase))
			{
				return Strings.AddedMembers;
			}
			if (string.Equals(cmdlet, "Remove-RoleGroupMember", StringComparison.InvariantCultureIgnoreCase))
			{
				return Strings.RemovedMembers;
			}
			if (string.Equals(cmdlet, "Update-RoleGroupMember", StringComparison.InvariantCultureIgnoreCase))
			{
				return Strings.UpdatedMembers;
			}
			if (string.Equals(cmdlet, "New-RoleGroup", StringComparison.InvariantCultureIgnoreCase))
			{
				return Strings.CreatedRoleGroup;
			}
			if (string.Equals(cmdlet, "Remove-RoleGroup", StringComparison.InvariantCultureIgnoreCase))
			{
				return Strings.RemovedRoleGroup;
			}
			throw new FaultException(new ArgumentException("RoleGroupValue").Message);
		}

		private const string RoleGroupParameterMembers = "Members";

		private const string RoleGroupParameterMember = "Member";

		private const string RoleGroupParameterRoles = "Roles";

		private const string RoleGroupParameterRole = "Role";

		private const string AddRoleGroupMember = "Add-RoleGroupMember";

		private const string RemoveRoleGroupMember = "Remove-RoleGroupMember";

		private const string UpdateRoleGroupMember = "Update-RoleGroupMember";

		private const string NewRoleGroup = "New-RoleGroup";

		private const string RemoveRoleGroup = "Remove-RoleGroup";
	}
}
