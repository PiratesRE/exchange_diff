using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.Services;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal class ParticipantInformationDictionary
	{
		internal ParticipantInformationDictionary()
		{
			ExTraceGlobals.ParticipantLookupBatchingTracer.TraceDebug<int>((long)this.GetHashCode(), "ParticipantInformationDictionary constructed. Hashcode = {0}", this.GetHashCode());
		}

		internal bool ContainsParticipant(IParticipant participant)
		{
			return this.dictionary.ContainsKey(participant);
		}

		internal void AddParticipant(IParticipant participant, ParticipantInformation participantInformation)
		{
			if (!this.dictionary.ContainsKey(participant))
			{
				this.dictionary.Add(participant, participantInformation);
			}
		}

		internal ParticipantInformation GetParticipant(IParticipant participant)
		{
			return this.dictionary[participant];
		}

		internal ParticipantInformation GetParticipantInformationOrCreateNew(IParticipant participant)
		{
			ParticipantInformation participantInformation;
			if (!this.TryGetParticipant(participant, out participantInformation))
			{
				ExTraceGlobals.CommonAlgorithmTracer.TraceDebug<string>((long)this.GetHashCode(), "Participant is null. Name='{0}';", participant.DisplayName);
				participantInformation = ParticipantInformationDictionary.ConvertToParticipantInformation(participant);
				this.AddParticipant(participant, participantInformation);
			}
			return participantInformation;
		}

		internal bool TryGetParticipantFromDictionary(IParticipant participant, out ParticipantInformation participantInformation)
		{
			return this.dictionary.TryGetValue(participant, out participantInformation) && participantInformation != null;
		}

		internal bool TryGetParticipant(IParticipant participant, out ParticipantInformation participantInformation)
		{
			bool flag = this.dictionary.TryGetValue(participant, out participantInformation) && participantInformation != null;
			if (flag && participantInformation.DisplayName != null && participant.DisplayName != null && string.Compare(participantInformation.DisplayName, participant.DisplayName, false) != 0)
			{
				ExTraceGlobals.CommonAlgorithmTracer.TraceDebug(0L, string.Format("ParticipantInformationDictionary.TryGetParticipant - Using display name {0} for emailAddress {1}", participant.DisplayName, participantInformation.EmailAddress));
				ParticipantInformation participantInformation2 = new ParticipantInformation(participant.DisplayName, participantInformation.RoutingType, participantInformation.EmailAddress, participantInformation.Origin, participantInformation.Demoted, participantInformation.SipUri, participantInformation.Submitted, new MailboxHelper.MailboxTypeType?(participantInformation.MailboxType));
				participantInformation = participantInformation2;
			}
			return flag;
		}

		internal static ParticipantInformation ConvertToParticipantInformation(IParticipant participant)
		{
			if (participant.SmtpEmailAddress != null)
			{
				return new ParticipantInformation(participant.DisplayName, "SMTP", participant.SmtpEmailAddress, participant.Origin, new bool?(true), participant.SipUri, new bool?(participant.Submitted));
			}
			return new ParticipantInformation(participant.DisplayName, participant.RoutingType, participant.EmailAddress, participant.Origin, new bool?(true), participant.SipUri, new bool?(participant.Submitted));
		}

		private readonly Dictionary<IParticipant, ParticipantInformation> dictionary = new Dictionary<IParticipant, ParticipantInformation>(ParticipantComparer.EmailAddress);
	}
}
