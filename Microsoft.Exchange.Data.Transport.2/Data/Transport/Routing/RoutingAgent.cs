using System;
using System.Globalization;

namespace Microsoft.Exchange.Data.Transport.Routing
{
	public abstract class RoutingAgent : Agent
	{
		protected event SubmittedMessageEventHandler OnSubmittedMessage
		{
			add
			{
				base.AddHandler("OnSubmittedMessage", value);
			}
			remove
			{
				base.RemoveHandler("OnSubmittedMessage");
			}
		}

		protected event ResolvedMessageEventHandler OnResolvedMessage
		{
			add
			{
				base.AddHandler("OnResolvedMessage", value);
			}
			remove
			{
				base.RemoveHandler("OnResolvedMessage");
			}
		}

		protected event RoutedMessageEventHandler OnRoutedMessage
		{
			add
			{
				base.AddHandler("OnRoutedMessage", value);
			}
			remove
			{
				base.RemoveHandler("OnRoutedMessage");
			}
		}

		protected event CategorizedMessageEventHandler OnCategorizedMessage
		{
			add
			{
				base.AddHandler("OnCategorizedMessage", value);
			}
			remove
			{
				base.RemoveHandler("OnCategorizedMessage");
			}
		}

		internal override string SnapshotPrefix
		{
			get
			{
				return "Routing";
			}
		}

		internal override object HostState
		{
			get
			{
				return base.HostStateInternal;
			}
			set
			{
				base.HostStateInternal = value;
				((SmtpServer)base.HostStateInternal).AssociatedAgent = this;
			}
		}

		internal override void AsyncComplete()
		{
			base.EnsureMimeWriteStreamClosed();
			base.MailItem = null;
			((SmtpServer)this.HostState).AddressBook.RecipientCache = null;
			base.EventArgId = null;
			base.SnapshotWriter = null;
		}

		internal override void Invoke(string eventTopic, object source, object e)
		{
			QueuedMessageEventArgs queuedMessageEventArgs = (QueuedMessageEventArgs)e;
			base.MailItem = queuedMessageEventArgs.MailItem;
			if (queuedMessageEventArgs.MailItem.PipelineTracingEnabled && base.SnapshotEnabled)
			{
				base.SnapshotWriter = queuedMessageEventArgs.MailItem.SnapshotWriter;
				base.EventArgId = queuedMessageEventArgs.MailItem.InternalMessageId.ToString(CultureInfo.InvariantCulture);
				if (base.SnapshotWriter != null && eventTopic == "OnSubmittedMessage")
				{
					base.SnapshotWriter.WriteOriginalData(this.GetHashCode(), base.EventArgId, eventTopic, null, base.MailItem);
				}
			}
			Delegate @delegate = (Delegate)base.Handlers[eventTopic];
			if (@delegate == null)
			{
				base.EventArgId = null;
				base.SnapshotWriter = null;
				return;
			}
			if (base.SnapshotWriter != null)
			{
				base.SnapshotWriter.WritePreProcessedData(this.GetHashCode(), "Routing", base.EventArgId, eventTopic, base.MailItem);
			}
			if (eventTopic != null)
			{
				if (!(eventTopic == "OnSubmittedMessage"))
				{
					if (!(eventTopic == "OnResolvedMessage"))
					{
						if (!(eventTopic == "OnRoutedMessage"))
						{
							if (eventTopic == "OnCategorizedMessage")
							{
								this.SetRecipientCache(queuedMessageEventArgs);
								((CategorizedMessageEventHandler)@delegate)((CategorizedMessageEventSource)source, queuedMessageEventArgs);
							}
						}
						else
						{
							this.SetRecipientCache(queuedMessageEventArgs);
							((RoutedMessageEventHandler)@delegate)((RoutedMessageEventSource)source, queuedMessageEventArgs);
						}
					}
					else
					{
						this.SetRecipientCache(queuedMessageEventArgs);
						((ResolvedMessageEventHandler)@delegate)((ResolvedMessageEventSource)source, queuedMessageEventArgs);
					}
				}
				else
				{
					this.SetRecipientCache(queuedMessageEventArgs);
					((SubmittedMessageEventHandler)@delegate)((SubmittedMessageEventSource)source, queuedMessageEventArgs);
				}
			}
			if (base.Synchronous)
			{
				base.EnsureMimeWriteStreamClosed();
				if (base.SnapshotWriter != null)
				{
					base.SnapshotWriter.WriteProcessedData("Routing", base.EventArgId, eventTopic, base.Name, base.MailItem);
					base.EventArgId = null;
					base.SnapshotWriter = null;
				}
				((SmtpServer)this.HostState).AddressBook.RecipientCache = null;
				base.MailItem = null;
			}
		}

		private void SetRecipientCache(QueuedMessageEventArgs eventArgs)
		{
			((SmtpServer)this.HostState).AddressBook.RecipientCache = eventArgs.MailItem.RecipientCache;
		}

		private const string SnapshotFileNamePrefix = "Routing";
	}
}
