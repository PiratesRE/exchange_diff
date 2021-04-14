using System;
using System.Linq;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.Services;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal sealed class AdParticipantLookup
	{
		public AdParticipantLookup(CallContext callContext, int maxParticipantsToResolve = 2147483647)
		{
			this.callContext = callContext;
			this.maxBatchSizeToAdResolve = ((maxParticipantsToResolve > 0) ? maxParticipantsToResolve : int.MaxValue);
		}

		public IParticipant[] LookUpAdParticipants(IParticipant[] pregatherParticipants)
		{
			IParticipant[] convertedParticipants = new IParticipant[0];
			if (RequestDetailsLogger.Current != null)
			{
				RequestDetailsLogger.Current.TrackLatency<IParticipant[]>(EwsMetadata.ParticipantResolveLatency, () => convertedParticipants = this.InternalLookUpAdParticipants(pregatherParticipants));
			}
			else
			{
				convertedParticipants = this.InternalLookUpAdParticipants(pregatherParticipants);
			}
			return convertedParticipants;
		}

		private IParticipant[] InternalLookUpAdParticipants(IParticipant[] participantsToConvert)
		{
			if (!participantsToConvert.Any<IParticipant>())
			{
				return new IParticipant[0];
			}
			IParticipant[] participantsSentToBeConverted = participantsToConvert.Take(this.maxBatchSizeToAdResolve).ToArray<IParticipant>();
			ExTraceGlobals.ParticipantLookupBatchingTracer.TraceDebug<int, int>(0L, "AdParticipantLookup.InternalLookUpAdParticipants - {0} participants identified as needed to resolve. The set may be trimmed due to MaxBatchSizeToAdResolve({1})", participantsToConvert.Length, this.maxBatchSizeToAdResolve);
			IParticipant[] result;
			if (this.TryAdResolve(participantsSentToBeConverted, out result))
			{
				return result;
			}
			ExTraceGlobals.ParticipantLookupBatchingTracer.TraceDebug(0L, "AdParticipantLookup.InternalLookUpAdParticipants - AD resolution didnt succeed. Or the AdRecipientSession was not present or the call returned null as result");
			return new IParticipant[0];
		}

		private bool TryAdResolve(IParticipant[] participantsSentToBeConverted, out IParticipant[] convertedParticipants)
		{
			convertedParticipants = null;
			IRecipientSession recipientSession = (this.callContext != null) ? this.callContext.ADRecipientSessionContext.GetADRecipientSession() : null;
			if (recipientSession == null)
			{
				return false;
			}
			convertedParticipants = Participant.TryConvertTo(participantsSentToBeConverted.Cast<Participant>().ToArray<Participant>(), "SMTP", recipientSession);
			return convertedParticipants != null;
		}

		private readonly int maxBatchSizeToAdResolve;

		private readonly CallContext callContext;
	}
}
