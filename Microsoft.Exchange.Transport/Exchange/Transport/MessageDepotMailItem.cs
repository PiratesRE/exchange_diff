using System;
using System.Globalization;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Transport.MessageDepot;

namespace Microsoft.Exchange.Transport
{
	internal class MessageDepotMailItem : IMessageDepotItem
	{
		public MessageDepotMailItem(TransportMailItem mailItem)
		{
			ArgumentValidator.ThrowIfNull("mailItem", mailItem);
			this.mailItem = mailItem;
			this.messageId = new TransportMessageId(this.mailItem.RecordId.ToString(CultureInfo.CurrentCulture));
		}

		public object MessageObject
		{
			get
			{
				return this.mailItem;
			}
		}

		public DateTime ArrivalTime
		{
			get
			{
				return this.mailItem.LatencyStartTime;
			}
		}

		public DateTime ExpirationTime
		{
			get
			{
				return this.mailItem.Expiry;
			}
		}

		public TransportMessageId Id
		{
			get
			{
				return this.messageId;
			}
		}

		public bool IsDelayDsnGenerated { get; set; }

		public bool IsSuspended
		{
			get
			{
				return false;
			}
		}

		public MessageDepotItemStage Stage
		{
			get
			{
				return MessageDepotItemStage.Submission;
			}
		}

		public bool IsPoison
		{
			get
			{
				return this.mailItem.IsPoison;
			}
		}

		public DateTime DeferUntil
		{
			get
			{
				return this.mailItem.DeferUntil;
			}
			set
			{
				this.mailItem.DeferUntil = value;
			}
		}

		public MessageEnvelope MessageEnvelope
		{
			get
			{
				return this.mailItem.GetMessageEnvelope();
			}
		}

		public void Dehydrate()
		{
		}

		public object GetProperty(string propertyName)
		{
			throw new NotImplementedException();
		}

		private readonly TransportMailItem mailItem;

		private readonly TransportMessageId messageId;
	}
}
