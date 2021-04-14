using System;
using System.Globalization;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.ApplicationLogic;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Management.Powershell.Support
{
	internal abstract class MailboxFileStoreBase : RecipientObjectActionTask<MailboxIdParameter, ADUser>
	{
		[Parameter(Mandatory = true)]
		public OrganizationCapability OrganizationCapability { get; set; }

		[Parameter(Mandatory = true)]
		public string FolderName { get; set; }

		[Parameter(Mandatory = true)]
		public string FileSetId { get; set; }

		protected abstract void Process(MailboxSession mailboxSession, MailboxFileStore mailboxFileStore);

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter(new object[]
			{
				this.DataObject
			});
			ADUser localOrganizationMailbox = this.GetLocalOrganizationMailbox();
			if (localOrganizationMailbox != null)
			{
				MailboxFileStore mailboxFileStore = new MailboxFileStore(this.FolderName);
				ExchangePrincipal mailboxOwner = ExchangePrincipal.FromADUser(localOrganizationMailbox, null);
				using (MailboxSession mailboxSession = MailboxSession.OpenAsAdmin(mailboxOwner, CultureInfo.InvariantCulture, "Client=Management"))
				{
					this.Process(mailboxSession, mailboxFileStore);
				}
			}
			TaskLogger.LogExit();
		}

		private ADUser GetLocalOrganizationMailbox()
		{
			Server localServer = LocalServerCache.LocalServer;
			ADUser[] array = OrganizationMailbox.FindByOrganizationId(this.DataObject.OrganizationId, this.OrganizationCapability);
			foreach (ADUser aduser in array)
			{
				if (this.DataObject.Identity.Equals(aduser.Id))
				{
					string activeServerFqdn = OrganizationMailbox.GetActiveServerFqdn(aduser.Id);
					if (StringComparer.OrdinalIgnoreCase.Equals(localServer.Fqdn, activeServerFqdn))
					{
						return aduser;
					}
				}
			}
			base.WriteError(new LocalizedException(Strings.ErrorNoLocalOrganizationMailbox(this.DataObject.Identity.ToString())), ErrorCategory.ObjectNotFound, this.Identity);
			return null;
		}
	}
}
