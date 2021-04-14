using System;
using System.Security.Permissions;
using Microsoft.Exchange.PowerShell.RbacHostingTools;

namespace Microsoft.Exchange.Management.ControlPanel.DataContracts
{
	public class MailboxPermissions : DataSourceService, IGetListService<GetMailboxPermissionParameters, MailboxPermissionsRow>
	{
		public static bool CanAccessMailboxOf(string smtpAddress)
		{
			if (!RbacPrincipal.Current.IsInRole(string.Format("{0}?Identity&User", "Get-MailboxPermission")))
			{
				return false;
			}
			if (string.Compare(smtpAddress, LocalSession.Current.ExecutingUserPrimarySmtpAddress.ToString(), true) == 0)
			{
				return true;
			}
			MailboxPermissions mailboxPermissions = new MailboxPermissions();
			PowerShellResults<MailboxPermissionsRow> list = mailboxPermissions.GetList(new GetMailboxPermissionParameters
			{
				Identity = new Identity(smtpAddress),
				User = Identity.FromExecutingUserId()
			}, null);
			return list.Output.Length == 1 && (list.Output[0].HasReadAccess || list.Output[0].HasFullAccess);
		}

		[PrincipalPermission(SecurityAction.Demand, Role = "Get-MailboxPermission@C:OrganizationConfig")]
		public PowerShellResults<MailboxPermissionsRow> GetList(GetMailboxPermissionParameters filter, SortOptions sort)
		{
			return base.GetList<MailboxPermissionsRow, GetMailboxPermissionParameters>(filter.AssociatedCmdlet, filter, sort);
		}

		private const string Noun = "MailboxPermission";

		private const string GetCmdlet = "Get-MailboxPermission";

		private const string ReadScope = "@C:OrganizationConfig";

		private const string GetListRole = "Get-MailboxPermission@C:OrganizationConfig";
	}
}
