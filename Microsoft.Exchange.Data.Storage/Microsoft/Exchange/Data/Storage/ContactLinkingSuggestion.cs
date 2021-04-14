using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;
using Microsoft.Exchange.Net;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class ContactLinkingSuggestion
	{
		internal PersonId PersonId { get; private set; }

		internal bool SurnameMatch { get; private set; }

		internal bool GivenNameMatch { get; private set; }

		internal bool AliasOfEmailAddressMatch { get; private set; }

		internal bool PhoneNumberMatch { get; private set; }

		internal int PartialSurnameMatch { get; private set; }

		internal int PartialGivenNameMatch { get; private set; }

		internal int PartialAliasOfEmailAddressMatch { get; private set; }

		public override string ToString()
		{
			return string.Format("Suggestion for {0}: SurnameMatch={1}, GivenNameMatch={2}, AliasOfEmailAddressMatch={3}, PhoneNumberMatch={4}, PartialSurnameMatch={5}, PartialGivenNameMatch={6}, PartialAliasOfEmailAddressMatch={7}", new object[]
			{
				this.PersonId,
				this.SurnameMatch,
				this.GivenNameMatch,
				this.AliasOfEmailAddressMatch,
				this.PhoneNumberMatch,
				this.PartialSurnameMatch,
				this.PartialGivenNameMatch,
				this.PartialAliasOfEmailAddressMatch
			});
		}

		internal static int Compare(ContactLinkingSuggestion a, ContactLinkingSuggestion b)
		{
			Util.ThrowOnNullArgument(a, "a");
			Util.ThrowOnNullArgument(b, "b");
			int num = ContactLinkingSuggestion.Compare(a.SurnameMatch && a.GivenNameMatch, b.SurnameMatch && b.GivenNameMatch);
			if (num != 0)
			{
				return num;
			}
			num = ContactLinkingSuggestion.Compare(a.AliasOfEmailAddressMatch, b.AliasOfEmailAddressMatch);
			if (num != 0)
			{
				return num;
			}
			num = ContactLinkingSuggestion.Compare(a.PhoneNumberMatch, b.PhoneNumberMatch);
			if (num != 0)
			{
				return num;
			}
			num = ContactLinkingSuggestion.Compare(a.PartialSurnameMatch, b.PartialSurnameMatch);
			if (num != 0)
			{
				return num;
			}
			num = ContactLinkingSuggestion.Compare(a.PartialGivenNameMatch, b.PartialGivenNameMatch);
			if (num != 0)
			{
				return num;
			}
			num = ContactLinkingSuggestion.Compare(a.PartialAliasOfEmailAddressMatch, b.PartialAliasOfEmailAddressMatch);
			if (num != 0)
			{
				return num;
			}
			return 0;
		}

		internal static IList<ContactLinkingSuggestion> GetSuggestions(CultureInfo culture, IList<ContactInfoForSuggestion> personContacts, IEnumerable<ContactInfoForSuggestion> otherContacts)
		{
			Dictionary<PersonId, ContactLinkingSuggestion> dictionary = new Dictionary<PersonId, ContactLinkingSuggestion>();
			foreach (ContactInfoForSuggestion contactInfoForSuggestion in otherContacts)
			{
				if (!WellKnownNetworkNames.IsHiddenSourceNetworkName(contactInfoForSuggestion.PartnerNetworkId, contactInfoForSuggestion.ParentDisplayName))
				{
					foreach (ContactInfoForSuggestion personContact in personContacts)
					{
						ContactLinkingSuggestion contactLinkingSuggestion = ContactLinkingSuggestion.Create(culture, personContact, contactInfoForSuggestion);
						if (contactLinkingSuggestion != null)
						{
							ContactLinkingSuggestion b;
							if (dictionary.TryGetValue(contactLinkingSuggestion.PersonId, out b))
							{
								if (ContactLinkingSuggestion.Compare(contactLinkingSuggestion, b) > 0)
								{
									dictionary[contactLinkingSuggestion.PersonId] = contactLinkingSuggestion;
								}
							}
							else
							{
								dictionary.Add(contactLinkingSuggestion.PersonId, contactLinkingSuggestion);
							}
						}
					}
				}
			}
			List<ContactLinkingSuggestion> list = new List<ContactLinkingSuggestion>(dictionary.Values);
			list.Sort(new Comparison<ContactLinkingSuggestion>(ContactLinkingSuggestion.Compare));
			if (list.Count > ContactLinkingSuggestion.MaximumSuggestions.Value)
			{
				list.RemoveRange(ContactLinkingSuggestion.MaximumSuggestions.Value, list.Count - ContactLinkingSuggestion.MaximumSuggestions.Value);
			}
			return list;
		}

		internal static ContactLinkingSuggestion Create(CultureInfo culture, ContactInfoForSuggestion personContact, ContactInfoForSuggestion otherContact)
		{
			Util.ThrowOnNullArgument(culture, "culture");
			Util.ThrowOnNullArgument(personContact, "personContact");
			Util.ThrowOnNullArgument(otherContact, "otherContact");
			if (otherContact.PersonId == null)
			{
				ContactLinkingSuggestion.Tracer.TraceError<StoreId>(0L, "Cannot use contact without PersonId: {0}", otherContact.ItemId);
				return null;
			}
			if (otherContact.PersonId.Equals(personContact.PersonId))
			{
				ContactLinkingSuggestion.Tracer.TraceDebug<StoreId>(0L, "Ignoring contact {0} because it has same PersonId of the person we are looking suggestions for", otherContact.ItemId);
				return null;
			}
			if (otherContact.LinkRejectHistory != null && Array.Exists<PersonId>(otherContact.LinkRejectHistory, (PersonId otherContactLinkReject) => personContact.PersonId.Equals(otherContactLinkReject)))
			{
				ContactLinkingSuggestion.Tracer.TraceDebug<StoreId, PersonId>(0L, "Ignoring contact {0} because its LinkRejectHistory has PersonId of the person we are looking suggestions for: {1}", otherContact.ItemId, personContact.PersonId);
				return null;
			}
			if (personContact.LinkRejectHistory != null && Array.Exists<PersonId>(personContact.LinkRejectHistory, (PersonId personPersonId) => personPersonId.Equals(otherContact.PersonId)))
			{
				ContactLinkingSuggestion.Tracer.TraceDebug<StoreId, PersonId>(0L, "Ignoring contact {0} because its PersonId is present in LinkRejectHistory of the person we are looking suggestions for: {1}", otherContact.ItemId, personContact.PersonId);
				return null;
			}
			ContactLinkingSuggestion.NameCompareResult nameCompareResult = ContactLinkingSuggestion.CompareNames(culture, personContact.GivenName, personContact.Surname, otherContact.GivenName, otherContact.Surname);
			ContactLinkingSuggestion.NameCompareResult nameCompareResult2 = ContactLinkingSuggestion.CompareNames(culture, personContact.GivenName, personContact.Surname, otherContact.Surname, otherContact.GivenName);
			if (nameCompareResult.IsFullMatch || nameCompareResult2.IsFullMatch)
			{
				return new ContactLinkingSuggestion
				{
					PersonId = otherContact.PersonId,
					SurnameMatch = true,
					GivenNameMatch = true
				};
			}
			if (ContactLinkingSuggestion.IsAliasOfEmailAddressMatch(personContact, otherContact))
			{
				return new ContactLinkingSuggestion
				{
					PersonId = otherContact.PersonId,
					AliasOfEmailAddressMatch = true
				};
			}
			if (ContactLinkingSuggestion.IsPhoneNumberMatch(personContact, otherContact))
			{
				return new ContactLinkingSuggestion
				{
					PersonId = otherContact.PersonId,
					PhoneNumberMatch = true
				};
			}
			if (nameCompareResult.IsPartialMatch)
			{
				return new ContactLinkingSuggestion
				{
					PersonId = otherContact.PersonId,
					GivenNameMatch = nameCompareResult.FullGivenName,
					SurnameMatch = nameCompareResult.FullSurname,
					PartialSurnameMatch = nameCompareResult.PartialSurname,
					PartialGivenNameMatch = nameCompareResult.PartialGivenName
				};
			}
			if (nameCompareResult2.IsPartialMatch)
			{
				return new ContactLinkingSuggestion
				{
					PersonId = otherContact.PersonId,
					GivenNameMatch = nameCompareResult2.FullGivenName,
					SurnameMatch = nameCompareResult2.FullSurname,
					PartialSurnameMatch = nameCompareResult2.PartialSurname,
					PartialGivenNameMatch = nameCompareResult2.PartialGivenName
				};
			}
			int partialAliasOfEmailAddressMatch = ContactLinkingSuggestion.GetPartialAliasOfEmailAddressMatch(personContact, otherContact);
			if (partialAliasOfEmailAddressMatch > 0)
			{
				return new ContactLinkingSuggestion
				{
					PersonId = otherContact.PersonId,
					PartialAliasOfEmailAddressMatch = partialAliasOfEmailAddressMatch
				};
			}
			ContactLinkingSuggestion.Tracer.TraceDebug<StoreId>(0L, "Ignoring contact {0} because its doesn't match any criteria for suggestion.", otherContact.ItemId);
			return null;
		}

		private static int Compare(bool a, bool b)
		{
			return (a ? 1 : 0) - (b ? 1 : 0);
		}

		private static int Compare(int a, int b)
		{
			return Math.Sign(a - b);
		}

		private static int GetPartialAliasOfEmailAddressMatch(ContactInfoForSuggestion a, ContactInfoForSuggestion b)
		{
			int num = 0;
			if (a.AliasOfEmailAddresses != null && b.AliasOfEmailAddresses != null)
			{
				foreach (string a2 in a.AliasOfEmailAddresses)
				{
					foreach (string b2 in b.AliasOfEmailAddresses)
					{
						num = Math.Max(num, ContactLinkingSuggestionMatching.AliasOrEmailAddress.GetPartialMatchCount(CultureInfo.InvariantCulture, a2, b2));
					}
				}
			}
			return num;
		}

		private static bool IsAliasOfEmailAddressMatch(ContactInfoForSuggestion a, ContactInfoForSuggestion b)
		{
			return a.AliasOfEmailAddresses != null && b.AliasOfEmailAddresses != null && a.AliasOfEmailAddresses.Overlaps(b.AliasOfEmailAddresses);
		}

		private static bool IsPhoneNumberMatch(ContactInfoForSuggestion a, ContactInfoForSuggestion b)
		{
			return a.PhoneNumbers != null && b.PhoneNumbers != null && a.PhoneNumbers.Overlaps(b.PhoneNumbers);
		}

		private static ContactLinkingSuggestion.NameCompareResult CompareNames(CultureInfo culture, string aGivenName, string aSurname, string bGivenName, string bSurname)
		{
			bool flag = ContactLinkingSuggestionMatching.FirstOrLastName.IsFullMatch(culture, aGivenName, bGivenName);
			bool flag2 = ContactLinkingSuggestionMatching.FirstOrLastName.IsFullMatch(culture, aSurname, bSurname);
			if (flag && flag2)
			{
				return new ContactLinkingSuggestion.NameCompareResult
				{
					FullGivenName = true,
					FullSurname = true
				};
			}
			int partialGivenName = 0;
			if (!flag)
			{
				partialGivenName = ContactLinkingSuggestionMatching.FirstOrLastName.GetPartialMatchCount(culture, aGivenName, bGivenName);
			}
			int partialSurname = 0;
			if (!flag2)
			{
				partialSurname = ContactLinkingSuggestionMatching.FirstOrLastName.GetPartialMatchCount(culture, aSurname, bSurname);
			}
			return new ContactLinkingSuggestion.NameCompareResult
			{
				FullGivenName = flag,
				FullSurname = flag2,
				PartialGivenName = partialGivenName,
				PartialSurname = partialSurname
			};
		}

		private static readonly Trace Tracer = ExTraceGlobals.ContactLinkingTracer;

		internal static readonly IntAppSettingsEntry MaximumSuggestions = new IntAppSettingsEntry("MaximumContactSuggestions", 3, ContactLinkingSuggestion.Tracer);

		private sealed class NameCompareResult
		{
			public bool FullGivenName { get; set; }

			public bool FullSurname { get; set; }

			public int PartialGivenName { get; set; }

			public int PartialSurname { get; set; }

			public bool IsFullMatch
			{
				get
				{
					return this.FullGivenName && this.FullSurname;
				}
			}

			public bool IsPartialMatch
			{
				get
				{
					return (this.FullGivenName && this.PartialSurname > 0) || (this.FullSurname && this.PartialGivenName > 0);
				}
			}
		}
	}
}
