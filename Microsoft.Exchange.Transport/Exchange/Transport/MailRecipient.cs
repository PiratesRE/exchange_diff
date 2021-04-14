using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Data.Transport.Smtp;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Extensibility.Internal;
using Microsoft.Exchange.Transport.Categorizer;
using Microsoft.Exchange.Transport.Logging.MessageTracking;
using Microsoft.Exchange.Transport.Storage;
using Microsoft.Exchange.Transport.Storage.Messaging;
using Microsoft.Office.Datacenter.ActiveMonitoring;

namespace Microsoft.Exchange.Transport
{
	internal class MailRecipient : IMailRecipientFacade
	{
		private MailRecipient(TransportMailItem mailItem, IMailRecipientStorage storage)
		{
			this.mailItem = mailItem;
			this.storage = storage;
		}

		public long RecipientId
		{
			get
			{
				return this.storage.RecipId;
			}
		}

		public bool ExposeRoutingDomain
		{
			get
			{
				return this.exposeRoutingDomain;
			}
			set
			{
				this.exposeRoutingDomain = value;
			}
		}

		public bool IsProcessed
		{
			get
			{
				return this.Status == Status.Handled || this.Status == Status.Complete;
			}
		}

		public bool IsDelayDsnNeeded
		{
			get
			{
				return (this.Status == Status.Ready || this.Status == Status.Retry || this.Status == Status.Locked) && ((this.DsnRequested & DsnRequestedFlags.Delay) == DsnRequestedFlags.Delay || this.DsnRequested == DsnRequestedFlags.Default) && (this.DsnCompleted & DsnFlags.Delay) == DsnFlags.None;
			}
		}

		public RoutingAddress Email
		{
			get
			{
				return new RoutingAddress(this.PrivateEmail);
			}
			set
			{
				if (value == RoutingAddress.Empty)
				{
					throw new ArgumentException("Email property cannot be set to an empty RoutingAddress value.");
				}
				if (!value.IsValid)
				{
					throw new FormatException("Can not set Email property to an invalid RoutingAddress value.");
				}
				this.PrivateEmail = value.ToString();
			}
		}

		public IExtendedPropertyCollection ExtendedProperties
		{
			get
			{
				return this.storage.ExtendedProperties;
			}
		}

		[Obsolete("Use ExtendedProperties instead.")]
		public IDictionary<string, object> ExtendedPropertyDictionary
		{
			get
			{
				this.ThrowIfDeleted();
				return this.ExtendedProperties as IDictionary<string, object>;
			}
		}

		public string ORcpt
		{
			get
			{
				return this.storage.ORcpt;
			}
			set
			{
				this.storage.ORcpt = value;
			}
		}

		public AdminActionStatus AdminActionStatus
		{
			get
			{
				return this.storage.AdminActionStatus;
			}
			set
			{
				this.storage.AdminActionStatus = value;
			}
		}

		public bool IsActive
		{
			get
			{
				return this.storage.IsActive;
			}
		}

		public bool IsRowDeleted
		{
			get
			{
				return this.storage.IsDeleted;
			}
		}

		public bool IsInSafetyNet
		{
			get
			{
				return this.storage.IsInSafetyNet;
			}
		}

		public DsnRequestedFlags DsnRequested
		{
			get
			{
				return this.storage.DsnRequested;
			}
			set
			{
				this.storage.DsnRequested = value;
			}
		}

		public DsnFlags DsnNeeded
		{
			get
			{
				return this.storage.DsnNeeded;
			}
			set
			{
				this.storage.DsnNeeded = value;
			}
		}

		public DsnFlags DsnCompleted
		{
			get
			{
				return this.storage.DsnCompleted;
			}
			set
			{
				this.storage.DsnCompleted = value;
			}
		}

		public DsnParameters DsnParameters
		{
			get
			{
				return this.dsnParameters;
			}
		}

		public string DsnMessageId
		{
			get
			{
				return this.dsnMsgId;
			}
			set
			{
				this.ThrowIfDeleted();
				this.dsnMsgId = value;
			}
		}

		public bool SmtpDeferLogged
		{
			get
			{
				return this.smtpDeferLogged;
			}
		}

		public Status Status
		{
			get
			{
				return this.storage.Status;
			}
			set
			{
				this.storage.Status = value;
			}
		}

		public Destination DeliveredDestination
		{
			get
			{
				return this.storage.DeliveredDestination;
			}
			set
			{
				this.storage.DeliveredDestination = value;
			}
		}

		public string PrimaryServerFqdnGuid
		{
			get
			{
				return this.storage.PrimaryServerFqdnGuid;
			}
			set
			{
				this.storage.PrimaryServerFqdnGuid = value;
			}
		}

		public DateTime? DeliveryTime
		{
			get
			{
				return this.storage.DeliveryTime;
			}
			set
			{
				this.storage.DeliveryTime = value;
			}
		}

		public SmtpResponse SmtpResponse
		{
			get
			{
				return this.smtpResponse;
			}
			internal set
			{
				this.ThrowIfDeleted();
				this.smtpResponse = value;
			}
		}

		public int RetryCount
		{
			get
			{
				return this.storage.RetryCount;
			}
			internal set
			{
				this.storage.RetryCount = value;
			}
		}

		public AckStatus AckStatus
		{
			get
			{
				return this.ackStatus;
			}
		}

		public long MsgRecordId
		{
			get
			{
				return this.storage.MsgId;
			}
		}

		public NextHopSolutionKey NextHop
		{
			get
			{
				return this.nextHop;
			}
			set
			{
				this.nextHop = value;
			}
		}

		public long MsgId
		{
			get
			{
				return this.storage.MsgId;
			}
			private set
			{
				this.storage.MsgId = value;
			}
		}

		public bool PendingDatabaseUpdates
		{
			get
			{
				return this.storage.PendingDatabaseUpdates;
			}
		}

		public RoutingOverride RoutingOverride
		{
			get
			{
				this.ThrowIfDeleted();
				return this.routingOverride;
			}
		}

		public string OverrideSource { get; private set; }

		internal int OutboundIPPool
		{
			get
			{
				return this.storage.OutboundIPPool;
			}
			set
			{
				this.storage.OutboundIPPool = value;
			}
		}

		internal RequiredTlsAuthLevel? TlsAuthLevel
		{
			get
			{
				return this.storage.TlsAuthLevel;
			}
			set
			{
				this.storage.TlsAuthLevel = value;
			}
		}

		internal UnreachableReason UnreachableReason
		{
			get
			{
				return this.unreachableReason;
			}
			set
			{
				this.unreachableReason = value;
			}
		}

		internal MailRecipientType Type
		{
			get
			{
				return this.type;
			}
			set
			{
				this.type = value;
			}
		}

		internal string FinalDestination
		{
			get
			{
				return this.finalDestination;
			}
			set
			{
				this.finalDestination = value;
			}
		}

		internal OrganizationId MailItemScopeOrganizationId
		{
			get
			{
				return this.mailItem.OrganizationId;
			}
		}

		private string PrivateEmail
		{
			get
			{
				return this.storage.Email;
			}
			set
			{
				this.storage.Email = value;
			}
		}

		private int PrivateRetryCount
		{
			get
			{
				return this.storage.RetryCount;
			}
			set
			{
				this.storage.RetryCount = value;
			}
		}

		public static MailRecipient NewUnbound(string smtpAddress)
		{
			MailRecipient mailRecipient = new MailRecipient(null, TransportMailItem.Database.NewRecipientStorage(0L));
			mailRecipient.InitializeDefaults();
			mailRecipient.Email = new RoutingAddress(smtpAddress);
			return mailRecipient;
		}

		public void SetRoutingOverride(RoutingOverride routingOverride, string agentName, string overrideSource = null)
		{
			this.ThrowIfDeleted();
			if (!this.exposeRoutingDomain)
			{
				throw new InvalidOperationException(Strings.InvalidRoutingOverrideEvent);
			}
			if (this.routingOverride == null && routingOverride == null)
			{
				return;
			}
			string text = (routingOverride != null) ? routingOverride.AlternateDeliveryRoutingHostsString : string.Empty;
			if (this.routingOverride != null && routingOverride != null)
			{
				string alternateDeliveryRoutingHostsString = this.RoutingOverride.AlternateDeliveryRoutingHostsString;
				if (this.routingOverride.RoutingDomain.Equals(routingOverride.RoutingDomain) && alternateDeliveryRoutingHostsString.Equals(text, StringComparison.OrdinalIgnoreCase) && this.routingOverride.DeliveryQueueDomain.Equals(routingOverride.DeliveryQueueDomain))
				{
					return;
				}
			}
			this.routingOverride = routingOverride;
			this.OverrideSource = overrideSource;
			RoutingDomain redirectedConnectorDomain = (this.routingOverride == null) ? RoutingDomain.Empty : this.routingOverride.RoutingDomain;
			string redirectedDeliveryDestination = string.Empty;
			if (this.routingOverride != null)
			{
				switch (this.routingOverride.DeliveryQueueDomain)
				{
				case DeliveryQueueDomain.UseOverrideDomain:
					redirectedDeliveryDestination = redirectedConnectorDomain.ToString();
					break;
				case DeliveryQueueDomain.UseRecipientDomain:
					redirectedDeliveryDestination = this.Email.DomainPart;
					break;
				case DeliveryQueueDomain.UseAlternateDeliveryRoutingHosts:
					redirectedDeliveryDestination = text;
					break;
				}
			}
			MsgTrackRedirectInfo msgTrackInfo = new MsgTrackRedirectInfo(agentName, this.Email, redirectedConnectorDomain, redirectedDeliveryDestination, new long?(this.mailItem.MsgId));
			MessageTrackingLog.TrackRedirectToDomain(MessageTrackingSource.AGENT, this.mailItem, msgTrackInfo, this);
		}

		public void SetTlsDomain(string tlsDomain)
		{
			this.ThrowIfDeleted();
			SmtpDomainWithSubdomains smtpDomainWithSubdomains;
			if (!SmtpDomainWithSubdomains.TryParse(tlsDomain, out smtpDomainWithSubdomains))
			{
				throw new ArgumentException("Given TLS Domain is not valid.");
			}
			if (smtpDomainWithSubdomains.IsStar)
			{
				throw new ArgumentException("Given domain cannot be \"*\"");
			}
			this.tlsDomain = tlsDomain;
		}

		public void ResetTlsDomain()
		{
			this.tlsDomain = null;
		}

		public void ResetRoutingOverride()
		{
			this.routingOverride = null;
		}

		public void SetSmtpDeferLogged()
		{
			this.smtpDeferLogged = true;
		}

		public string GetTlsDomain()
		{
			this.ThrowIfDeleted();
			return this.tlsDomain;
		}

		public void AddDsnParameters(string key, object value)
		{
			if (this.dsnParameters == null)
			{
				this.dsnParameters = new DsnParameters();
			}
			this.dsnParameters[key] = value;
		}

		public void Ack(AckStatus ackStatus, SmtpResponse smtpResponse)
		{
			this.SmtpResponse = smtpResponse;
			switch (ackStatus)
			{
			case AckStatus.Pending:
				this.DsnNeeded = DsnFlags.None;
				this.Status = Status.Ready;
				this.ackStatus = ackStatus;
				return;
			case AckStatus.Success:
			case AckStatus.Expand:
			case AckStatus.Relay:
			case AckStatus.SuccessNoDsn:
				if (Components.Configuration.ProcessTransportRole == ProcessTransportRole.Hub)
				{
					Components.MessageResubmissionComponent.StoreRecipient(this);
				}
				if ((this.DsnRequested & DsnRequestedFlags.Success) == DsnRequestedFlags.Default || ackStatus == AckStatus.SuccessNoDsn)
				{
					this.DsnNeeded = DsnFlags.None;
					this.Status = Status.Complete;
				}
				else
				{
					if (ackStatus == AckStatus.Relay)
					{
						this.DsnNeeded = DsnFlags.Relay;
					}
					else if (ackStatus == AckStatus.Expand)
					{
						this.DsnNeeded = DsnFlags.Expansion;
					}
					else
					{
						this.DsnNeeded = DsnFlags.Delivery;
					}
					this.Status = Status.Handled;
				}
				this.ackStatus = ackStatus;
				return;
			case AckStatus.Retry:
				this.PrivateRetryCount++;
				this.Status = Status.Retry;
				this.ackStatus = ackStatus;
				return;
			case AckStatus.Fail:
				if ((this.DsnRequested & DsnRequestedFlags.Failure) != DsnRequestedFlags.Default || this.DsnRequested == DsnRequestedFlags.Default)
				{
					this.DsnNeeded = DsnFlags.Failure;
					this.Status = Status.Handled;
				}
				else
				{
					this.DsnNeeded = DsnFlags.None;
					if (!this.recipientIsOnPoisonMessage)
					{
						this.Status = Status.Complete;
					}
				}
				this.ackStatus = ackStatus;
				return;
			case AckStatus.Resubmit:
				this.DsnNeeded = DsnFlags.None;
				this.Status = Status.Retry;
				this.ackStatus = ackStatus;
				return;
			case AckStatus.Quarantine:
				this.DsnNeeded = DsnFlags.Quarantine;
				this.Status = Status.Handled;
				this.ackStatus = ackStatus;
				return;
			default:
				throw new ArgumentException("AckStatus can not be set to the specified value");
			}
		}

		public void MoveTo(TransportMailItem target)
		{
			if (target == null)
			{
				throw new ArgumentNullException("target");
			}
			this.storage = this.storage.MoveTo(target.MsgId);
			this.mailItem.Recipients.RemoveInternal(this);
			this.mailItem = target;
			target.Recipients.Add(this);
		}

		public MailRecipient CopyTo(TransportMailItem target)
		{
			if (target == null)
			{
				throw new ArgumentNullException("target");
			}
			MailRecipient mailRecipient = new MailRecipient(target, this.storage.CopyTo(target.MsgId));
			target.Recipients.Add(mailRecipient);
			return mailRecipient;
		}

		public void ReleaseFromActive()
		{
			if (this.IsActive)
			{
				this.Status = Status.Complete;
				this.storage.ReleaseFromActive();
			}
		}

		public void MarkToDelete()
		{
			this.Status = Status.Complete;
			this.storage.MarkToDelete();
		}

		public void AddToSafetyNet()
		{
			this.storage.AddToSafetyNet();
			this.PublishAddToSafetyNetEvent();
		}

		public void UpdateForPoisonMessage()
		{
			this.recipientIsOnPoisonMessage = true;
			this.DsnRequested = DsnRequestedFlags.Never;
		}

		public override string ToString()
		{
			return this.Email.ToString();
		}

		public Guid? GetMDBGuid()
		{
			MailboxItem mailboxItem = RecipientItem.Create(this) as MailboxItem;
			if (mailboxItem == null)
			{
				return null;
			}
			return new Guid?(mailboxItem.Database.ObjectGuid);
		}

		public string GetLastErrorDetails()
		{
			return new LastError(null, null, null, this.SmtpResponse).ToString();
		}

		internal static MailRecipient NewMessageRecipient(TransportMailItem mailItem, IMailRecipientStorage storageObject)
		{
			return new MailRecipient(mailItem, storageObject);
		}

		internal static MailRecipient NewMessageRecipient(TransportMailItem mailItem)
		{
			MailRecipient mailRecipient = new MailRecipient(mailItem, TransportMailItem.Database.NewRecipientStorage(mailItem.MsgId));
			mailRecipient.InitializeDefaults();
			mailRecipient.MsgId = mailItem.RecordId;
			mailItem.Recipients.Add(mailRecipient);
			return mailRecipient;
		}

		internal void Attach(TransportMailItem mailItem)
		{
			this.MsgId = mailItem.RecordId;
			this.mailItem = mailItem;
		}

		internal void Commit(Transaction transaction)
		{
			this.storage.Materialize(transaction);
		}

		internal void UpdateOwnerId()
		{
			this.MsgId = this.mailItem.RecordId;
		}

		internal void MinimizeMemory()
		{
			this.storage.MinimizeMemory();
		}

		private void InitializeDefaults()
		{
			this.PrivateEmail = string.Empty;
			this.DsnRequested = DsnRequestedFlags.Default;
			this.DsnNeeded = DsnFlags.None;
			this.DsnCompleted = DsnFlags.None;
			this.Status = Status.Ready;
			this.SmtpResponse = SmtpResponse.Empty;
			this.PrivateRetryCount = 0;
			this.AdminActionStatus = AdminActionStatus.None;
		}

		private void ThrowIfDeleted()
		{
			if (this.storage.IsDeleted)
			{
				throw new InvalidOperationException("operations not allowed on a deleted recipient");
			}
		}

		private void PublishAddToSafetyNetEvent()
		{
			if (this.mailItem.SystemProbeId != Guid.Empty && this.DeliveryTime != null && this.DeliveredDestination != null && this.DeliveredDestination.Type == Destination.DestinationType.Mdb)
			{
				EventNotificationItem eventNotificationItem = new EventNotificationItem(ExchangeComponent.Transport.Name, Components.MessageResubmissionComponent.GetDiagnosticComponentName(), null, ResultSeverityLevel.Verbose);
				eventNotificationItem.AddCustomProperty("Key", "Operation");
				eventNotificationItem.AddCustomProperty("Value", "SafetyNetAdd");
				eventNotificationItem.AddCustomProperty("DeliveryTime", this.DeliveryTime.Value.Ticks);
				eventNotificationItem.AddCustomProperty("TargetMdb", this.DeliveredDestination.ToGuid());
				eventNotificationItem.StateAttribute1 = this.mailItem.SystemProbeId.ToString();
				eventNotificationItem.Publish(false);
			}
		}

		private TransportMailItem mailItem;

		private NextHopSolutionKey nextHop;

		private bool exposeRoutingDomain;

		private RoutingOverride routingOverride;

		private string tlsDomain;

		private AckStatus ackStatus;

		private string dsnMsgId;

		private UnreachableReason unreachableReason;

		private MailRecipientType type;

		private string finalDestination = string.Empty;

		private SmtpResponse smtpResponse = SmtpResponse.Empty;

		private DsnParameters dsnParameters;

		private IMailRecipientStorage storage;

		private bool recipientIsOnPoisonMessage;

		private bool smtpDeferLogged;
	}
}
