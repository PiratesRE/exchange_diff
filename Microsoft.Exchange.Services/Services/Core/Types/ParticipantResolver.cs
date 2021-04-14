using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Principal;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.Services.Core.DataConverter;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal sealed class ParticipantResolver : IParticipantResolver
	{
		private ParticipantResolver(ParticipantInformationDictionary participantInformationDictionary, MailboxTypeCache mailboxTypeCache, bool isOwa, IExchangePrincipal mailboxIdentity, string orgUnitName, AdParticipantLookup adParticipantLookup)
		{
			this.resolvedParticipantsMap = participantInformationDictionary;
			this.mailboxTypeCache = mailboxTypeCache;
			this.isOwa = isOwa;
			this.mailboxIdentity = mailboxIdentity;
			this.organizationalUnitName = orgUnitName;
			this.adParticipantLookup = adParticipantLookup;
		}

		public static IParticipantResolver Create(CallContext callContext, int maxParticipantsToResolve = 2147483647)
		{
			if (callContext != null && callContext.FeaturesManager != null && callContext.FeaturesManager.IsFeatureSupported("OptimizedParticipantResolver"))
			{
				AdParticipantLookup adParticipantLookup = new AdParticipantLookup(callContext, maxParticipantsToResolve);
				return new ParticipantResolver(EWSSettings.ParticipantInformation, MailboxTypeCache.Instance, callContext.IsOwa, callContext.MailboxIdentityPrincipal, callContext.OrganizationalUnitName, adParticipantLookup);
			}
			if (maxParticipantsToResolve == 2147483647)
			{
				return StaticParticipantResolver.DefaultInstance;
			}
			return new StaticParticipantResolver(maxParticipantsToResolve);
		}

		private IParticipant CallContextMailboxIdentity
		{
			get
			{
				if (this.callContextMailboxIdentity == null && this.mailboxIdentity != null)
				{
					this.callContextMailboxIdentity = new Participant(this.mailboxIdentity);
				}
				return this.callContextMailboxIdentity;
			}
		}

		public EmailAddressWrapper[] ResolveToEmailAddressWrapper(IEnumerable<IParticipant> participants)
		{
			HashSet<EmailAddressWrapper> hashSet = new HashSet<EmailAddressWrapper>(EmailAddressWrapper.AddressEqualityComparer);
			hashSet.UnionWith(from p in participants
			select EmailAddressWrapper.FromParticipantInformation(this.GetParticipantInformation(p)));
			return hashSet.ToArray<EmailAddressWrapper>();
		}

		public SingleRecipientType ResolveToSingleRecipientType(IParticipant participant)
		{
			if (participant == null)
			{
				return null;
			}
			ParticipantInformation participantInformation = this.GetParticipantInformation(participant);
			return PropertyCommand.CreateRecipientFromParticipant(participantInformation);
		}

		public SmtpAddress ResolveToSmtpAddress(IParticipant participant)
		{
			if (participant == null)
			{
				return new SmtpAddress(string.Empty);
			}
			ParticipantInformation participantInformation = this.GetParticipantInformation(participant);
			if (participantInformation.RoutingType != "SMTP")
			{
				this.LoadAdDataIfNeeded(new IParticipant[]
				{
					participant
				});
				participantInformation = this.GetParticipantInformation(participant);
			}
			return new SmtpAddress(participantInformation.EmailAddress);
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
			IParticipant[] array = this.CalculateParticipantsToConvert(pregatherParticipants).ToArray<IParticipant>();
			IParticipant[] array2 = this.adParticipantLookup.LookUpAdParticipants(array);
			int num = 0;
			foreach (IParticipant participant in array2)
			{
				ExTraceGlobals.ParticipantLookupBatchingTracer.TraceDebug<string, int>(0L, "ParticipantResolver.LoadAdDataIfNeeded - converted participant with EmailAddress = {0} and HashCode = {1}.", array[num].EmailAddress, array[num].GetHashCode());
				IParticipant participant2 = participant;
				bool value = false;
				if (participant == null || participant.EmailAddress == null)
				{
					participant2 = array[num];
					value = true;
				}
				ParticipantInformation info = ParticipantInformation.Create(participant2, MailboxHelper.GetMailboxType(participant2, this.isOwa), new bool?(value));
				this.UpdateCaches(array[num], info);
				num++;
			}
		}

		private ParticipantInformation GetParticipantInformation(IParticipant participant)
		{
			ParticipantInformation result;
			if (this.TryGetParticipantInformation(participant, out result))
			{
				return result;
			}
			return ParticipantInformation.Create(participant, MailboxHelper.GetMailboxType(participant, this.isOwa), null);
		}

		private bool TryGetMailboxType(IParticipant participant, out MailboxHelper.MailboxTypeType mailboxType)
		{
			string key;
			MailboxHelper.MailboxTypeType? mailboxTypeType;
			if (MailboxTypeCache.TryGetKey(participant.SmtpEmailAddress, this.organizationalUnitName, out key) && this.mailboxTypeCache.TryGetValue(key, out mailboxTypeType))
			{
				mailboxType = mailboxTypeType.Value;
				return true;
			}
			mailboxType = MailboxHelper.GetMailboxType(participant, this.isOwa);
			return MailboxHelper.IsFullyResolvedMailboxType(mailboxType);
		}

		private bool TryConstructParticipantInformation(IParticipant participant, out ParticipantInformation participantInformation)
		{
			participantInformation = null;
			bool flag = string.Equals(participant.RoutingType, "EX", StringComparison.OrdinalIgnoreCase);
			MailboxHelper.MailboxTypeType mailboxType;
			if (Global.FastParticipantResolveEnabled && flag && this.TryGetMailboxType(participant, out mailboxType))
			{
				ExTraceGlobals.ParticipantLookupBatchingTracer.TraceDebug<string, int>(0L, "[ParticipantInformation::TryFastResolution] Found cached SMTP address and MailboxType: EmailAddress = {0}, HashCode = {1}", participant.SmtpEmailAddress, participant.GetHashCode());
				participantInformation = ParticipantInformation.CreateSmtpParticipant(participant, participant.DisplayName, participant.SmtpEmailAddress, mailboxType);
				return true;
			}
			if (ParticipantComparer.EmailAddress.Equals(participant, this.CallContextMailboxIdentity))
			{
				participantInformation = ParticipantInformation.CreateSmtpParticipant(participant, this.mailboxIdentity);
				return true;
			}
			if (!flag)
			{
				participantInformation = ParticipantInformation.Create(participant, MailboxHelper.GetMailboxType(participant, this.isOwa), null);
				return true;
			}
			return false;
		}

		private HashSet<IParticipant> CalculateParticipantsToConvert(IEnumerable<IParticipant> participants)
		{
			HashSet<IParticipant> hashSet = new HashSet<IParticipant>(ParticipantComparer.EmailAddress);
			if (!participants.Any<IParticipant>())
			{
				return hashSet;
			}
			foreach (IParticipant participant in participants)
			{
				ParticipantInformation participantInformation;
				if (participant == null)
				{
					ExTraceGlobals.ParticipantLookupBatchingTracer.TraceDebug(0L, "ParticipantResolver.CalculateParticipantsToConvert - found null entry");
				}
				else if (!this.TryGetParticipantInformation(participant, out participantInformation))
				{
					hashSet.Add(participant);
				}
			}
			return hashSet;
		}

		private bool TryGetParticipantInformation(IParticipant participant, out ParticipantInformation info)
		{
			if (this.resolvedParticipantsMap.TryGetParticipant(participant, out info))
			{
				ExTraceGlobals.ParticipantLookupBatchingTracer.TraceDebug<string, int>(0L, "ToServiceObjectPropertyList.ConvertAndGetParticipantInformation - using already resolved participant for EmailAddress = {0}, HashCode = {1}", participant.EmailAddress, participant.GetHashCode());
				return true;
			}
			if (this.TryConstructParticipantInformation(participant, out info))
			{
				this.UpdateCaches(participant, info);
				return true;
			}
			return false;
		}

		private void UpdateCaches(IParticipant participant, ParticipantInformation info)
		{
			this.UpdateMailboxTypeCache(info.EmailAddress, info.MailboxType);
			this.resolvedParticipantsMap.AddParticipant(participant, info);
			if (participant.RoutingType == "EX" && info.RoutingType == "SMTP")
			{
				Participant participant2 = new Participant(info.DisplayName, info.EmailAddress, "SMTP");
				if (!this.resolvedParticipantsMap.ContainsParticipant(participant2))
				{
					this.resolvedParticipantsMap.AddParticipant(participant2, info);
				}
			}
		}

		private void UpdateMailboxTypeCache(string smtpAddress, MailboxHelper.MailboxTypeType mailboxType)
		{
			string key = null;
			if (Global.FastParticipantResolveEnabled && MailboxTypeCache.TryGetKey(smtpAddress, this.organizationalUnitName, out key))
			{
				this.mailboxTypeCache.TryAdd(key, mailboxType);
			}
		}

		private readonly ParticipantInformationDictionary resolvedParticipantsMap;

		private readonly MailboxTypeCache mailboxTypeCache;

		private readonly bool isOwa;

		private readonly IExchangePrincipal mailboxIdentity;

		private string organizationalUnitName;

		private readonly AdParticipantLookup adParticipantLookup;

		private Participant callContextMailboxIdentity;
	}
}
