using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Microsoft.Exchange.Data.Mime;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Data.Transport.Email;
using Microsoft.Exchange.Data.Transport.Internal.MExRuntime;
using Microsoft.Exchange.Extensibility.Internal;
using Microsoft.Exchange.Protocols.Smtp;

namespace Microsoft.Exchange.Transport
{
	internal class TransportMailItemWrapper : MailItem, ITransportMailItemWrapperFacade
	{
		public TransportMailItemWrapper(TransportMailItem mailItem, IMExSession mexSession, bool canAddRecipients)
		{
			this.mailItem = mailItem;
			this.mexSession = mexSession;
			this.canAddRecipients = canAddRecipients;
			this.snapshotWriter = new MessageSnapshotWriter(this.mailItem.SnapshotWriterState);
			this.pipelineTracingEnabled = mailItem.PipelineTracingEnabled;
			this.pipelineTracingPath = mailItem.PipelineTracingPath;
		}

		public TransportMailItemWrapper(TransportMailItem mailItem, bool canAddRecipients) : this(mailItem, null, canAddRecipients)
		{
		}

		ITransportMailItemFacade ITransportMailItemWrapperFacade.TransportMailItem
		{
			get
			{
				return this.TransportMailItem;
			}
		}

		public override string OriginalAuthenticator
		{
			get
			{
				this.ThrowIfDeferred();
				return this.mailItem.Auth;
			}
			set
			{
				this.ThrowIfDeferred();
				this.mailItem.Auth = value;
			}
		}

		public override DateTime DateTimeReceived
		{
			get
			{
				this.ThrowIfDeferred();
				return this.mailItem.DateReceived;
			}
		}

		public override string EnvelopeId
		{
			get
			{
				this.ThrowIfDeferred();
				return this.mailItem.EnvId;
			}
			set
			{
				this.ThrowIfDeferred();
				this.mailItem.EnvId = value;
			}
		}

		public override RoutingAddress FromAddress
		{
			get
			{
				this.ThrowIfDeferred();
				return this.mailItem.From;
			}
			set
			{
				this.ThrowIfDeferred();
				this.mailItem.From = value;
			}
		}

		public override string OriginatingDomain
		{
			get
			{
				this.ThrowIfDeferred();
				return this.mailItem.HeloDomain;
			}
		}

		public override EmailMessage Message
		{
			get
			{
				this.ThrowIfDeferred();
				if (this.mailItem.ExposeMessage)
				{
					return this.mailItem.Message;
				}
				return null;
			}
		}

		public override long MimeStreamLength
		{
			get
			{
				this.ThrowIfDeferred();
				return this.mailItem.GetCurrrentMimeSize();
			}
		}

		public override IDictionary<string, object> Properties
		{
			get
			{
				this.ThrowIfDeferred();
				return this.mailItem.ExtendedPropertyDictionary;
			}
		}

		public override EnvelopeRecipientCollection Recipients
		{
			get
			{
				this.ThrowIfDeferred();
				if (this.recipients == null)
				{
					this.recipients = new MailRecipientCollectionWrapper(this.mailItem, this.mexSession, this.canAddRecipients);
				}
				return this.recipients;
			}
		}

		public override DsnFormatRequested DsnFormatRequested
		{
			get
			{
				this.ThrowIfDeferred();
				return EnumConverter.InternalToPublic(this.mailItem.DsnFormat);
			}
			set
			{
				this.ThrowIfDeferred();
				this.mailItem.DsnFormat = EnumConverter.PublicToInternal(value);
			}
		}

		public override DeliveryPriority DeliveryPriority
		{
			get
			{
				this.ThrowIfDeferred();
				return ((IQueueItem)this.mailItem).Priority;
			}
			set
			{
				this.ThrowIfDeferred();
				((IQueueItem)this.mailItem).Priority = value;
			}
		}

		public override DeliveryMethod InboundDeliveryMethod
		{
			get
			{
				this.ThrowIfDeferred();
				return TransportMailItemWrapper.GetInboundDeliveryMethod(this.mailItem);
			}
		}

		internal override string InternetMessageId
		{
			get
			{
				if (this.mailItem != null)
				{
					return this.mailItem.InternetMessageId;
				}
				return null;
			}
		}

		internal override Guid NetworkMessageId
		{
			get
			{
				return this.mailItem.NetworkMessageId;
			}
		}

		internal override Guid SystemProbeId
		{
			get
			{
				if (this.mailItem != null)
				{
					return this.mailItem.SystemProbeId;
				}
				return Guid.Empty;
			}
			set
			{
				this.mailItem.SystemProbeId = value;
			}
		}

		internal override long InternalMessageId
		{
			get
			{
				this.ThrowIfDeferred();
				return this.mailItem.RecordId;
			}
		}

		internal override MessageSnapshotWriter SnapshotWriter
		{
			get
			{
				return this.snapshotWriter;
			}
		}

		internal override bool PipelineTracingEnabled
		{
			get
			{
				return this.pipelineTracingEnabled;
			}
		}

		internal override string PipelineTracingPath
		{
			get
			{
				return this.pipelineTracingPath;
			}
		}

		internal override MimeDocument MimeDocument
		{
			get
			{
				this.ThrowIfDeferred();
				return this.mailItem.MimeDocument;
			}
		}

		internal override object RecipientCache
		{
			get
			{
				this.ThrowIfDeferred();
				return this.mailItem.ADRecipientCache;
			}
		}

		internal override bool MimeWriteStreamOpen
		{
			get
			{
				return this.mailItem != null && this.mailItem.MimeWriteStreamOpen;
			}
		}

		internal TransportMailItem TransportMailItem
		{
			get
			{
				return this.mailItem;
			}
			set
			{
				this.mailItem = value;
				this.recipients = null;
			}
		}

		internal override bool HasBeenDeferred
		{
			get
			{
				return null == this.mailItem;
			}
		}

		internal override bool HasBeenDeleted
		{
			get
			{
				this.ThrowIfDeferred();
				return !this.mailItem.IsActive;
			}
		}

		internal override long CachedMimeStreamLength
		{
			get
			{
				this.ThrowIfDeferred();
				return this.mailItem.MimeSize;
			}
		}

		public override bool MustDeliver
		{
			get
			{
				this.ThrowIfDeferred();
				return this.mailItem.RetryDeliveryIfRejected;
			}
		}

		public override Guid TenantId
		{
			get
			{
				this.ThrowIfDeferred();
				return TransportMailItemWrapper.GetTenantId(this.mailItem);
			}
		}

		public override string OriginatorOrganization
		{
			get
			{
				this.ThrowIfDeferred();
				if (!this.mailItem.ExposeMessageHeaders && !ConfigurationComponent.IsFrontEndTransportProcess(Components.Configuration))
				{
					throw new InvalidOperationException("Property 'Oorg' isn't accessible prior to the OnEndOfHeaders protocol event");
				}
				return this.mailItem.Oorg;
			}
			set
			{
				this.ThrowIfDeferred();
				if (!this.mailItem.ExposeMessageHeaders && !ConfigurationComponent.IsFrontEndTransportProcess(Components.Configuration))
				{
					throw new InvalidOperationException("Property 'Oorg' isn't accessible prior to the OnEndOfHeaders protocol event");
				}
				this.mailItem.Oorg = value;
			}
		}

		public static DeliveryMethod GetInboundDeliveryMethod(IReadOnlyMailItem mailItem)
		{
			if (string.IsNullOrEmpty(mailItem.ReceiveConnectorName))
			{
				return DeliveryMethod.Unknown;
			}
			string value = "SMTP:";
			if (mailItem.ReceiveConnectorName.StartsWith(value, StringComparison.Ordinal))
			{
				return DeliveryMethod.Smtp;
			}
			if (mailItem.ReceiveConnectorName == "FromLocal")
			{
				return DeliveryMethod.Mailbox;
			}
			return DeliveryMethod.File;
		}

		public static Guid GetTenantId(IReadOnlyMailItem mailItem)
		{
			if (mailItem.OrganizationId == null || mailItem.OrganizationId.ConfigurationUnit == null)
			{
				return Guid.Empty;
			}
			return mailItem.OrganizationId.ConfigurationUnit.ObjectGuid;
		}

		public static Stream GetMimeReadStream(IReadOnlyMailItem mailItem, ref List<Stream> openedStreams)
		{
			Stream stream = mailItem.OpenMimeReadStream();
			if (openedStreams == null)
			{
				openedStreams = new List<Stream>();
			}
			openedStreams.Add(stream);
			return stream;
		}

		public static void CloseStreams(ref List<Stream> openedStreams)
		{
			if (openedStreams != null)
			{
				foreach (Stream stream in openedStreams)
				{
					stream.Close();
				}
				openedStreams = null;
			}
		}

		public override void SetMustDeliver()
		{
			this.ThrowIfDeferred();
			this.mailItem.SetMustDeliver();
		}

		public override Stream GetMimeReadStream()
		{
			this.ThrowIfDeferred();
			if (!this.mailItem.ExposeMessage)
			{
				return null;
			}
			if (this.mailItem.MimeWriteStreamOpen)
			{
				throw new InvalidOperationException(Strings.MimeWriteStreamOpen);
			}
			return TransportMailItemWrapper.GetMimeReadStream(this.mailItem, ref this.openedReadStreams);
		}

		public override Stream GetMimeWriteStream()
		{
			this.ThrowIfDeferred();
			if (!this.mailItem.ExposeMessage)
			{
				return null;
			}
			if (this.mailItem.MimeWriteStreamOpen)
			{
				throw new InvalidOperationException(Strings.MimeWriteStreamOpen);
			}
			return this.mailItem.OpenMimeWriteStream();
		}

		internal override void RestoreLastSavedMime(string agentName, string eventName)
		{
			this.mailItem.RestoreLastSavedMime(agentName, eventName);
		}

		public override string ToString()
		{
			this.ThrowIfDeferred();
			return string.Format(CultureInfo.InvariantCulture, "TransportMailItem(hash={0},id={1})", new object[]
			{
				this.mailItem.GetHashCode(),
				this.mailItem.RecordId
			});
		}

		internal void CloseWrapper()
		{
			TransportMailItemWrapper.CloseStreams(ref this.openedReadStreams);
			this.recipients = null;
			this.mailItem = null;
		}

		private void ThrowIfDeferred()
		{
			if (this.HasBeenDeferred)
			{
				throw new InvalidOperationException(Strings.MailItemDeferred);
			}
		}

		protected TransportMailItem mailItem;

		private List<Stream> openedReadStreams;

		private MailRecipientCollectionWrapper recipients;

		private readonly bool canAddRecipients;

		private readonly MessageSnapshotWriter snapshotWriter;

		private readonly IMExSession mexSession;

		private readonly bool pipelineTracingEnabled;

		private readonly string pipelineTracingPath;
	}
}
