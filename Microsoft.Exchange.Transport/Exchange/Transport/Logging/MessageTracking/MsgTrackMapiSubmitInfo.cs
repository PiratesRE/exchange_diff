using System;
using System.Net;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Transport;

namespace Microsoft.Exchange.Transport.Logging.MessageTracking
{
	internal class MsgTrackMapiSubmitInfo
	{
		public MsgTrackMapiSubmitInfo(string mapiEventInfo, IPAddress bridgeheadServerIPAddress, string bridgeheadServerName, IPAddress mailboxServerIPAddress, string mailboxServerName, string sender, string from, string messageId, byte[] itemEntryId, string subject, string latencyData, string diagnosticInfo, bool isRegularSubmission)
		{
			this.mapiEventInfo = mapiEventInfo;
			this.bridgeheadServerIPAddress = bridgeheadServerIPAddress;
			this.bridgeheadServerName = bridgeheadServerName;
			this.mailboxServerIPAddress = mailboxServerIPAddress;
			this.mailboxServerName = mailboxServerName;
			this.sender = sender;
			this.from = from;
			this.messageId = messageId;
			this.itemEntryId = itemEntryId;
			this.subject = subject;
			this.latencyData = latencyData;
			this.diagnosticInfo = diagnosticInfo;
			this.isRegularSubmission = isRegularSubmission;
		}

		public MsgTrackMapiSubmitInfo(string mapiEventInfo, IPAddress bridgeheadServerIPAddress, string bridgeheadServerName, IPAddress mailboxServerIPAddress, string mailboxServerName, string sender, string from, string messageId, byte[] itemEntryId, string subject, string latencyData, string diagnosticInfo, bool isRegularSubmission, Guid externalOrganizationId, OrganizationId organizationId, string[] recipientAddresses, MailDirectionality direction, Guid networkMessageId, IPAddress originalClientIPAddress)
		{
			this.mapiEventInfo = mapiEventInfo;
			this.bridgeheadServerIPAddress = bridgeheadServerIPAddress;
			this.bridgeheadServerName = bridgeheadServerName;
			this.mailboxServerIPAddress = mailboxServerIPAddress;
			this.mailboxServerName = mailboxServerName;
			this.sender = sender;
			this.from = from;
			this.messageId = messageId;
			this.itemEntryId = itemEntryId;
			this.subject = subject;
			this.latencyData = latencyData;
			this.diagnosticInfo = diagnosticInfo;
			this.isRegularSubmission = isRegularSubmission;
			this.externalOrganizationId = externalOrganizationId;
			this.organizationId = organizationId;
			this.recipientAddresses = recipientAddresses;
			this.direction = direction;
			this.directionIsSet = true;
			this.networkMessageId = networkMessageId;
			this.originalClientIPAddress = originalClientIPAddress;
		}

		public MsgTrackMapiSubmitInfo(string mapiEventInfo, IPAddress bridgeheadServerIPAddress, string bridgeheadServerName, IPAddress mailboxServerIPAddress, string mailboxServerName, string sender, string messageId, byte[] itemEntryId, string latencyData, string diagnosticInfo, bool isPermanentFailure, bool isRegularSubmission)
		{
			this.mapiEventInfo = mapiEventInfo;
			this.bridgeheadServerIPAddress = bridgeheadServerIPAddress;
			this.bridgeheadServerName = bridgeheadServerName;
			this.mailboxServerIPAddress = mailboxServerIPAddress;
			this.mailboxServerName = mailboxServerName;
			this.sender = sender;
			this.messageId = messageId;
			this.latencyData = latencyData;
			this.diagnosticInfo = diagnosticInfo;
			this.itemEntryId = itemEntryId;
			this.isPermanentFailure = isPermanentFailure;
			this.failed = true;
			this.isRegularSubmission = isRegularSubmission;
		}

		public MsgTrackMapiSubmitInfo(string mapiEventInfo, IPAddress bridgeheadServerIPAddress, string bridgeheadServerName, IPAddress mailboxServerIPAddress, string mailboxServerName, string sender, string messageId, byte[] itemEntryId, string latencyData, string diagnosticInfo, bool isPermanentFailure, bool isRegularSubmission, Guid externalOrganizationId, OrganizationId organizationId, string[] recipientAddresses, MailDirectionality direction, Guid networkMessageId)
		{
			this.mapiEventInfo = mapiEventInfo;
			this.bridgeheadServerIPAddress = bridgeheadServerIPAddress;
			this.bridgeheadServerName = bridgeheadServerName;
			this.mailboxServerIPAddress = mailboxServerIPAddress;
			this.mailboxServerName = mailboxServerName;
			this.sender = sender;
			this.messageId = messageId;
			this.latencyData = latencyData;
			this.diagnosticInfo = diagnosticInfo;
			this.itemEntryId = itemEntryId;
			this.isPermanentFailure = isPermanentFailure;
			this.failed = true;
			this.isRegularSubmission = isRegularSubmission;
			this.externalOrganizationId = externalOrganizationId;
			this.organizationId = organizationId;
			this.recipientAddresses = recipientAddresses;
			this.direction = direction;
			this.directionIsSet = true;
			this.networkMessageId = networkMessageId;
		}

		public IPAddress BridgeheadServerIPAddress
		{
			get
			{
				return this.bridgeheadServerIPAddress;
			}
		}

		public IPAddress MailboxServerIPAddress
		{
			get
			{
				return this.mailboxServerIPAddress;
			}
		}

		public string BridgeheadServerName
		{
			get
			{
				return this.bridgeheadServerName;
			}
		}

		public string MailboxServerName
		{
			get
			{
				return this.mailboxServerName;
			}
		}

		public string Sender
		{
			get
			{
				return this.sender;
			}
		}

		public string From
		{
			get
			{
				return this.from;
			}
		}

		public string MessageId
		{
			get
			{
				return this.messageId;
			}
		}

		public byte[] ItemEntryId
		{
			get
			{
				return this.itemEntryId;
			}
		}

		public string Subject
		{
			get
			{
				return this.subject;
			}
		}

		public bool Failed
		{
			get
			{
				return this.failed;
			}
		}

		public bool IsPermanentFailure
		{
			get
			{
				return this.isPermanentFailure;
			}
		}

		public string LatencyData
		{
			get
			{
				return this.latencyData;
			}
		}

		public string DiagnosticInfo
		{
			get
			{
				return this.diagnosticInfo;
			}
		}

		public string MapiEventInfo
		{
			get
			{
				return this.mapiEventInfo;
			}
		}

		public bool IsRegularSubmission
		{
			get
			{
				return this.isRegularSubmission;
			}
		}

		public OrganizationId OrganizationId
		{
			get
			{
				return this.organizationId;
			}
		}

		public Guid ExternalOrganizationId
		{
			get
			{
				return this.externalOrganizationId;
			}
		}

		public string[] RecipientAddresses
		{
			get
			{
				return this.recipientAddresses;
			}
		}

		public MailDirectionality Direction
		{
			get
			{
				return this.direction;
			}
		}

		public bool DirectionIsSet
		{
			get
			{
				return this.directionIsSet;
			}
		}

		public Guid NetworkMessageId
		{
			get
			{
				return this.networkMessageId;
			}
		}

		public IPAddress OriginalClientIPAddress
		{
			get
			{
				return this.originalClientIPAddress;
			}
		}

		private readonly bool directionIsSet;

		private readonly OrganizationId organizationId;

		private readonly Guid externalOrganizationId;

		private readonly string[] recipientAddresses;

		private readonly MailDirectionality direction;

		private readonly Guid networkMessageId;

		private IPAddress bridgeheadServerIPAddress;

		private string bridgeheadServerName;

		private IPAddress mailboxServerIPAddress;

		private string mailboxServerName;

		private string messageId;

		private string subject;

		private string sender;

		private string from;

		private string diagnosticInfo;

		private string latencyData;

		private bool failed;

		private bool isPermanentFailure;

		private string mapiEventInfo;

		private byte[] itemEntryId;

		private bool isRegularSubmission;

		private IPAddress originalClientIPAddress;
	}
}
