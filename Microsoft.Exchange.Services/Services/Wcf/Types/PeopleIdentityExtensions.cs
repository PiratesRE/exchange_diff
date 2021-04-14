using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Management;

namespace Microsoft.Exchange.Services.Wcf.Types
{
	public static class PeopleIdentityExtensions
	{
		public static INamedIdentity[] ToIdParameters(this IEnumerable<PeopleIdentity> identities)
		{
			if (identities == null || !identities.Any<PeopleIdentity>())
			{
				return null;
			}
			return (INamedIdentity[])identities.ToArray<PeopleIdentity>();
		}

		public static PeopleIdentity ToPeopleIdentity(this ADRecipient entry)
		{
			if (entry != null && entry.Id != null)
			{
				return new PeopleIdentity(entry.DisplayName, entry.LegacyExchangeDN, entry.PrimarySmtpAddress.ToString(), 2, "EX");
			}
			return null;
		}

		public static PeopleIdentity ToPeopleIdentity(this ADRecipientOrAddress entry)
		{
			if (entry != null)
			{
				return new PeopleIdentity(entry.DisplayName, entry.Address, entry.Address, (entry.RoutingType == "EX") ? 2 : 3, entry.RoutingType);
			}
			return null;
		}

		public static RecipientIdParameter[] ToRecipientIdParameters(this IEnumerable<PeopleIdentity> identities)
		{
			if (identities == null || !identities.Any<PeopleIdentity>())
			{
				return null;
			}
			return identities.Select(delegate(PeopleIdentity e)
			{
				if (!string.IsNullOrEmpty(e.SmtpAddress))
				{
					return new RecipientIdParameter(e.SmtpAddress);
				}
				return new RecipientIdParameter(e);
			}).ToArray<RecipientIdParameter>();
		}

		public static IEnumerable<string> ToSmtpAddressArray(this IEnumerable<PeopleIdentity> identities)
		{
			if (identities != null && identities.Any<PeopleIdentity>())
			{
				return (from identity in identities
				select identity.SmtpAddress).ToArray<string>().Distinct<string>();
			}
			return null;
		}

		public static PeopleIdentity ToPeopleIdentity(this SmtpAddress? address, string displayName)
		{
			if (address == null)
			{
				return null;
			}
			Participant participant = new Participant(displayName, address.Value.ToString(), "SMTP");
			return new ADRecipientOrAddress(participant).ToPeopleIdentity();
		}

		public static PeopleIdentity[] ToPeopleIdentityArray(this IEnumerable<ADObjectId> addresses)
		{
			if (addresses == null || !addresses.Any<ADObjectId>())
			{
				return null;
			}
			return (from r in RecipientObjectResolver.Instance.ResolveObjects(addresses)
			select r.ToPeopleIdentity()).ToArray<PeopleIdentity>();
		}

		public static PeopleIdentity[] ToPeopleIdentityArray(this IEnumerable<ADRecipientOrAddress> addresses)
		{
			List<PeopleIdentity> list = new List<PeopleIdentity>();
			if (addresses == null || !addresses.Any<ADRecipientOrAddress>())
			{
				return null;
			}
			Dictionary<string, ADRecipient> dictionary = new Dictionary<string, ADRecipient>(StringComparer.OrdinalIgnoreCase);
			IEnumerable<string> legacyDNs = from address in addresses
			where address.RoutingType == "EX"
			select address.Address;
			IEnumerable<ADRecipient> enumerable = RecipientObjectResolver.Instance.ResolveLegacyDNs(legacyDNs);
			if (enumerable != null && enumerable.Any<ADRecipient>())
			{
				foreach (ADRecipient adrecipient in enumerable)
				{
					if (!dictionary.ContainsKey(adrecipient.LegacyExchangeDN))
					{
						dictionary.Add(adrecipient.LegacyExchangeDN, adrecipient);
					}
				}
			}
			IEnumerable<string> addresses2 = from address in addresses
			where address.RoutingType == "SMTP"
			select address.Address;
			enumerable = RecipientObjectResolver.Instance.ResolveSmtpAddress(addresses2);
			if (enumerable != null && enumerable.Any<ADRecipient>())
			{
				foreach (ADRecipient adrecipient2 in enumerable)
				{
					string key = adrecipient2.PrimarySmtpAddress.ToString();
					if (!dictionary.ContainsKey(key))
					{
						dictionary.Add(key, adrecipient2);
					}
				}
			}
			foreach (ADRecipientOrAddress adrecipientOrAddress in addresses)
			{
				ADRecipient entry = null;
				if (dictionary.TryGetValue(adrecipientOrAddress.Address, out entry))
				{
					list.Add(entry.ToPeopleIdentity());
				}
				else
				{
					list.Add(adrecipientOrAddress.ToPeopleIdentity());
				}
			}
			return list.ToArray();
		}
	}
}
