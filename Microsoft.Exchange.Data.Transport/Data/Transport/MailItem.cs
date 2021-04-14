using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Exchange.Data.Mime;
using Microsoft.Exchange.Data.Transport.Email;

namespace Microsoft.Exchange.Data.Transport
{
	public abstract class MailItem
	{
		internal MailItem()
		{
		}

		public abstract string OriginalAuthenticator { get; set; }

		public abstract DateTime DateTimeReceived { get; }

		public abstract string EnvelopeId { get; set; }

		public abstract RoutingAddress FromAddress { get; set; }

		public abstract string OriginatingDomain { get; }

		public abstract EmailMessage Message { get; }

		public abstract long MimeStreamLength { get; }

		public abstract IDictionary<string, object> Properties { get; }

		public abstract EnvelopeRecipientCollection Recipients { get; }

		public abstract DsnFormatRequested DsnFormatRequested { get; set; }

		public abstract DeliveryPriority DeliveryPriority { get; set; }

		public abstract DeliveryMethod InboundDeliveryMethod { get; }

		public abstract bool MustDeliver { get; }

		public abstract Guid TenantId { get; }

		public abstract string OriginatorOrganization { get; set; }

		internal abstract long InternalMessageId { get; }

		internal abstract Guid NetworkMessageId { get; }

		internal abstract Guid SystemProbeId { get; set; }

		internal abstract MessageSnapshotWriter SnapshotWriter { get; }

		internal abstract bool PipelineTracingEnabled { get; }

		internal abstract string PipelineTracingPath { get; }

		internal abstract MimeDocument MimeDocument { get; }

		internal abstract string InternetMessageId { get; }

		internal abstract object RecipientCache { get; }

		internal abstract bool MimeWriteStreamOpen { get; }

		internal abstract bool HasBeenDeferred { get; }

		internal abstract bool HasBeenDeleted { get; }

		internal abstract long CachedMimeStreamLength { get; }

		public abstract void SetMustDeliver();

		public abstract Stream GetMimeReadStream();

		public abstract Stream GetMimeWriteStream();

		internal abstract void RestoreLastSavedMime(string agentName, string eventName);

		internal bool IsProbeMessage
		{
			get
			{
				return this.SystemProbeId != Guid.Empty;
			}
		}

		internal bool HasRecipients
		{
			get
			{
				return this.Recipients == null || this.Recipients.Any<EnvelopeRecipient>();
			}
		}
	}
}
