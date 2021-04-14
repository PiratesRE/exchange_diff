using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	internal sealed class AdRecipientBatchQuery
	{
		public AdRecipientBatchQuery(IEnumerator<Participant> recipients, UserContext userContext)
		{
			this.legacyExchangeDNToRecipientDictionary = AdRecipientBatchQuery.FindByParticipantLegacyExchangeDNs(recipients, userContext);
		}

		public AdRecipientBatchQuery(UserContext userContext, params string[] legacyDNs)
		{
			this.legacyExchangeDNToRecipientDictionary = AdRecipientBatchQuery.FindByLegacyExchangeDNs(legacyDNs, userContext);
		}

		public static Result<ADRawEntry>[] FindAdResultsByLegacyExchangeDNs(IEnumerator<Participant> recipients, UserContext userContext)
		{
			List<string> list = new List<string>();
			while (recipients.MoveNext())
			{
				Participant participant = recipients.Current;
				if (participant.RoutingType == "EX" && !string.IsNullOrEmpty(participant.EmailAddress))
				{
					list.Add(participant.EmailAddress);
				}
			}
			if (list.Count > 0)
			{
				string[] legacyExchangeDNs = list.ToArray();
				IRecipientSession recipientSession = Utilities.CreateADRecipientSession(ConsistencyMode.IgnoreInvalid, userContext);
				return recipientSession.FindByLegacyExchangeDNs(legacyExchangeDNs, AdRecipientBatchQuery.recipientQueryProperties);
			}
			return null;
		}

		public ADRecipient GetAdRecipient(string legacyExchangeDN)
		{
			if (legacyExchangeDN == null)
			{
				return null;
			}
			ADRecipient result = null;
			this.legacyExchangeDNToRecipientDictionary.TryGetValue(legacyExchangeDN, out result);
			return result;
		}

		private static Dictionary<string, ADRecipient> FindByParticipantLegacyExchangeDNs(IEnumerator<Participant> recipients, UserContext userContext)
		{
			List<string> list = new List<string>();
			while (recipients.MoveNext())
			{
				Participant participant = recipients.Current;
				if (participant.RoutingType == "EX" && !string.IsNullOrEmpty(participant.EmailAddress))
				{
					list.Add(participant.EmailAddress.ToLowerInvariant());
				}
			}
			return AdRecipientBatchQuery.FindByLegacyExchangeDNs(list.ToArray(), userContext);
		}

		private static Dictionary<string, ADRecipient> FindByLegacyExchangeDNs(string[] legacyExchangeDNs, UserContext userContext)
		{
			Dictionary<string, ADRecipient> dictionary = new Dictionary<string, ADRecipient>(StringComparer.OrdinalIgnoreCase);
			if (legacyExchangeDNs.Length > 0)
			{
				IRecipientSession recipientSession = Utilities.CreateADRecipientSession(ConsistencyMode.IgnoreInvalid, userContext);
				Result<ADRecipient>[] array = recipientSession.FindADRecipientsByLegacyExchangeDNs(legacyExchangeDNs);
				for (int i = 0; i < array.Length; i++)
				{
					if (array[i].Data != null)
					{
						dictionary[legacyExchangeDNs[i]] = array[i].Data;
					}
				}
			}
			return dictionary;
		}

		private static PropertyDefinition[] recipientQueryProperties = new PropertyDefinition[]
		{
			ADRecipientSchema.PrimarySmtpAddress,
			ADObjectSchema.Id,
			ADRecipientSchema.Alias,
			ADRecipientSchema.RecipientDisplayType,
			ADObjectSchema.ObjectClass,
			ADOrgPersonSchema.MobilePhone
		};

		private Dictionary<string, ADRecipient> legacyExchangeDNToRecipientDictionary;
	}
}
