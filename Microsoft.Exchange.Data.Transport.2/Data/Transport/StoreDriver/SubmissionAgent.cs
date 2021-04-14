using System;

namespace Microsoft.Exchange.Data.Transport.StoreDriver
{
	internal abstract class SubmissionAgent : StoreDriverAgent
	{
		protected event DemotedMessageEventHandler OnDemotedMessage
		{
			add
			{
				base.AddHandler("OnDemotedMessage", value);
			}
			remove
			{
				base.RemoveHandler("OnDemotedMessage");
			}
		}

		internal override string SnapshotPrefix
		{
			get
			{
				return "Submission";
			}
		}

		internal override void AsyncComplete()
		{
			base.AsyncComplete();
			base.EnsureMimeWriteStreamClosed();
			base.MailItem = null;
			base.EventArgId = null;
			base.SnapshotWriter = null;
		}

		internal override void Invoke(string eventTopic, object source, object e)
		{
			Delegate @delegate = (Delegate)base.Handlers[eventTopic];
			if (@delegate == null)
			{
				return;
			}
			StoreDriverEventSource source2 = (StoreDriverEventSource)source;
			StoreDriverSubmissionEventArgs storeDriverSubmissionEventArgs = (StoreDriverSubmissionEventArgs)e;
			base.MailItem = storeDriverSubmissionEventArgs.MailItem;
			if (eventTopic != null && eventTopic == "OnDemotedMessage")
			{
				((SmtpServer)this.HostState).AddressBook.RecipientCache = storeDriverSubmissionEventArgs.MailItem.RecipientCache;
				((DemotedMessageEventHandler)@delegate)(source2, storeDriverSubmissionEventArgs);
				if (base.Synchronous)
				{
					base.EnsureMimeWriteStreamClosed();
					((SmtpServer)this.HostState).AddressBook.RecipientCache = null;
					base.MailItem = null;
				}
				return;
			}
			throw new InvalidOperationException("Internal error: unsupported event topic: " + (eventTopic ?? "null"));
		}
	}
}
