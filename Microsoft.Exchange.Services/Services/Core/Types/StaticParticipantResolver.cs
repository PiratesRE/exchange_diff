using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.Services.Core.DataConverter;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal sealed class StaticParticipantResolver : IParticipantResolver
	{
		public static StaticParticipantResolver DefaultInstance
		{
			get
			{
				if (StaticParticipantResolver.defaultInstance == null)
				{
					StaticParticipantResolver.defaultInstance = new StaticParticipantResolver(int.MaxValue);
				}
				return StaticParticipantResolver.defaultInstance;
			}
		}

		public StaticParticipantResolver(int maxParticipantsToResolve)
		{
			this.maxBatchSizeToAdResolve = maxParticipantsToResolve;
		}

		public EmailAddressWrapper[] ResolveToEmailAddressWrapper(IEnumerable<IParticipant> participants)
		{
			HashSet<EmailAddressWrapper> hashSet = new HashSet<EmailAddressWrapper>(EmailAddressWrapper.AddressEqualityComparer);
			hashSet.UnionWith(from p in participants
			select EmailAddressWrapper.FromParticipant(p, EWSSettings.ParticipantInformation));
			return hashSet.ToArray<EmailAddressWrapper>();
		}

		public SingleRecipientType ResolveToSingleRecipientType(IParticipant participant)
		{
			if (participant == null)
			{
				return null;
			}
			ParticipantInformation participantInformation;
			if (EWSSettings.ParticipantInformation.TryGetParticipant(participant, out participantInformation))
			{
				return PropertyCommand.CreateRecipientFromParticipant(participantInformation);
			}
			return PropertyCommand.CreateOneOffRecipientFromParticipantSmtpAddress(participant);
		}

		public SmtpAddress ResolveToSmtpAddress(IParticipant participant)
		{
			this.LoadAdDataIfNeeded(new List<IParticipant>
			{
				participant
			});
			ParticipantInformation participantInformation2;
			ParticipantInformation participantInformation = EWSSettings.ParticipantInformation.TryGetParticipant(participant, out participantInformation2) ? participantInformation2 : null;
			string address = (participantInformation != null) ? participantInformation.EmailAddress : string.Empty;
			return new SmtpAddress(address);
		}

		public SingleRecipientType[] ResolveToSingleRecipientType(IEnumerable<IParticipant> participants)
		{
			List<SingleRecipientType> list = new List<SingleRecipientType>(participants.Count<IParticipant>());
			foreach (IParticipant participant in participants)
			{
				list.Add(this.ResolveToSingleRecipientType(participant));
			}
			return list.ToArray();
		}

		public void LoadAdDataIfNeeded(IEnumerable<IParticipant> pregatherParticipants)
		{
			if (RequestDetailsLogger.Current != null)
			{
				RequestDetailsLogger.Current.TrackLatency(EwsMetadata.ParticipantResolveLatency, delegate()
				{
					this.ConvertAndGetParticipantInformation(EWSSettings.ParticipantInformation, pregatherParticipants.Cast<IParticipant>().ToList<IParticipant>());
				});
				return;
			}
			this.ConvertAndGetParticipantInformation(EWSSettings.ParticipantInformation, pregatherParticipants.Cast<IParticipant>().ToList<IParticipant>());
		}

		private void ConvertAndGetParticipantInformation(ParticipantInformationDictionary participantInformationDictionary, IEnumerable<IParticipant> participants)
		{
			if (!participants.Any<IParticipant>())
			{
				return;
			}
			HashSet<Participant> hashSet = new HashSet<Participant>(ParticipantComparer.EmailAddress);
			ExchangePrincipal exchangePrincipal = null;
			Participant participant = null;
			CallContext callContext = CallContext.Current;
			if (callContext != null)
			{
				exchangePrincipal = callContext.MailboxIdentityPrincipal;
				if (exchangePrincipal != null)
				{
					participant = new Participant(exchangePrincipal);
				}
			}
			foreach (IParticipant participant2 in participants)
			{
				Participant participant3 = (Participant)participant2;
				ParticipantInformation participantInformation = null;
				MailboxHelper.MailboxTypeType? mailboxTypeType = null;
				if (participant3 == null)
				{
					ExTraceGlobals.ParticipantLookupBatchingTracer.TraceDebug(0L, "ToServiceObjectPropertyList.ConvertAndGetParticipantInformation - found null entry");
				}
				else if (participantInformationDictionary.ContainsParticipant(participant3))
				{
					ExTraceGlobals.ParticipantLookupBatchingTracer.TraceDebug<string, int>(0L, "ToServiceObjectPropertyList.ConvertAndGetParticipantInformation - using already resolved participant for EmailAddress = {0}, HashCode = {1}", participant3.EmailAddress, participant3.GetHashCode());
				}
				else
				{
					bool flag = false;
					if (Global.FastParticipantResolveEnabled && string.Equals(participant3.RoutingType, "EX", StringComparison.OrdinalIgnoreCase))
					{
						string text = participant3.TryGetProperty(ParticipantSchema.SmtpAddress) as string;
						string key = null;
						if (!string.IsNullOrEmpty(text) && callContext != null && MailboxTypeCache.TryGetKey(text, callContext.OrganizationalUnitName, out key) && MailboxTypeCache.Instance.TryGetValue(key, out mailboxTypeType))
						{
							ExTraceGlobals.ParticipantLookupBatchingTracer.TraceDebug<string, int>(0L, "[ParticipantInformation::TryFastResolution] Found cached SMTP address and MailboxType: EmailAddress = {0}, HashCode = {1}", text, participant3.GetHashCode());
							string sipUri = participant3.SipUri;
							participantInformation = new ParticipantInformation(participant3.DisplayName, "SMTP", text, participant3.Origin, null, sipUri, new bool?(participant3.Submitted), new MailboxHelper.MailboxTypeType?(mailboxTypeType.Value));
							flag = true;
						}
					}
					if (!flag && participant != null && ParticipantComparer.EmailAddress.Equals(participant3, participant))
					{
						participantInformation = ParticipantInformation.Create(participant3, exchangePrincipal);
						flag = true;
					}
					if (!flag)
					{
						if (string.Equals(participant3.RoutingType, "EX", StringComparison.OrdinalIgnoreCase))
						{
							if (hashSet.Add(participant3))
							{
								ExTraceGlobals.ParticipantLookupBatchingTracer.TraceDebug<string, int>(0L, "ToServiceObjectPropertyList.CheckAndAddParticipantToConvert - converting participant with EmailAddress = {0} and HashCode = {1}.", participant3.EmailAddress, participant3.GetHashCode());
							}
						}
						else
						{
							string sipUri2 = participant3.SipUri;
							participantInformation = new ParticipantInformation(participant3.DisplayName, participant3.RoutingType, participant3.EmailAddress, participant3.Origin, null, sipUri2, new bool?(participant3.Submitted), new MailboxHelper.MailboxTypeType?(MailboxHelper.GetMailboxType(participant3.Origin, participant3.RoutingType)));
						}
					}
					if (participantInformation != null)
					{
						participantInformationDictionary.AddParticipant(participant3, participantInformation);
					}
				}
			}
			if (hashSet.Count == 0)
			{
				return;
			}
			ExTraceGlobals.ParticipantLookupBatchingTracer.TraceDebug<int>(0L, "ToServiceObjectPropertyList.ConvertAndGetParticipantInformation - now calling TryConvertTo on {0} participants", hashSet.Count);
			Participant[] array = hashSet.Take(this.maxBatchSizeToAdResolve).ToArray<Participant>();
			Participant[] array2 = MailboxHelper.TryConvertTo(array, "SMTP");
			int num = 0;
			foreach (Participant participant4 in array2)
			{
				Participant participant5;
				bool value;
				if (participant4 == null || participant4.EmailAddress == null)
				{
					participant5 = array[num];
					value = true;
				}
				else
				{
					participant5 = participant4;
					value = false;
				}
				MailboxHelper.MailboxTypeType mailboxType = MailboxHelper.GetMailboxType(participant5.Origin, participant5.RoutingType);
				string key2 = null;
				if (Global.FastParticipantResolveEnabled && callContext != null && !string.IsNullOrEmpty(participant5.SmtpEmailAddress) && MailboxTypeCache.TryGetKey(participant5.SmtpEmailAddress, callContext.OrganizationalUnitName, out key2))
				{
					MailboxTypeCache.Instance.TryAdd(key2, mailboxType);
				}
				ParticipantInformation participantInformation2 = new ParticipantInformation(participant5.DisplayName, participant5.RoutingType, participant5.EmailAddress, participant5.Origin, new bool?(value), participant5.SipUri, new bool?(participant5.Submitted), new MailboxHelper.MailboxTypeType?(mailboxType));
				participantInformationDictionary.AddParticipant(array[num], participantInformation2);
				num++;
			}
		}

		private static StaticParticipantResolver defaultInstance;

		private readonly int maxBatchSizeToAdResolve;
	}
}
