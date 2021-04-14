using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Transport.Categorizer
{
	internal class RoutingContext
	{
		public RoutingContext(TransportMailItem mailItem, RoutingTables routingTables, RoutingContextCore contextCore, TaskContext taskContext)
		{
			RoutingUtils.ThrowIfNull(mailItem, "mailItem");
			RoutingUtils.ThrowIfNull(routingTables, "routingTables");
			RoutingUtils.ThrowIfNull(contextCore, "contextCore");
			this.mailItem = mailItem;
			this.contextCore = contextCore;
			this.routingTables = routingTables;
			this.messageSize = -1L;
			this.taskContext = taskContext;
		}

		public TransportMailItem MailItem
		{
			get
			{
				return this.mailItem;
			}
		}

		public MailRecipient CurrentRecipient
		{
			get
			{
				return this.currentRecipient;
			}
			set
			{
				this.currentRecipient = value;
			}
		}

		public RoutingContextCore Core
		{
			get
			{
				return this.contextCore;
			}
		}

		public RoutingTables RoutingTables
		{
			get
			{
				return this.routingTables;
			}
		}

		public TaskContext TaskContext
		{
			get
			{
				return this.taskContext;
			}
		}

		public long MessageSize
		{
			get
			{
				if (this.messageSize < 0L)
				{
					this.messageSize = this.GetOriginalMessageSize();
					if (this.messageSize < 0L)
					{
						throw new InvalidOperationException("GetOriginalMessageSize() must not return a negative value: " + this.messageSize);
					}
				}
				return this.messageSize;
			}
		}

		public bool? ShouldMakeMailItemShadowed
		{
			get
			{
				return this.shouldMakeMailItemShadowed;
			}
			set
			{
				this.shouldMakeMailItemShadowed = value;
			}
		}

		public DateTime Timestamp
		{
			get
			{
				return this.routingTables.WhenCreated;
			}
		}

		public void RegisterCurrentRecipientForAction(Guid actionId, RoutingContext.ActionOnRecipients action)
		{
			RoutingContext.RecipientSetWithAction recipientSetWithAction = null;
			if (this.actions == null)
			{
				this.actions = new Dictionary<Guid, RoutingContext.RecipientSetWithAction>();
			}
			if (this.actions.TryGetValue(actionId, out recipientSetWithAction))
			{
				recipientSetWithAction.AddRecipient(this.currentRecipient);
				return;
			}
			recipientSetWithAction = new RoutingContext.RecipientSetWithAction(this.currentRecipient, action);
			this.actions.Add(actionId, recipientSetWithAction);
		}

		public void ExecuteActions()
		{
			if (this.actions == null)
			{
				return;
			}
			foreach (KeyValuePair<Guid, RoutingContext.RecipientSetWithAction> keyValuePair in this.actions)
			{
				keyValuePair.Value.ExecuteAction(keyValuePair.Key, this);
			}
		}

		private long GetOriginalMessageSize()
		{
			return ResolverMessage.GetOriginalMessageSize(this.mailItem.RootPart.Headers, this.mailItem.MimeSize);
		}

		private RoutingContextCore contextCore;

		private RoutingTables routingTables;

		private TransportMailItem mailItem;

		private MailRecipient currentRecipient;

		private long messageSize;

		private Dictionary<Guid, RoutingContext.RecipientSetWithAction> actions;

		private bool? shouldMakeMailItemShadowed;

		private TaskContext taskContext;

		internal delegate void ActionOnRecipients(Guid actionId, ICollection<MailRecipient> recipients, RoutingContext context);

		private class RecipientSetWithAction
		{
			public RecipientSetWithAction(MailRecipient recipient, RoutingContext.ActionOnRecipients action)
			{
				RoutingUtils.ThrowIfNull(recipient, "recipient");
				RoutingUtils.ThrowIfNull(action, "action");
				this.recipients = new List<MailRecipient>();
				this.recipients.Add(recipient);
				this.action = action;
			}

			public void AddRecipient(MailRecipient recipient)
			{
				RoutingUtils.ThrowIfNull(recipient, "recipient");
				this.recipients.Add(recipient);
			}

			public void ExecuteAction(Guid actionId, RoutingContext context)
			{
				this.action(actionId, this.recipients, context);
			}

			private List<MailRecipient> recipients;

			private RoutingContext.ActionOnRecipients action;
		}
	}
}
