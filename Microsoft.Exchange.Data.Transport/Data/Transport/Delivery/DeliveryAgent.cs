using System;

namespace Microsoft.Exchange.Data.Transport.Delivery
{
	public abstract class DeliveryAgent : Agent
	{
		protected event OpenConnectionEventHandler OnOpenConnection
		{
			add
			{
				base.AddHandler("OnOpenConnection", value);
			}
			remove
			{
				base.RemoveHandler("OnOpenConnection");
			}
		}

		protected event DeliverMailItemEventHandler OnDeliverMailItem
		{
			add
			{
				base.AddHandler("OnDeliverMailItem", value);
			}
			remove
			{
				base.RemoveHandler("OnDeliverMailItem");
			}
		}

		protected event CloseConnectionEventHandler OnCloseConnection
		{
			add
			{
				base.AddHandler("OnCloseConnection", value);
			}
			remove
			{
				base.RemoveHandler("OnCloseConnection");
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
			this.Cleanup();
		}

		internal override void Invoke(string eventTopic, object source, object e)
		{
			Delegate @delegate = (Delegate)base.Handlers[eventTopic];
			if (@delegate == null)
			{
				return;
			}
			DeliverableMailItem mailItem = DeliveryAgent.GetMailItem(e);
			if (mailItem != null)
			{
				((SmtpServer)this.HostState).AddressBook.RecipientCache = mailItem.RecipientCache;
			}
			if (eventTopic != null)
			{
				if (!(eventTopic == "OnOpenConnection"))
				{
					if (!(eventTopic == "OnDeliverMailItem"))
					{
						if (!(eventTopic == "OnCloseConnection"))
						{
							goto IL_B4;
						}
						((CloseConnectionEventHandler)@delegate)((CloseConnectionEventSource)source, (CloseConnectionEventArgs)e);
					}
					else
					{
						((DeliverMailItemEventHandler)@delegate)((DeliverMailItemEventSource)source, (DeliverMailItemEventArgs)e);
					}
				}
				else
				{
					((OpenConnectionEventHandler)@delegate)((OpenConnectionEventSource)source, (OpenConnectionEventArgs)e);
				}
				if (base.Synchronous)
				{
					this.Cleanup();
				}
				return;
			}
			IL_B4:
			throw new InvalidOperationException("Internal error: unsupported event topic: " + (eventTopic ?? "null"));
		}

		private static DeliverableMailItem GetMailItem(object e)
		{
			OpenConnectionEventArgs openConnectionEventArgs = e as OpenConnectionEventArgs;
			if (openConnectionEventArgs != null)
			{
				return openConnectionEventArgs.DeliverableMailItem;
			}
			DeliverMailItemEventArgs deliverMailItemEventArgs = e as DeliverMailItemEventArgs;
			if (deliverMailItemEventArgs != null)
			{
				return deliverMailItemEventArgs.DeliverableMailItem;
			}
			return null;
		}

		private void Cleanup()
		{
			((SmtpServer)this.HostState).AddressBook.RecipientCache = null;
		}
	}
}
