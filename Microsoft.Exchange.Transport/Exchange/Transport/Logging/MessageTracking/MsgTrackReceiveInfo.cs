using System;
using System.Collections.Generic;
using System.Net;
using Microsoft.Exchange.Data.Mime;

namespace Microsoft.Exchange.Transport.Logging.MessageTracking
{
	internal class MsgTrackReceiveInfo
	{
		public MsgTrackReceiveInfo(string sourceContext) : this(null, string.Empty, null, sourceContext, string.Empty, null, null, string.Empty, string.Empty, string.Empty, null, string.Empty, null, null, string.Empty, null, Guid.Empty)
		{
		}

		public MsgTrackReceiveInfo(string sourceContext, string securityInfo, IList<string> invalidRecipients) : this(null, null, null, sourceContext, null, null, null, securityInfo, string.Empty, string.Empty, invalidRecipients, string.Empty, null, null, string.Empty, null, Guid.Empty)
		{
		}

		public MsgTrackReceiveInfo(IPAddress serverIp, string sourceContext, string relatedMessageId, string securityInfo, IList<string> invalidRecipients) : this(null, string.Empty, serverIp, sourceContext, string.Empty, null, relatedMessageId, securityInfo, string.Empty, string.Empty, invalidRecipients, string.Empty, null, null, string.Empty, null, Guid.Empty)
		{
		}

		public MsgTrackReceiveInfo(IPAddress serverIp, long? relatedMailItemId, string securityInfo, IList<string> invalidRecipients) : this(null, string.Empty, serverIp, string.Empty, string.Empty, relatedMailItemId, null, securityInfo, string.Empty, string.Empty, null, string.Empty, null, null, string.Empty, null, Guid.Empty)
		{
		}

		public MsgTrackReceiveInfo(IPAddress clientIP, string clientHostName, IPAddress serverIP, string sourceContext, string securityInfo, string mailboxDatabaseGuid, string submittingMailboxSmtpAddress, byte[] entryId) : this(clientIP, clientHostName, serverIP, sourceContext, string.Empty, null, null, securityInfo, string.Empty, submittingMailboxSmtpAddress, null, mailboxDatabaseGuid, entryId, null, string.Empty, null, Guid.Empty)
		{
		}

		public MsgTrackReceiveInfo(IPAddress clientIP, string clientHostName, IPAddress serverIP, string sourceContext, string connectorId, long? relatedMailItemId) : this(clientIP, clientHostName, serverIP, sourceContext, connectorId, relatedMailItemId, null, string.Empty, Guid.Empty)
		{
		}

		public MsgTrackReceiveInfo(IPAddress clientIP, string clientHostName, IPAddress serverIP, string sourceContext, string connectorId, long? relatedMailItemId, IPAddress proxiedClientIPAddress, string proxiedClientHostname, Guid authUserMailboxGuid) : this(clientIP, clientHostName, serverIP, sourceContext, connectorId, relatedMailItemId, null, string.Empty, string.Empty, string.Empty, null, string.Empty, null, proxiedClientIPAddress, proxiedClientHostname, null, authUserMailboxGuid)
		{
		}

		public MsgTrackReceiveInfo(IPAddress clientIP, string clientHostName, IPAddress serverIP, string sourceContext, string connectorId, long? relatedMailItemId, string securityInfo) : this(clientIP, clientHostName, serverIP, sourceContext, connectorId, relatedMailItemId, null, securityInfo, string.Empty, string.Empty, null, string.Empty, null, null, string.Empty, null, Guid.Empty)
		{
		}

		public MsgTrackReceiveInfo(IPAddress clientIP, string clientHostName, IPAddress serverIP, string sourceContext, string connectorId, long? relatedMailItemId, string securityInfo, string relatedMessageInfo, string submittingMailboxSmtpAddress) : this(clientIP, clientHostName, serverIP, sourceContext, connectorId, relatedMailItemId, securityInfo, relatedMessageInfo, submittingMailboxSmtpAddress, null, string.Empty, null, Guid.Empty)
		{
		}

		public MsgTrackReceiveInfo(IPAddress clientIP, string clientHostName, IPAddress serverIP, string sourceContext, string connectorId, long? relatedMailItemId, string securityInfo, string relatedMessageInfo, string submittingMailboxSmtpAddress, IPAddress proxiedClientIPAddress, string proxiedClientHostname, IReadOnlyList<Header> receivedHeaders, Guid authUserMailboxGuid) : this(clientIP, clientHostName, serverIP, sourceContext, connectorId, relatedMailItemId, string.Empty, securityInfo, relatedMessageInfo, submittingMailboxSmtpAddress, null, string.Empty, null, proxiedClientIPAddress, proxiedClientHostname, receivedHeaders, authUserMailboxGuid)
		{
		}

		private MsgTrackReceiveInfo(IPAddress clientIP, string clientHostName, IPAddress serverIP, string sourceContext, string connectorId, long? relatedMailItemId, string relatedMessageId, string securityInfo, string relatedMessageInfo, string submittingMailboxSmtpAddress, IList<string> invalidRecipients, string mailboxDatabaseGuid, byte[] entryId, IPAddress proxiedClientIP, string proxiedClientHostname, IReadOnlyList<Header> receivedHeaders, Guid authUserMailboxGuid)
		{
			this.clientIPAddress = clientIP;
			this.clientHostName = clientHostName;
			this.serverIPAddress = serverIP;
			this.sourceContext = sourceContext;
			this.connectorId = connectorId;
			this.submittingMailboxSmtpAddress = submittingMailboxSmtpAddress;
			this.relatedMailItemId = relatedMailItemId;
			this.relatedMessageId = relatedMessageId;
			this.securityInfo = securityInfo;
			this.relatedMessageInfo = relatedMessageInfo;
			this.submittingMailboxSmtpAddress = submittingMailboxSmtpAddress;
			this.invalidRecipients = invalidRecipients;
			this.mailboxDatabaseGuid = mailboxDatabaseGuid;
			this.entryId = entryId;
			this.proxiedClientIPAddress = proxiedClientIP;
			this.proxiedClientHostname = proxiedClientHostname;
			this.receivedHeaders = receivedHeaders;
			this.authUserMailboxGuid = authUserMailboxGuid;
		}

		internal IPAddress ClientIPAddress
		{
			get
			{
				return this.clientIPAddress;
			}
		}

		internal IPAddress ServerIPAddress
		{
			get
			{
				return this.serverIPAddress;
			}
		}

		internal string ClientHostname
		{
			get
			{
				return this.clientHostName;
			}
		}

		internal string SourceContext
		{
			get
			{
				return this.sourceContext;
			}
		}

		internal string ConnectorId
		{
			get
			{
				return this.connectorId;
			}
		}

		internal long? RelatedMailItemId
		{
			get
			{
				return this.relatedMailItemId;
			}
		}

		internal string RelatedMessageId
		{
			get
			{
				return this.relatedMessageId;
			}
		}

		internal string SecurityInfo
		{
			get
			{
				return this.securityInfo;
			}
		}

		internal string RelatedMessageInfo
		{
			get
			{
				return this.relatedMessageInfo;
			}
		}

		internal string SubmittingMailboxSmtpAddress
		{
			get
			{
				return this.submittingMailboxSmtpAddress;
			}
		}

		public IList<string> InvalidRecipients
		{
			get
			{
				return this.invalidRecipients;
			}
		}

		public byte[] EntryId
		{
			get
			{
				return this.entryId;
			}
		}

		public string MailboxDatabaseGuid
		{
			get
			{
				return this.mailboxDatabaseGuid;
			}
		}

		public IPAddress ProxiedClientIPAddress
		{
			get
			{
				return this.proxiedClientIPAddress;
			}
		}

		public string ProxiedClientHostname
		{
			get
			{
				return this.proxiedClientHostname;
			}
		}

		public IReadOnlyList<Header> ReceivedHeaders
		{
			get
			{
				return this.receivedHeaders;
			}
		}

		public Guid AuthUserMailboxGuid
		{
			get
			{
				return this.authUserMailboxGuid;
			}
		}

		private readonly string proxiedClientHostname;

		private readonly IReadOnlyList<Header> receivedHeaders;

		private IPAddress proxiedClientIPAddress;

		private IPAddress clientIPAddress;

		private string clientHostName;

		private IPAddress serverIPAddress;

		private string sourceContext;

		private string connectorId;

		private long? relatedMailItemId = null;

		private string relatedMessageId;

		private string securityInfo;

		private string relatedMessageInfo;

		private string submittingMailboxSmtpAddress;

		private IList<string> invalidRecipients;

		private byte[] entryId;

		private string mailboxDatabaseGuid;

		private Guid authUserMailboxGuid;
	}
}
