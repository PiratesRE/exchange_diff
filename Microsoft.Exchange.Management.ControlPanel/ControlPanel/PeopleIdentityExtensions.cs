using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.PowerShell.RbacHostingTools;

namespace Microsoft.Exchange.Management.ControlPanel
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
				return new PeopleIdentity(entry.DisplayName, entry.LegacyExchangeDN, entry.PrimarySmtpAddress.ToString(), 2, RbacPrincipal.Current.IsInRole("FFO") ? "SMTP" : "EX", (int)entry.RecipientType.ToRecipientAddressFlag());
			}
			return null;
		}

		public static PeopleIdentity ToPeopleIdentity(this PeopleRecipientObject entry)
		{
			if (entry != null)
			{
				return new PeopleIdentity(entry.DisplayName, entry.LegacyExchangeDN, entry.PrimarySmtpAddress, 2, "EX", (int)entry.RecipientType.ToRecipientAddressFlag());
			}
			return null;
		}

		public static PeopleIdentity ToPeopleIdentity(this ADRecipientOrAddress entry)
		{
			if (entry != null)
			{
				return new PeopleIdentity(entry.DisplayName, entry.Address, entry.Address, (entry.RoutingType == "EX") ? 2 : 3, entry.RoutingType, 0);
			}
			return null;
		}

		public static IEnumerable<string> ToSMTPAddressArray(this IEnumerable<PeopleIdentity> identities)
		{
			if (identities != null && identities.Any<PeopleIdentity>())
			{
				return (from identity in identities
				select identity.SMTPAddress).ToArray<string>().Distinct<string>();
			}
			return null;
		}

		public static PeopleIdentity[] ToPeopleIdentityArray(this IEnumerable<ADRecipient> identities)
		{
			if (identities != null && identities.Any<ADRecipient>())
			{
				return (from identity in identities
				select identity.ToPeopleIdentity()).ToArray<PeopleIdentity>();
			}
			return null;
		}

		public static RecipientAddressFlags ToRecipientAddressFlag(this RecipientType type)
		{
			RecipientAddressFlags result = RecipientAddressFlags.None;
			if (type == RecipientType.MailUniversalDistributionGroup || type == RecipientType.MailUniversalSecurityGroup || type == RecipientType.MailNonUniversalGroup || type == RecipientType.DynamicDistributionGroup)
			{
				result = RecipientAddressFlags.DistributionList;
			}
			return result;
		}

		public static PeopleIdentity[] ToPeopleIdentityArray(this IEnumerable<ADIdParameter> identities)
		{
			IEnumerable<ADRecipientOrAddress> enumerable = identities.ToADRecipientOrAddresses();
			if (enumerable != null && enumerable.Any<ADRecipientOrAddress>())
			{
				return enumerable.ToArray<ADRecipientOrAddress>().ToPeopleIdentityArray();
			}
			return null;
		}

		public static PeopleIdentity ToPeopleIdentity(this SmtpAddress? address)
		{
			ADRecipientOrAddress adrecipientOrAddress = address.ToADRecipientOrAddress();
			if (adrecipientOrAddress != null)
			{
				return adrecipientOrAddress.ToPeopleIdentity();
			}
			return null;
		}

		public static PeopleIdentity[] ToPeopleIdentityArray(this IEnumerable<PeopleRecipientObject> identities)
		{
			if (identities != null && identities.Any<PeopleRecipientObject>())
			{
				return (from identity in identities
				select identity.ToPeopleIdentity()).ToArray<PeopleIdentity>();
			}
			return null;
		}

		public static PeopleIdentity[] ToPeopleIdentityArray(this ADRecipientOrAddress[] addresses)
		{
			List<PeopleIdentity> list = new List<PeopleIdentity>();
			if (addresses != null && addresses.Any<ADRecipientOrAddress>())
			{
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
			}
			if (!list.Any<PeopleIdentity>())
			{
				return null;
			}
			return list.ToArray();
		}
	}
}
