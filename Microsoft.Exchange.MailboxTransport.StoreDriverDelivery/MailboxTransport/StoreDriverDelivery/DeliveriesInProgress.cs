using System;
using System.Diagnostics;
using System.Xml.Linq;
using Microsoft.Exchange.Collections;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Transport;

namespace Microsoft.Exchange.MailboxTransport.StoreDriverDelivery
{
	internal sealed class DeliveriesInProgress
	{
		public DeliveriesInProgress(int capacity)
		{
			this.map = new SynchronizedDictionary<ulong, MailItemDeliver>(capacity);
		}

		public MailItemDeliver this[ulong sessionID]
		{
			get
			{
				return this.map[sessionID];
			}
			set
			{
				this.map[sessionID] = value;
			}
		}

		public void Remove(ulong sessionID)
		{
			this.map.Remove(sessionID);
		}

		public bool DetectHang(TimeSpan limit, out ulong sessionID, out MailItemDeliver mailItemDeliver)
		{
			bool hang = false;
			ulong hangSessionID = 0UL;
			MailItemDeliver hangMailItemDeliver = null;
			ExDateTime utcNow = ExDateTime.UtcNow;
			this.map.ForEach((MailItemDeliver perEntryMailItemDeliver) => !hang, delegate(ulong perEntrySessionID, MailItemDeliver perEntryMailItemDeliver)
			{
				if (default(ExDateTime) != perEntryMailItemDeliver.RecipientStartTime && limit < utcNow - perEntryMailItemDeliver.RecipientStartTime)
				{
					hang = true;
					hangSessionID = perEntrySessionID;
					hangMailItemDeliver = perEntryMailItemDeliver;
				}
			});
			sessionID = hangSessionID;
			mailItemDeliver = hangMailItemDeliver;
			return hang;
		}

		public XElement GetDiagnosticInfo()
		{
			XElement root = new XElement("CurrentDeliveries");
			this.map.ForEach(null, delegate(ulong sessionID, MailItemDeliver mailItemDeliver)
			{
				mailItemDeliver.Thread.Suspend();
				StackTrace content;
				try
				{
					content = new StackTrace(mailItemDeliver.Thread, true);
				}
				finally
				{
					mailItemDeliver.Thread.Resume();
				}
				XElement xelement = new XElement("Delivery");
				xelement.Add(new object[]
				{
					new XElement("ThreadID", mailItemDeliver.Thread.ManagedThreadId),
					new XElement("SessionID", sessionID),
					new XElement("CurrentRecipientDuration", (default(ExDateTime) == mailItemDeliver.RecipientStartTime) ? TimeSpan.Zero : (ExDateTime.UtcNow - mailItemDeliver.RecipientStartTime)),
					new XElement("MessageID", mailItemDeliver.MbxTransportMailItem.InternetMessageId),
					new XElement("Recipient", (mailItemDeliver.Recipient == null) ? null : mailItemDeliver.Recipient.Email.ToString()),
					new XElement("MDB", mailItemDeliver.MbxTransportMailItem.DatabaseName),
					new XElement("MailboxServer", StoreDriverDelivery.MailboxServerFqdn),
					new XElement("MessageSize", mailItemDeliver.MbxTransportMailItem.MimeSize),
					new XElement("RecipientCount", mailItemDeliver.MbxTransportMailItem.Recipients.Count),
					new XElement("TMIRecipientCount", mailItemDeliver.MbxTransportMailItem.MailItemRecipientCount),
					new XElement("Sender", mailItemDeliver.MbxTransportMailItem.MimeSender),
					new XElement("StackTrace", content),
					LatencyFormatter.GetDiagnosticInfo(mailItemDeliver.MbxTransportMailItem.LatencyTracker)
				});
				root.Add(xelement);
			});
			return root;
		}

		private SynchronizedDictionary<ulong, MailItemDeliver> map;
	}
}
