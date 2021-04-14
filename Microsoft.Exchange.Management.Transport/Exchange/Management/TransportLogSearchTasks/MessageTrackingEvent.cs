using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Management.TransportLogSearchTasks
{
	[Serializable]
	public class MessageTrackingEvent
	{
		public DateTime Timestamp
		{
			get
			{
				return this.timestamp;
			}
			internal set
			{
				this.timestamp = value;
			}
		}

		public string ClientIp
		{
			get
			{
				return this.clientIp;
			}
			internal set
			{
				this.clientIp = value;
			}
		}

		public string ClientHostname
		{
			get
			{
				return this.clientHostname;
			}
			internal set
			{
				this.clientHostname = value;
			}
		}

		public string ServerIp
		{
			get
			{
				return this.serverIp;
			}
			internal set
			{
				this.serverIp = value;
			}
		}

		public string ServerHostname
		{
			get
			{
				return this.serverHostname;
			}
			internal set
			{
				this.serverHostname = value;
			}
		}

		public string SourceContext
		{
			get
			{
				return this.sourceContext;
			}
			internal set
			{
				this.sourceContext = value;
			}
		}

		public string ConnectorId
		{
			get
			{
				return this.connectorId;
			}
			internal set
			{
				this.connectorId = value;
			}
		}

		public string Source
		{
			get
			{
				return this.source;
			}
			internal set
			{
				this.source = value;
			}
		}

		public string EventId
		{
			get
			{
				return this.eventId;
			}
			internal set
			{
				this.eventId = value;
			}
		}

		public string InternalMessageId
		{
			get
			{
				return this.internalMessageId;
			}
			internal set
			{
				this.internalMessageId = value;
			}
		}

		public string MessageId
		{
			get
			{
				return this.messageId;
			}
			internal set
			{
				this.messageId = value;
			}
		}

		public string[] Recipients
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

		public string[] RecipientStatus
		{
			get
			{
				return this.recipientStatus;
			}
			internal set
			{
				this.recipientStatus = value;
			}
		}

		public int? TotalBytes
		{
			get
			{
				return this.totalBytes;
			}
			internal set
			{
				this.totalBytes = value;
			}
		}

		public int? RecipientCount
		{
			get
			{
				return this.recipientCount;
			}
			internal set
			{
				this.recipientCount = value;
			}
		}

		public string RelatedRecipientAddress
		{
			get
			{
				return this.relatedRecipientAddress;
			}
			internal set
			{
				this.relatedRecipientAddress = value;
			}
		}

		public string[] Reference
		{
			get
			{
				return this.reference;
			}
			internal set
			{
				this.reference = value;
			}
		}

		public string MessageSubject
		{
			get
			{
				return this.messageSubject;
			}
			internal set
			{
				this.messageSubject = value;
			}
		}

		public string Sender
		{
			get
			{
				return this.sender;
			}
			internal set
			{
				this.sender = value;
			}
		}

		public string ReturnPath
		{
			get
			{
				return this.returnPath;
			}
			internal set
			{
				this.returnPath = value;
			}
		}

		public string Directionality
		{
			get
			{
				return this.directionality;
			}
			internal set
			{
				this.directionality = value;
			}
		}

		public string TenantId
		{
			get
			{
				return this.tenantId;
			}
			internal set
			{
				this.tenantId = value;
			}
		}

		public string OriginalClientIp
		{
			get
			{
				return this.originalClientIp;
			}
			internal set
			{
				this.originalClientIp = value;
			}
		}

		public string MessageInfo
		{
			get
			{
				return MessageTrackingEvent.FormatMessageInfo(this.messageInfo, this.eventId);
			}
			internal set
			{
				this.messageInfo = value;
			}
		}

		public EnhancedTimeSpan? MessageLatency
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

		public MessageLatencyType MessageLatencyType
		{
			get
			{
				return this.messageLatencyType;
			}
			internal set
			{
				this.messageLatencyType = value;
			}
		}

		public KeyValuePair<string, object>[] EventData
		{
			get
			{
				return this.eventData;
			}
			internal set
			{
				this.eventData = value;
			}
		}

		private static string FormatMessageInfo(string messageInfo, string eventId)
		{
			if (eventId == "SEND" || eventId == "DELIVER" || eventId == "DEFER")
			{
				DateTime dateTime;
				bool flag = DateTime.TryParseExact(messageInfo, "yyyy-MM-ddTHH\\:mm\\:ss.fffZ", DateTimeFormatInfo.InvariantInfo, DateTimeStyles.AllowLeadingWhite | DateTimeStyles.AllowTrailingWhite | DateTimeStyles.AllowInnerWhite | DateTimeStyles.AssumeUniversal, out dateTime);
				if (flag)
				{
					return dateTime.ToLocalTime().ToString();
				}
			}
			return messageInfo;
		}

		private DateTime timestamp;

		private string clientIp;

		private string clientHostname;

		private string serverIp;

		private string serverHostname;

		private string sourceContext;

		private string connectorId;

		private string source;

		private string eventId;

		private string internalMessageId;

		private string messageId;

		private string[] recipients;

		private string[] recipientStatus;

		private int? totalBytes;

		private int? recipientCount;

		private string relatedRecipientAddress;

		private string[] reference;

		private string messageSubject;

		private string sender;

		private string returnPath;

		private string directionality;

		private string tenantId;

		private string originalClientIp;

		private string messageInfo;

		private EnhancedTimeSpan? messageLatency;

		private MessageLatencyType messageLatencyType;

		private KeyValuePair<string, object>[] eventData;
	}
}
