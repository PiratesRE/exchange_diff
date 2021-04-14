using System;
using System.Management.Automation;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Management.ContactLinking
{
	[Cmdlet("Invoke", "ContactLinking")]
	public sealed class InvokeContactLinking : ContactLinkingBaseCmdLet
	{
		protected override string UserAgent
		{
			get
			{
				return "Client=Management;Action=Invoke-ContactLinking";
			}
		}

		protected override bool OwnsPerformanceTrackerLifeCycle
		{
			get
			{
				return false;
			}
		}

		internal override void ContactLinkingOperation(MailboxSession mailboxSession)
		{
			MailboxInfoForLinking mailboxInfo = MailboxInfoForLinking.CreateFromMailboxSession(mailboxSession);
			DirectoryPersonSearcher directoryPersonSearcher = new DirectoryPersonSearcher(mailboxSession.MailboxOwner);
			ContactStoreForContactLinking contactStoreForContactLinking = new ContactStoreForBulkContactLinking(mailboxSession, base.PerformanceTracker);
			ContactLinkingLogger logger = new ContactLinkingLogger("InvokeContactLinkingCmdLet", mailboxInfo);
			AutomaticLink automaticLink = new AutomaticLink(mailboxInfo, logger, base.PerformanceTracker, directoryPersonSearcher, contactStoreForContactLinking);
			automaticLink.LinkAllExistingContacts();
		}
	}
}
