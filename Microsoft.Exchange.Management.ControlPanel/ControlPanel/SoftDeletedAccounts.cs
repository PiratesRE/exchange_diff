using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Security.Permissions;
using Microsoft.Exchange.Configuration.Common.LocStrings;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.ControlPanel
{
	public sealed class SoftDeletedAccounts : Accounts, ISoftDeletedAccounts, IAccounts, IDataSourceService<MailboxFilter, MailboxRecipientRow, Account, SetAccount, NewAccount, RemoveAccount>, IEditListService<MailboxFilter, MailboxRecipientRow, Account, NewAccount, RemoveAccount>, IGetListService<MailboxFilter, MailboxRecipientRow>, INewObjectService<MailboxRecipientRow, NewAccount>, IRemoveObjectsService<RemoveAccount>, IEditObjectForListService<Account, SetAccount, MailboxRecipientRow>, IGetObjectService<Account>, IGetObjectForListService<MailboxRecipientRow>
	{
		[PrincipalPermission(SecurityAction.Demand, Role = "MultiTenant+Get-Mailbox?Identity@R:Organization")]
		public override PowerShellResults<RecoverAccount> GetObjectForNew(Identity identity)
		{
			PSCommand pscommand = new PSCommand().AddCommand("Get-Mailbox");
			pscommand.AddParameter("SoftDeletedmailbox");
			if (!(identity == null))
			{
				return base.GetObject<RecoverAccount>(pscommand, identity);
			}
			return new PowerShellResults<RecoverAccount>();
		}

		[PrincipalPermission(SecurityAction.Demand, Role = "MultiTenant+New-Mailbox?DisplayName&Password&Name&MicrosoftOnlineServicesID@W:Organization")]
		[PrincipalPermission(SecurityAction.Demand, Role = "MultiTenant+Undo-SoftDeletedMailbox@W:Organization")]
		public override PowerShellResults<MailboxRecipientRow> NewObject(NewAccount properties)
		{
			PowerShellResults<MailboxRecipientRow> powerShellResults = base.NewObject<MailboxRecipientRow, NewAccount>("Undo-SoftDeletedMailbox", properties);
			if (powerShellResults.SucceededWithValue)
			{
				if (powerShellResults.HasWarnings)
				{
					LocalizedString warningUnlicensedMailbox = Strings.WarningUnlicensedMailbox;
					if (powerShellResults.Warnings.Contains(warningUnlicensedMailbox))
					{
						List<string> list = powerShellResults.Warnings.ToList<string>();
						list.Remove(warningUnlicensedMailbox);
						powerShellResults.Warnings = list.ToArray();
					}
				}
				PowerShellResults<MailboxRecipientRow> objectForList = base.GetObjectForList(powerShellResults.Value.Identity);
				if (objectForList != null)
				{
					powerShellResults.Output = objectForList.Output;
				}
			}
			return powerShellResults;
		}

		private const string GetSoftDeletedObjectForNewRole_MultiTenant = "MultiTenant+Get-Mailbox?Identity@R:Organization";

		private const string UndoSoftDeletedMailboxRole_WLID = "MultiTenant+Undo-SoftDeletedMailbox@W:Organization";
	}
}
