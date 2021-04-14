using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Principal;
using Microsoft.Exchange.Diagnostics.Components.Services;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal class ParticipantInformation
	{
		internal ParticipantInformation(string displayName, string routingType, string emailAddress, ParticipantOrigin participantOrigin, bool? demoted, string sipUri, bool? submitted) : this(displayName, routingType, emailAddress, participantOrigin, demoted, sipUri, submitted, null)
		{
		}

		internal ParticipantInformation(string displayName, string routingType, string emailAddress, ParticipantOrigin participantOrigin, bool? demoted, string sipUri, bool? submitted, MailboxHelper.MailboxTypeType? mailboxType)
		{
			ExTraceGlobals.ParticipantLookupBatchingTracer.TraceDebug((long)this.GetHashCode(), "ParticipantInformation constructed - Hashcode = {0}; DisplayName = {1}; RoutingType = {2}; EmailAddress = {3}; ParticipantOrigin (type) = {4}; Demoted = {5}; SipUri = {6}; Submitted = {7}", new object[]
			{
				this.GetHashCode(),
				displayName,
				routingType,
				emailAddress,
				participantOrigin,
				(demoted == null) ? "<NULL>" : demoted.Value.ToString(),
				sipUri ?? string.Empty,
				(submitted == null) ? "<NULL>" : submitted.Value.ToString()
			});
			this.displayName = displayName;
			this.routingType = routingType;
			this.emailAddress = emailAddress;
			this.participantOrigin = participantOrigin;
			this.demoted = demoted;
			this.sipUri = sipUri;
			this.Submitted = submitted;
			this.mailboxType = mailboxType;
		}

		internal string DisplayName
		{
			get
			{
				return this.displayName;
			}
		}

		internal string RoutingType
		{
			get
			{
				return this.routingType;
			}
		}

		internal string EmailAddress
		{
			get
			{
				return this.emailAddress;
			}
		}

		internal ParticipantOrigin Origin
		{
			get
			{
				return this.participantOrigin;
			}
		}

		internal bool? Demoted
		{
			get
			{
				return this.demoted;
			}
		}

		internal string SipUri
		{
			get
			{
				return this.sipUri;
			}
		}

		internal bool? Submitted { get; private set; }

		internal MailboxHelper.MailboxTypeType MailboxType
		{
			get
			{
				if (this.mailboxType == null)
				{
					this.mailboxType = new MailboxHelper.MailboxTypeType?(MailboxHelper.GetMailboxType(this.Origin, this.RoutingType));
				}
				return this.mailboxType.Value;
			}
		}

		internal static ParticipantInformation CreateSmtpParticipant(IParticipant participant, IExchangePrincipal mailboxIndentity)
		{
			MailboxHelper.MailboxTypeType mailboxTypeType = MailboxHelper.ConvertToMailboxType(mailboxIndentity.RecipientType, mailboxIndentity.RecipientTypeDetails);
			RemoteUserMailboxPrincipal remoteUserMailboxPrincipal = CallContext.Current.AccessingPrincipal as RemoteUserMailboxPrincipal;
			if (remoteUserMailboxPrincipal != null)
			{
				return ParticipantInformation.CreateSmtpParticipant(participant, remoteUserMailboxPrincipal.DisplayName, remoteUserMailboxPrincipal.PrimarySmtpAddress.ToString(), mailboxTypeType);
			}
			return ParticipantInformation.CreateSmtpParticipant(participant, mailboxIndentity.MailboxInfo.DisplayName, mailboxIndentity.MailboxInfo.PrimarySmtpAddress.ToString(), mailboxTypeType);
		}

		internal static ParticipantInformation Create(IParticipant participant, ExchangePrincipal mailboxIndentity)
		{
			string text = participant.SipUri;
			MailboxHelper.MailboxTypeType value = MailboxHelper.ConvertToMailboxType(mailboxIndentity.RecipientType, mailboxIndentity.RecipientTypeDetails);
			RemoteUserMailboxPrincipal remoteUserMailboxPrincipal = CallContext.Current.AccessingPrincipal as RemoteUserMailboxPrincipal;
			if (remoteUserMailboxPrincipal != null)
			{
				return new ParticipantInformation(remoteUserMailboxPrincipal.DisplayName, "SMTP", remoteUserMailboxPrincipal.PrimarySmtpAddress.ToString(), participant.Origin, null, text, new bool?(participant.Submitted), new MailboxHelper.MailboxTypeType?(value));
			}
			return new ParticipantInformation(mailboxIndentity.MailboxInfo.DisplayName, "SMTP", mailboxIndentity.MailboxInfo.PrimarySmtpAddress.ToString(), participant.Origin, null, text, new bool?(participant.Submitted), new MailboxHelper.MailboxTypeType?(value));
		}

		internal static ParticipantInformation CreateSmtpParticipant(IParticipant participant, string displayName, string smtpAddress, MailboxHelper.MailboxTypeType mailboxType)
		{
			return new ParticipantInformation(displayName, "SMTP", smtpAddress, participant.Origin, null, participant.SipUri, new bool?(participant.Submitted), new MailboxHelper.MailboxTypeType?(mailboxType));
		}

		internal static ParticipantInformation Create(IParticipant participant, MailboxHelper.MailboxTypeType mailboxType, bool? demoted = null)
		{
			if (participant.SmtpEmailAddress != null)
			{
				return new ParticipantInformation(participant.DisplayName, "SMTP", participant.SmtpEmailAddress, participant.Origin, new bool?(true), participant.SipUri, new bool?(participant.Submitted), new MailboxHelper.MailboxTypeType?(mailboxType));
			}
			return new ParticipantInformation(participant.DisplayName, participant.RoutingType, participant.EmailAddress, participant.Origin, demoted, participant.SipUri, new bool?(participant.Submitted), new MailboxHelper.MailboxTypeType?(mailboxType));
		}

		private readonly string displayName;

		private readonly string routingType;

		private readonly string emailAddress;

		private readonly ParticipantOrigin participantOrigin;

		private readonly bool? demoted;

		private readonly string sipUri;

		private MailboxHelper.MailboxTypeType? mailboxType;
	}
}
