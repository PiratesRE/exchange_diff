using System;
using System.Net;
using Microsoft.Exchange.Data.Transport;

namespace Microsoft.Exchange.Data.QueueViewer
{
	[Serializable]
	public abstract class ExtensibleMessageInfo : ConfigurableObject, PagedDataObject, IConfigurable
	{
		internal ExtensibleMessageInfo(long identity, QueueIdentity queueIdentity, MessageInfoPropertyBag propertyBag) : base(propertyBag)
		{
			this[this.propertyBag.ObjectIdentityPropertyDefinition] = new MessageIdentity(identity, queueIdentity);
		}

		internal ExtensibleMessageInfo(MessageInfoPropertyBag emptyBag) : base(emptyBag)
		{
		}

		public void Reset(long identity, QueueIdentity queueIdentity)
		{
			this.propertyBag = new MessageInfoPropertyBag();
			this[this.propertyBag.ObjectIdentityPropertyDefinition] = new MessageIdentity(identity, queueIdentity);
			this.Subject = null;
			this.InternetMessageId = null;
			this.FromAddress = null;
			this.Status = MessageStatus.None;
			this.Size = default(ByteQuantifiedSize);
			this.MessageSourceName = null;
			this.SourceIP = null;
			this.SCL = 0;
			this.DateReceived = default(DateTime);
			this.ExpirationTime = null;
			this.LastError = null;
			this.LastErrorCode = 0;
			this.RetryCount = 0;
			this.Recipients = null;
			this.ComponentLatency = null;
			this.MessageLatency = default(EnhancedTimeSpan);
			this.DeferReason = null;
			this.LockReason = null;
			this.IsProbeMessage = false;
			this.OutboundIPPool = 0;
			this.Directionality = MailDirectionality.Undefined;
			this.ExternalDirectoryOrganizationId = default(Guid);
			this.OriginalFromAddress = null;
			this.AccountForest = null;
		}

		internal override ObjectSchema ObjectSchema
		{
			get
			{
				return ExtensibleMessageInfo.schema;
			}
		}

		public void ConvertDatesToLocalTime()
		{
			this.DateReceived = this.DateReceived.ToLocalTime();
			if (this.ExpirationTime != null)
			{
				this.ExpirationTime = new DateTime?(this.ExpirationTime.Value.ToLocalTime());
			}
		}

		public void ConvertDatesToUniversalTime()
		{
			this.DateReceived = this.DateReceived.ToUniversalTime();
			if (this.ExpirationTime != null)
			{
				this.ExpirationTime = new DateTime?(this.ExpirationTime.Value.ToUniversalTime());
			}
		}

		public MessageIdentity MessageIdentity
		{
			get
			{
				return (MessageIdentity)this.Identity;
			}
		}

		public QueueIdentity Queue
		{
			get
			{
				return this.MessageIdentity.QueueIdentity;
			}
		}

		public abstract string Subject { get; internal set; }

		public abstract string InternetMessageId { get; internal set; }

		public abstract string FromAddress { get; internal set; }

		public abstract MessageStatus Status { get; internal set; }

		public abstract ByteQuantifiedSize Size { get; internal set; }

		public abstract string MessageSourceName { get; internal set; }

		public abstract IPAddress SourceIP { get; internal set; }

		public abstract int SCL { get; internal set; }

		public abstract DateTime DateReceived { get; internal set; }

		public abstract DateTime? ExpirationTime { get; internal set; }

		internal abstract int LastErrorCode { get; set; }

		public abstract string LastError { get; internal set; }

		public abstract int RetryCount { get; internal set; }

		public abstract RecipientInfo[] Recipients { get; internal set; }

		public abstract ComponentLatencyInfo[] ComponentLatency { get; internal set; }

		public abstract EnhancedTimeSpan MessageLatency { get; internal set; }

		public abstract string DeferReason { get; internal set; }

		public abstract string LockReason { get; internal set; }

		public abstract string Priority { get; internal set; }

		internal abstract bool IsProbeMessage { get; set; }

		public abstract Guid ExternalDirectoryOrganizationId { get; internal set; }

		public abstract MailDirectionality Directionality { get; internal set; }

		public abstract string OriginalFromAddress { get; internal set; }

		public abstract string AccountForest { get; internal set; }

		internal abstract int OutboundIPPool { get; set; }

		private static ExtensibleMessageInfoSchema schema = ObjectSchema.GetInstance<ExtensibleMessageInfoSchema>();
	}
}
