using System;
using System.Net;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Data.QueueViewer
{
	[Serializable]
	public class MessageInfo : PagedDataObject, IConfigurable
	{
		internal MessageInfo(long identity, QueueIdentity queueIdentity)
		{
			this.identity = new MessageIdentity(identity, queueIdentity);
		}

		public MessageInfo(ExtensibleMessageInfo messageInfo)
		{
			this.identity = messageInfo.MessageIdentity;
			this.subject = messageInfo.Subject;
			this.internetMessageId = messageInfo.InternetMessageId;
			this.fromAddress = messageInfo.FromAddress;
			this.status = messageInfo.Status;
			this.size = messageInfo.Size;
			this.messageSourceName = messageInfo.MessageSourceName;
			this.sourceIP = messageInfo.SourceIP;
			this.scl = messageInfo.SCL;
			this.dateReceived = messageInfo.DateReceived;
			this.expirationTime = messageInfo.ExpirationTime;
			this.lastError = messageInfo.LastError;
			this.lastErrorCode = messageInfo.LastErrorCode;
			this.retryCount = messageInfo.RetryCount;
			this.recipients = messageInfo.Recipients;
			this.componentLatency = messageInfo.ComponentLatency;
			this.messageLatency = messageInfo.MessageLatency;
		}

		public void Reset(long identity, QueueIdentity queueIdentity)
		{
			this.identity = new MessageIdentity(identity, queueIdentity);
			this.subject = null;
			this.internetMessageId = null;
			this.fromAddress = null;
			this.status = MessageStatus.None;
			this.size = default(ByteQuantifiedSize);
			this.messageSourceName = null;
			this.sourceIP = null;
			this.scl = 0;
			this.dateReceived = default(DateTime);
			this.expirationTime = null;
			this.lastError = null;
			this.lastErrorCode = 0;
			this.retryCount = 0;
			this.recipients = null;
			this.componentLatency = null;
			this.messageLatency = default(EnhancedTimeSpan);
		}

		public ObjectId Identity
		{
			get
			{
				return this.identity;
			}
		}

		public string Subject
		{
			get
			{
				return this.subject;
			}
			internal set
			{
				this.subject = value;
			}
		}

		public string InternetMessageId
		{
			get
			{
				return this.internetMessageId;
			}
			internal set
			{
				this.internetMessageId = value;
			}
		}

		public string FromAddress
		{
			get
			{
				return this.fromAddress;
			}
			internal set
			{
				this.fromAddress = value;
			}
		}

		public MessageStatus Status
		{
			get
			{
				return this.status;
			}
			internal set
			{
				this.status = value;
			}
		}

		public ByteQuantifiedSize Size
		{
			get
			{
				return this.size;
			}
			internal set
			{
				this.size = value;
			}
		}

		public string MessageSourceName
		{
			get
			{
				return this.messageSourceName;
			}
			internal set
			{
				this.messageSourceName = value;
			}
		}

		public IPAddress SourceIP
		{
			get
			{
				return this.sourceIP;
			}
			internal set
			{
				this.sourceIP = value;
			}
		}

		public int SCL
		{
			get
			{
				return this.scl;
			}
			internal set
			{
				this.scl = value;
			}
		}

		public DateTime DateReceived
		{
			get
			{
				return this.dateReceived;
			}
			internal set
			{
				this.dateReceived = value;
			}
		}

		public DateTime? ExpirationTime
		{
			get
			{
				return this.expirationTime;
			}
			internal set
			{
				this.expirationTime = value;
			}
		}

		internal int LastErrorCode
		{
			get
			{
				return this.lastErrorCode;
			}
			set
			{
				this.lastErrorCode = value;
			}
		}

		public string LastError
		{
			get
			{
				if (this.lastError != null)
				{
					return this.lastError;
				}
				if (this.identity.QueueIdentity.Type == QueueType.Unreachable)
				{
					return StatusCodeConverter.UnreachableReasonToString((UnreachableReason)this.lastErrorCode);
				}
				if (this.identity.QueueIdentity.Type == QueueType.Submission)
				{
					return StatusCodeConverter.DeferReasonToString((DeferReason)this.lastErrorCode);
				}
				return null;
			}
			internal set
			{
				this.lastError = value;
			}
		}

		public int RetryCount
		{
			get
			{
				return this.retryCount;
			}
			internal set
			{
				this.retryCount = value;
			}
		}

		public QueueIdentity Queue
		{
			get
			{
				return this.identity.QueueIdentity;
			}
		}

		public RecipientInfo[] Recipients
		{
			get
			{
				return this.recipients;
			}
			internal set
			{
				this.recipients = value;
			}
		}

		public ComponentLatencyInfo[] ComponentLatency
		{
			get
			{
				return this.componentLatency;
			}
			internal set
			{
				this.componentLatency = value;
			}
		}

		public EnhancedTimeSpan MessageLatency
		{
			get
			{
				return this.messageLatency;
			}
			internal set
			{
				this.messageLatency = value;
			}
		}

		public void ConvertDatesToLocalTime()
		{
			throw new NotImplementedException();
		}

		public void ConvertDatesToUniversalTime()
		{
			throw new NotImplementedException();
		}

		public ValidationError[] Validate()
		{
			throw new NotImplementedException();
		}

		public bool IsValid
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public ObjectState ObjectState
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public void CopyChangesFrom(IConfigurable source)
		{
			throw new NotImplementedException();
		}

		public void ResetChangeTracking()
		{
			throw new NotImplementedException();
		}

		private MessageIdentity identity;

		private string subject;

		private string internetMessageId;

		private string fromAddress;

		private MessageStatus status;

		private ByteQuantifiedSize size;

		private string messageSourceName;

		private IPAddress sourceIP;

		private int scl;

		private DateTime dateReceived;

		private DateTime? expirationTime;

		private string lastError;

		private int lastErrorCode;

		private int retryCount;

		private RecipientInfo[] recipients;

		[OptionalField(VersionAdded = 2)]
		private ComponentLatencyInfo[] componentLatency;

		[OptionalField(VersionAdded = 2)]
		private EnhancedTimeSpan messageLatency;
	}
}
