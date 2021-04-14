using System;
using System.Globalization;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Principal;
using Microsoft.Exchange.Inference.Common;
using Microsoft.Exchange.Search.Core.Abstraction;
using Microsoft.Exchange.Search.Core.Common;

namespace Microsoft.Exchange.Inference.Mdb
{
	[Serializable]
	internal class MdbRecipient : IMessageRecipient, IEquatable<IMessageRecipient>
	{
		public MdbRecipient(Recipient recipient) : this(recipient.Participant)
		{
		}

		public MdbRecipient(Participant participant)
		{
			Util.ThrowOnNullArgument(participant, "participant");
			this.displayName = participant.DisplayName;
			this.emailAddress = participant.EmailAddress;
			this.routingType = participant.RoutingType;
			this.smtpAddress = participant.GetValueOrDefault<string>(ParticipantSchema.SmtpAddress);
			if (string.IsNullOrEmpty(this.smtpAddress) && this.routingType == "SMTP")
			{
				this.smtpAddress = this.emailAddress;
			}
			this.sipUri = participant.GetValueOrDefault<string>(ParticipantSchema.SipUri);
			this.alias = participant.GetValueOrDefault<string>(ParticipantSchema.Alias);
			this.isDistributionList = MdbRecipient.IsDistributionListParticipant(participant);
			this.recipientDisplayType = participant.GetValueOrDefault<RecipientDisplayType>(ParticipantSchema.DisplayTypeEx);
			this.ComputeIdentity();
		}

		public MdbRecipient(IExchangePrincipal principal, CultureInfo culture)
		{
			Util.ThrowOnNullArgument(principal, "principal");
			Participant participant = new Participant(principal);
			this.displayName = participant.DisplayName;
			this.emailAddress = participant.EmailAddress;
			this.routingType = participant.RoutingType;
			this.smtpAddress = participant.GetValueOrDefault<string>(ParticipantSchema.SmtpAddress);
			if (string.IsNullOrEmpty(this.smtpAddress) && this.routingType == "SMTP")
			{
				this.smtpAddress = this.emailAddress;
			}
			this.sipUri = participant.GetValueOrDefault<string>(ParticipantSchema.SipUri);
			this.alias = participant.GetValueOrDefault<string>(ParticipantSchema.Alias);
			this.isDistributionList = MdbRecipient.IsDistributionListParticipant(participant);
			this.recipientDisplayType = participant.GetValueOrDefault<RecipientDisplayType>(ParticipantSchema.DisplayTypeEx);
			this.organizationalId = principal.MailboxInfo.OrganizationId;
			this.cultureInfo = culture;
			this.ComputeIdentity();
		}

		public MdbRecipient(IMessageRecipient recipient)
		{
			Util.ThrowOnNullArgument(recipient, "recipient");
			this.displayName = recipient.DisplayName;
			this.emailAddress = recipient.EmailAddress;
			this.routingType = recipient.RoutingType;
			this.smtpAddress = recipient.SmtpAddress;
			if (string.IsNullOrEmpty(this.smtpAddress) && this.routingType == "SMTP")
			{
				this.smtpAddress = this.emailAddress;
			}
			this.sipUri = recipient.SipUri;
			this.alias = recipient.Alias;
			this.isDistributionList = recipient.IsDistributionList;
			this.recipientDisplayType = recipient.RecipientDisplayType;
			this.ComputeIdentity();
		}

		public MdbRecipient(string smtpAddress, string emailAddress, string displayName)
		{
			this.smtpAddress = smtpAddress;
			this.emailAddress = emailAddress;
			this.displayName = displayName;
			this.routingType = "EX";
			this.isDistributionList = false;
			this.recipientDisplayType = RecipientDisplayType.MailboxUser;
			this.ComputeIdentity();
		}

		public IIdentity Identity
		{
			get
			{
				return this.identity;
			}
		}

		public string DisplayName
		{
			get
			{
				return this.displayName;
			}
		}

		public string EmailAddress
		{
			get
			{
				return this.emailAddress;
			}
		}

		public string SmtpAddress
		{
			get
			{
				return this.smtpAddress;
			}
		}

		public string RoutingType
		{
			get
			{
				return this.routingType;
			}
		}

		public string SipUri
		{
			get
			{
				return this.sipUri;
			}
		}

		public string Alias
		{
			get
			{
				return this.alias;
			}
		}

		public OrganizationId OrganizationalId
		{
			get
			{
				return this.organizationalId;
			}
		}

		public CultureInfo CultureInformation
		{
			get
			{
				return this.cultureInfo;
			}
		}

		public bool IsDistributionList
		{
			get
			{
				return this.isDistributionList;
			}
		}

		public RecipientDisplayType RecipientDisplayType
		{
			get
			{
				return this.recipientDisplayType;
			}
		}

		public static bool operator ==(MdbRecipient left, MdbRecipient right)
		{
			return left == right || (left != null && right != null && left.Equals(right));
		}

		public static bool operator !=(MdbRecipient left, MdbRecipient right)
		{
			return !(left == right);
		}

		public override bool Equals(object comparand)
		{
			return comparand is IMessageRecipient && this.Equals((IMessageRecipient)comparand);
		}

		public bool Equals(IMessageRecipient other)
		{
			return other != null && this.Identity.Equals(other.Identity);
		}

		public override int GetHashCode()
		{
			return this.Identity.GetHashCode();
		}

		public virtual void UpdateFromRecipient(IMessageRecipient recipient)
		{
			IIdentity identity = this.identity;
			this.displayName = recipient.DisplayName;
			this.emailAddress = recipient.EmailAddress;
			this.routingType = recipient.RoutingType;
			this.smtpAddress = recipient.SmtpAddress;
			this.sipUri = recipient.SipUri;
			this.alias = recipient.Alias;
			this.isDistributionList = recipient.IsDistributionList;
			this.recipientDisplayType = recipient.RecipientDisplayType;
			this.ComputeIdentity();
			Util.ThrowOnConditionFailed(identity.Equals(this.Identity), "Updates of a recipient are not allowed to change the recipient identity");
		}

		private static bool IsDistributionListParticipant(Participant participant)
		{
			bool? valueAsNullable = participant.GetValueAsNullable<bool>(ParticipantSchema.IsDistributionList);
			return (valueAsNullable != null && valueAsNullable.Value) || 0 == string.CompareOrdinal(participant.RoutingType, "MAPIPDL");
		}

		private void ComputeIdentity()
		{
			this.identity = new MdbRecipientIdentity(this.smtpAddress);
		}

		[NonSerialized]
		private readonly OrganizationId organizationalId;

		[NonSerialized]
		private readonly CultureInfo cultureInfo;

		private string smtpAddress;

		private string routingType;

		private bool isDistributionList;

		private string displayName;

		private string emailAddress;

		private string sipUri;

		private string alias;

		private RecipientDisplayType recipientDisplayType;

		private MdbRecipientIdentity identity;
	}
}
