using System;
using Microsoft.Exchange.Data.Transport.StoreDriver;

namespace Microsoft.Exchange.Data.Transport.StoreDriverDelivery
{
	internal abstract class StoreDriverDeliveryAgent : Agent
	{
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
			((SmtpServer)this.HostState).AddressBook.RecipientCache = null;
		}

		protected event InitializedMessageEventHandler OnInitializedMessage
		{
			add
			{
				base.AddHandler("OnInitializedMessage", value);
			}
			remove
			{
				base.RemoveHandler("OnInitializedMessage");
			}
		}

		protected event PromotedMessageEventHandler OnPromotedMessage
		{
			add
			{
				base.AddHandler("OnPromotedMessage", value);
			}
			remove
			{
				base.RemoveHandler("OnPromotedMessage");
			}
		}

		protected event CreatedMessageEventHandler OnCreatedMessage
		{
			add
			{
				base.AddHandler("OnCreatedMessage", value);
			}
			remove
			{
				base.RemoveHandler("OnCreatedMessage");
			}
		}

		protected event DeliveredMessageEventHandler OnDeliveredMessage
		{
			add
			{
				base.AddHandler("OnDeliveredMessage", value);
			}
			remove
			{
				base.RemoveHandler("OnDeliveredMessage");
			}
		}

		protected event CompletedMessageEventHandler OnCompletedMessage
		{
			add
			{
				base.AddHandler("OnCompletedMessage", value);
			}
			remove
			{
				base.RemoveHandler("OnCompletedMessage");
			}
		}

		internal override void Invoke(string eventTopic, object source, object e)
		{
			Delegate @delegate = (Delegate)base.Handlers[eventTopic];
			if (@delegate == null)
			{
				return;
			}
			StoreDriverEventSource source2 = (StoreDriverEventSource)source;
			StoreDriverDeliveryEventArgs storeDriverDeliveryEventArgs = (StoreDriverDeliveryEventArgs)e;
			((SmtpServer)this.HostState).AddressBook.RecipientCache = storeDriverDeliveryEventArgs.MailItem.RecipientCache;
			if (eventTopic != null)
			{
				if (!(eventTopic == "OnInitializedMessage"))
				{
					if (!(eventTopic == "OnPromotedMessage"))
					{
						if (!(eventTopic == "OnCreatedMessage"))
						{
							if (!(eventTopic == "OnDeliveredMessage"))
							{
								if (!(eventTopic == "OnCompletedMessage"))
								{
									goto IL_DA;
								}
								((CompletedMessageEventHandler)@delegate)(source2, storeDriverDeliveryEventArgs);
							}
							else
							{
								((DeliveredMessageEventHandler)@delegate)(source2, storeDriverDeliveryEventArgs);
							}
						}
						else
						{
							((CreatedMessageEventHandler)@delegate)(source2, storeDriverDeliveryEventArgs);
						}
					}
					else
					{
						((PromotedMessageEventHandler)@delegate)(source2, storeDriverDeliveryEventArgs);
					}
				}
				else
				{
					((InitializedMessageEventHandler)@delegate)(source2, storeDriverDeliveryEventArgs);
				}
				if (base.Synchronous)
				{
					((SmtpServer)this.HostState).AddressBook.RecipientCache = null;
				}
				return;
			}
			IL_DA:
			throw new InvalidOperationException("Internal error: unsupported event topic: " + (eventTopic ?? "null"));
		}
	}
}
