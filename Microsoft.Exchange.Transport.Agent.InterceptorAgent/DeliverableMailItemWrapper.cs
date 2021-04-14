using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Exchange.Data.Mime;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Data.Transport.Email;

namespace Microsoft.Exchange.Transport.Agent.InterceptorAgent
{
	internal sealed class DeliverableMailItemWrapper : MailItem
	{
		public DeliverableMailItemWrapper(DeliverableMailItem m)
		{
			this.mailItem = m;
		}

		public override string OriginalAuthenticator
		{
			get
			{
				return this.mailItem.OriginalAuthenticator;
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public override string EnvelopeId
		{
			get
			{
				return this.mailItem.EnvelopeId;
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public override RoutingAddress FromAddress
		{
			get
			{
				return this.mailItem.FromAddress;
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public override string OriginatingDomain
		{
			get
			{
				return this.mailItem.OriginatingDomain;
			}
		}

		public override EmailMessage Message
		{
			get
			{
				return this.mailItem.Message;
			}
		}

		public override IDictionary<string, object> Properties
		{
			get
			{
				return new Dictionary<string, object>();
			}
		}

		public override EnvelopeRecipientCollection Recipients
		{
			get
			{
				return new EnvelopeRecipientCollectionWrapper(this.mailItem.Recipients);
			}
		}

		public override DsnFormatRequested DsnFormatRequested
		{
			get
			{
				return this.mailItem.DsnFormatRequested;
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public override DateTime DateTimeReceived
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public override long MimeStreamLength
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public override DeliveryPriority DeliveryPriority
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public override DeliveryMethod InboundDeliveryMethod
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public override bool MustDeliver
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public override Guid TenantId
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public override string OriginatorOrganization
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		internal override long InternalMessageId
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		internal override Guid NetworkMessageId
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		internal override Guid SystemProbeId
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		internal override MessageSnapshotWriter SnapshotWriter
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		internal override bool PipelineTracingEnabled
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		internal override string PipelineTracingPath
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		internal override string InternetMessageId
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		internal override object RecipientCache
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		internal override bool MimeWriteStreamOpen
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		internal override bool HasBeenDeferred
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		internal override bool HasBeenDeleted
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		internal override long CachedMimeStreamLength
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		internal override MimeDocument MimeDocument
		{
			get
			{
				return this.mailItem.Message.MimeDocument;
			}
		}

		public override void SetMustDeliver()
		{
			throw new NotImplementedException();
		}

		public override Stream GetMimeReadStream()
		{
			throw new NotImplementedException();
		}

		public override Stream GetMimeWriteStream()
		{
			throw new NotImplementedException();
		}

		internal override void RestoreLastSavedMime(string agentName, string eventName)
		{
			throw new NotImplementedException();
		}

		private DeliverableMailItem mailItem;
	}
}
