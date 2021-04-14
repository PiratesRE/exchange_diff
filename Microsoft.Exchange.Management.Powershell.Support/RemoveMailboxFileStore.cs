using System;
using System.Management.Automation;
using Microsoft.Exchange.Data.ApplicationLogic;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Management.Powershell.Support
{
	[Cmdlet("Remove", "MailboxFileStore")]
	internal sealed class RemoveMailboxFileStore : MailboxFileStoreBase
	{
		protected override void Process(MailboxSession mailboxSession, MailboxFileStore mailboxFileStore)
		{
			mailboxFileStore.RemoveAll(base.FileSetId, mailboxSession);
		}
	}
}
