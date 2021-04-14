using System;
using System.Management.Automation;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Management;

namespace Microsoft.Exchange.Management.StoreTasks
{
	[Cmdlet("Get", "MailboxRegionalConfiguration")]
	public sealed class GetMailboxRegionalConfiguration : GetMailboxConfigurationTaskBase<MailboxRegionalConfiguration>
	{
		[Parameter(Mandatory = false)]
		public SwitchParameter VerifyDefaultFolderNameLanguage
		{
			get
			{
				return (SwitchParameter)(base.Fields["VerifyDefaultFolderNameLanguage"] ?? false);
			}
			set
			{
				base.Fields["VerifyDefaultFolderNameLanguage"] = value;
			}
		}

		protected override IConfigDataProvider CreateMailboxDataProvider(ADUser adUser)
		{
			MailboxStoreTypeProvider mailboxStoreTypeProvider = new MailboxStoreTypeProvider(adUser);
			ExchangePrincipal principal = ExchangePrincipal.FromADUser(base.SessionSettings, adUser, RemotingOptions.AllowCrossSite);
			mailboxStoreTypeProvider.MailboxSession = StoreTasksHelper.OpenMailboxSession(principal, "Get-MailboxConfiguration", this.VerifyDefaultFolderNameLanguage.IsPresent);
			return mailboxStoreTypeProvider;
		}

		protected override void WriteResult(IConfigurable dataObject)
		{
			if (this.VerifyDefaultFolderNameLanguage.IsPresent)
			{
				MailboxStoreTypeProvider mailboxStoreTypeProvider = (MailboxStoreTypeProvider)base.DataSession;
				((MailboxRegionalConfiguration)dataObject).DefaultFolderNameMatchingUserLanguage = mailboxStoreTypeProvider.MailboxSession.VerifyDefaultFolderLocalization();
			}
			base.WriteResult(dataObject);
		}

		protected override bool ReadUserFromDC
		{
			get
			{
				return true;
			}
		}

		private const string ParameterVerifyDefaultFolderNameLanguage = "VerifyDefaultFolderNameLanguage";
	}
}
