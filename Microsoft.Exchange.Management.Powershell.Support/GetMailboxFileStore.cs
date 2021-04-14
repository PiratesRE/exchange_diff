using System;
using System.Management.Automation;
using Microsoft.Exchange.Data.ApplicationLogic;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Management.Powershell.Support
{
	[Cmdlet("Get", "MailboxFileStore")]
	internal sealed class GetMailboxFileStore : MailboxFileStoreBase
	{
		protected override void Process(MailboxSession mailboxSession, MailboxFileStore mailboxFileStore)
		{
			foreach (FileSetItem sendToPipeline in mailboxFileStore.GetAll(base.FileSetId, mailboxSession))
			{
				base.WriteObject(sendToPipeline);
			}
		}
	}
}
