using System;
using System.Management.Automation;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.StoreTasks
{
	[Cmdlet("Set", "MailboxRegionalConfiguration", SupportsShouldProcess = true, DefaultParameterSetName = "Identity")]
	public sealed class SetMailboxRegionalConfiguration : SetMailboxConfigurationTaskBase<MailboxRegionalConfiguration>
	{
		[Parameter(Mandatory = false)]
		public SwitchParameter LocalizeDefaultFolderName
		{
			get
			{
				return (SwitchParameter)(base.Fields["LocalizeDefaultFolderName"] ?? false);
			}
			set
			{
				base.Fields["LocalizeDefaultFolderName"] = value;
			}
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageMailboxRegionalConfiguration(this.Identity.ToString());
			}
		}

		protected override IConfigDataProvider CreateMailboxDataProvider(ADUser adUser)
		{
			MailboxStoreTypeProvider mailboxStoreTypeProvider = new MailboxStoreTypeProvider(adUser);
			ExchangePrincipal principal = ExchangePrincipal.FromADUser(base.SessionSettings, adUser, RemotingOptions.AllowCrossSite);
			mailboxStoreTypeProvider.MailboxSession = StoreTasksHelper.OpenMailboxSession(principal, "Set-MailboxConfiguration", this.LocalizeDefaultFolderName.IsPresent);
			return mailboxStoreTypeProvider;
		}

		protected override void InternalProcessRecord()
		{
			base.InternalProcessRecord();
			MailboxStoreTypeProvider mailboxStoreTypeProvider = null;
			try
			{
				mailboxStoreTypeProvider = (MailboxStoreTypeProvider)this.CreateSession();
				if (this.LocalizeDefaultFolderName.IsPresent)
				{
					Exception[] array;
					mailboxStoreTypeProvider.MailboxSession.LocalizeDefaultFolders(out array);
					if (array != null && array.Length > 0)
					{
						base.WriteError(new InvalidOperationException(Strings.ErrorCannotLocalizeDefaultFolders(this.Identity.ToString(), array[0].Message), array[0]), ErrorCategory.InvalidOperation, this.Identity);
					}
				}
				mailboxStoreTypeProvider.MailboxSession.SetMailboxLocale();
			}
			finally
			{
				if (mailboxStoreTypeProvider != null && mailboxStoreTypeProvider.MailboxSession != null)
				{
					mailboxStoreTypeProvider.MailboxSession.Dispose();
					mailboxStoreTypeProvider.MailboxSession = null;
				}
			}
		}

		protected override bool ReadUserFromDC
		{
			get
			{
				return true;
			}
		}

		private const string ParameterLocalizeDefaultFolderName = "LocalizeDefaultFolderName";
	}
}
