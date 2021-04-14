using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Data.Transport.Email;
using Microsoft.Exchange.Protocols.Smtp;
using Microsoft.Exchange.Transport;

namespace Microsoft.Exchange.MailboxTransport.StoreDriverDelivery
{
	internal class MbxMailItemWrapper : DeliverableMailItem
	{
		public MbxMailItemWrapper(MbxTransportMailItem mbxMailItem)
		{
			this.mailItem = mbxMailItem;
			this.recipients = new ReadOnlyMailRecipientCollectionWrapper(this.mailItem.GetRecipients(), mbxMailItem);
		}

		public override string OriginalAuthenticator
		{
			get
			{
				this.ThrowIfClosed();
				return this.mailItem.Auth;
			}
		}

		public override DateTime DateTimeReceived
		{
			get
			{
				this.ThrowIfClosed();
				return this.mailItem.DateReceived;
			}
		}

		public override string EnvelopeId
		{
			get
			{
				this.ThrowIfClosed();
				return this.mailItem.EnvId;
			}
		}

		public override RoutingAddress FromAddress
		{
			get
			{
				this.ThrowIfClosed();
				return this.mailItem.From;
			}
		}

		public override string OriginatingDomain
		{
			get
			{
				this.ThrowIfClosed();
				return this.mailItem.HeloDomain;
			}
		}

		public override EmailMessage Message
		{
			get
			{
				this.ThrowIfClosed();
				return this.mailItem.Message;
			}
		}

		public override long MimeStreamLength
		{
			get
			{
				this.ThrowIfClosed();
				return this.mailItem.MimeSize;
			}
		}

		public override ReadOnlyEnvelopeRecipientCollection Recipients
		{
			get
			{
				this.ThrowIfClosed();
				return this.recipients;
			}
		}

		public override DsnFormatRequested DsnFormatRequested
		{
			get
			{
				this.ThrowIfClosed();
				return EnumConverter.InternalToPublic(this.mailItem.DsnFormat);
			}
		}

		public override DeliveryPriority DeliveryPriority
		{
			get
			{
				return this.mailItem.Priority;
			}
		}

		public string PrioritizationReason
		{
			get
			{
				return this.mailItem.PrioritizationReason;
			}
		}

		public override DeliveryMethod InboundDeliveryMethod
		{
			get
			{
				this.ThrowIfClosed();
				return TransportMailItemWrapper.GetInboundDeliveryMethod(this.mailItem);
			}
		}

		public override bool MustDeliver
		{
			get
			{
				this.ThrowIfClosed();
				return this.mailItem.RetryDeliveryIfRejected;
			}
		}

		public override Guid TenantId
		{
			get
			{
				this.ThrowIfClosed();
				return TransportMailItemWrapper.GetTenantId(this.mailItem);
			}
		}

		public void AddAgentInfo(string agentName, string eventName, List<KeyValuePair<string, string>> data)
		{
			this.mailItem.AddAgentInfo(agentName, eventName, data);
		}

		internal override Guid SystemProbeId
		{
			get
			{
				this.ThrowIfClosed();
				return this.mailItem.SystemProbeId;
			}
		}

		internal override object RecipientCache
		{
			get
			{
				this.ThrowIfClosed();
				return this.mailItem.ADRecipientCache;
			}
		}

		public override Stream GetMimeReadStream()
		{
			this.ThrowIfClosed();
			return TransportMailItemWrapper.GetMimeReadStream(this.mailItem, ref this.openedReadStreams);
		}

		public void Close()
		{
			TransportMailItemWrapper.CloseStreams(ref this.openedReadStreams);
			this.mailItem = null;
			this.recipients = null;
		}

		public override bool TryGetListProperty<ItemT>(string name, out ReadOnlyCollection<ItemT> value)
		{
			this.ThrowIfClosed();
			return this.mailItem.ExtendedProperties.TryGetListValue<ItemT>(name, out value);
		}

		public override bool TryGetProperty<T>(string name, out T value)
		{
			this.ThrowIfClosed();
			return this.mailItem.ExtendedProperties.TryGetValue<T>(name, out value);
		}

		private void ThrowIfClosed()
		{
			if (this.mailItem == null || this.recipients == null)
			{
				throw new InvalidOperationException("The mail item is no longer available.");
			}
		}

		private MbxTransportMailItem mailItem;

		private ReadOnlyMailRecipientCollectionWrapper recipients;

		private List<Stream> openedReadStreams;
	}
}
