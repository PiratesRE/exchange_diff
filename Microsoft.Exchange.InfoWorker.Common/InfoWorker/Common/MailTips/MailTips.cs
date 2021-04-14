using System;
using System.Globalization;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.InfoWorker.MailTips;
using Microsoft.Exchange.InfoWorker.Common.Availability;
using Microsoft.Exchange.InfoWorker.Common.OrganizationConfiguration;

namespace Microsoft.Exchange.InfoWorker.Common.MailTips
{
	internal class MailTips
	{
		public MailTips(EmailAddress emailAddress)
		{
			this.emailAddress = emailAddress;
			this.unavailableMailTips = MailTipTypes.All;
		}

		public MailTips(RecipientData recipientData) : this(recipientData.EmailAddress)
		{
			this.recipientData = recipientData;
			if (!recipientData.IsEmpty)
			{
				this.configuration = CachedOrganizationConfiguration.GetInstance(recipientData.OrganizationId, CachedOrganizationConfiguration.ConfigurationTypes.All);
				return;
			}
			this.permission = MailTipsPermission.AllAccess;
		}

		public MailTips(EmailAddress emailAddress, Exception exception) : this(emailAddress)
		{
			this.exception = exception;
		}

		internal MailTips(EmailAddress emailAddress, MailTipTypes unavailableMailTips, MailTipTypes pendingMailTips)
		{
			this.emailAddress = emailAddress;
			this.unavailableMailTips = unavailableMailTips;
			this.pendingMailTips = pendingMailTips;
		}

		public EmailAddress EmailAddress
		{
			get
			{
				return this.emailAddress;
			}
		}

		public RecipientData RecipientData
		{
			get
			{
				return this.recipientData;
			}
			internal set
			{
				this.recipientData = value;
			}
		}

		public MailTipTypes UnavailableMailTips
		{
			get
			{
				return this.unavailableMailTips;
			}
		}

		public MailTipTypes PendingMailTips
		{
			get
			{
				return this.pendingMailTips;
			}
		}

		public string OutOfOfficeMessage
		{
			get
			{
				return this.outOfOfficeMessage;
			}
			set
			{
				this.outOfOfficeMessage = value;
				this.MarkAsAvailable(MailTipTypes.OutOfOfficeMessage);
			}
		}

		public string OutOfOfficeMessageLanguage
		{
			get
			{
				return this.outOfOfficeMessageLanguage;
			}
			set
			{
				this.outOfOfficeMessageLanguage = value;
			}
		}

		public Duration OutOfOfficeDuration
		{
			get
			{
				return this.outOfOfficeDuration;
			}
			set
			{
				this.outOfOfficeDuration = value;
			}
		}

		public bool MailboxFull
		{
			get
			{
				return this.mailboxFull;
			}
			set
			{
				this.mailboxFull = value;
				this.MarkAsAvailable(MailTipTypes.MailboxFullStatus);
			}
		}

		public string CustomMailTip
		{
			get
			{
				return this.customMailTip;
			}
			set
			{
				this.customMailTip = value;
				this.MarkAsAvailable(MailTipTypes.CustomMailTip);
			}
		}

		public int TotalMemberCount
		{
			get
			{
				return this.totalMemberCount;
			}
			set
			{
				this.totalMemberCount = value;
				this.MarkAsAvailable(MailTipTypes.TotalMemberCount);
			}
		}

		public int ExternalMemberCount
		{
			get
			{
				return this.externalMemberCount;
			}
			set
			{
				this.externalMemberCount = value;
				this.MarkAsAvailable(MailTipTypes.ExternalMemberCount);
			}
		}

		public bool DeliveryRestricted
		{
			get
			{
				return this.deliveryRestricted;
			}
			set
			{
				this.deliveryRestricted = value;
				this.MarkAsAvailable(MailTipTypes.DeliveryRestriction);
			}
		}

		public bool IsModerated
		{
			get
			{
				return this.isModerated;
			}
			set
			{
				this.isModerated = value;
				this.MarkAsAvailable(MailTipTypes.ModerationStatus);
			}
		}

		public bool InvalidRecipient
		{
			get
			{
				return this.invalidRecipient;
			}
			set
			{
				this.invalidRecipient = value;
				this.MarkAsAvailable(MailTipTypes.InvalidRecipient);
			}
		}

		public ScopeTypes Scope
		{
			get
			{
				return this.scope;
			}
			set
			{
				this.scope = value;
				this.MarkAsAvailable(MailTipTypes.Scope);
			}
		}

		public int MaxMessageSize
		{
			get
			{
				return this.maxMessageSize;
			}
			set
			{
				this.maxMessageSize = value;
				this.MarkAsAvailable(MailTipTypes.MaxMessageSize);
			}
		}

		public Exception Exception
		{
			get
			{
				return this.exception;
			}
			internal set
			{
				this.exception = value;
			}
		}

		public bool NeedMerge
		{
			get
			{
				return this.needMerge;
			}
			internal set
			{
				this.needMerge = value;
			}
		}

		public CachedOrganizationConfiguration Configuration
		{
			get
			{
				return this.configuration;
			}
			set
			{
				this.configuration = value;
			}
		}

		public MailTipsPermission Permission
		{
			get
			{
				return this.permission;
			}
			set
			{
				this.permission = value;
			}
		}

		public bool IsAvailable(MailTipTypes mailTipType)
		{
			return (this.unavailableMailTips & mailTipType) == MailTipTypes.None;
		}

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "EmailAddress={0}, Size={1}, ExternalSize={2}, Restricted={3}, Invalid={4}, Moderated={5}, MaxMessageSize={6}, Full={7}, Custom={8}, OOFDuration:{9}, OOFLanguage={10}, OOF={11}, Exception={12}", new object[]
			{
				this.emailAddress,
				this.totalMemberCount,
				this.externalMemberCount,
				this.deliveryRestricted,
				this.invalidRecipient,
				this.isModerated,
				this.maxMessageSize,
				this.mailboxFull,
				this.customMailTip,
				this.outOfOfficeDuration,
				this.outOfOfficeMessageLanguage,
				this.outOfOfficeMessage,
				this.exception
			});
		}

		internal void MarkAsPending(MailTipTypes mailTips)
		{
			lock (this.flagAccessSynchronizer)
			{
				this.unavailableMailTips &= ~mailTips;
				this.pendingMailTips |= mailTips;
			}
		}

		internal void MarkAsUnavailable(MailTipTypes mailTipType)
		{
			lock (this.flagAccessSynchronizer)
			{
				this.unavailableMailTips |= mailTipType;
				this.pendingMailTips &= ~mailTipType;
			}
		}

		private void MarkAsAvailable(MailTipTypes mailTipType)
		{
			lock (this.flagAccessSynchronizer)
			{
				this.unavailableMailTips &= ~mailTipType;
				this.pendingMailTips &= ~mailTipType;
			}
		}

		public const string EventSource = "MSExchange MailTips";

		public static readonly Trace GetMailTipsTracer = ExTraceGlobals.GetMailTipsTracer;

		public static readonly ExEventLog Logger = new ExEventLog(MailTips.GetMailTipsTracer.Category, "MSExchange MailTips");

		private EmailAddress emailAddress;

		private RecipientData recipientData;

		private MailTipTypes unavailableMailTips;

		private MailTipTypes pendingMailTips;

		private string outOfOfficeMessage;

		private string outOfOfficeMessageLanguage;

		private Duration outOfOfficeDuration;

		private bool mailboxFull;

		private string customMailTip;

		private int totalMemberCount;

		private int externalMemberCount;

		private bool invalidRecipient;

		private ScopeTypes scope;

		private int maxMessageSize;

		private bool deliveryRestricted;

		private bool isModerated;

		private object flagAccessSynchronizer = new object();

		private Exception exception;

		private bool needMerge;

		private CachedOrganizationConfiguration configuration;

		private MailTipsPermission permission;
	}
}
