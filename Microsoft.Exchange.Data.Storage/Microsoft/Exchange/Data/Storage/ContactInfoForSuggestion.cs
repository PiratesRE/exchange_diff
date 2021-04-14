using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class ContactInfoForSuggestion
	{
		internal ContactInfoForSuggestion()
		{
		}

		internal PersonId PersonId { get; set; }

		internal StoreId ItemId { get; set; }

		internal PersonId[] LinkRejectHistory { get; set; }

		internal string GivenName { get; set; }

		internal string Surname { get; set; }

		internal HashSet<string> EmailAddresses { get; set; }

		internal HashSet<string> AliasOfEmailAddresses { get; set; }

		internal HashSet<string> PhoneNumbers { get; set; }

		internal string PartnerNetworkId { get; set; }

		internal string ParentDisplayName { get; set; }

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder(100);
			stringBuilder.Append("PersonId=");
			stringBuilder.Append(this.PersonId.ToString());
			stringBuilder.Append(", ItemId=");
			stringBuilder.Append(this.ItemId.ToString());
			if (!string.IsNullOrEmpty(this.GivenName))
			{
				stringBuilder.Append(", GivenName=");
				stringBuilder.Append(this.GivenName);
			}
			if (!string.IsNullOrEmpty(this.Surname))
			{
				stringBuilder.Append(", Surname=");
				stringBuilder.Append(this.Surname);
			}
			if (this.EmailAddresses.Count > 0)
			{
				stringBuilder.Append(", EmailAddreses={");
				foreach (string value in this.EmailAddresses)
				{
					stringBuilder.Append(value);
					stringBuilder.Append(",");
				}
				stringBuilder.Append("}");
			}
			if (this.PhoneNumbers.Count > 0)
			{
				stringBuilder.Append(", PhoneNumbers={");
				foreach (string value2 in this.PhoneNumbers)
				{
					stringBuilder.Append(value2);
					stringBuilder.Append(",");
				}
				stringBuilder.Append("}");
			}
			if (!string.IsNullOrEmpty(this.PartnerNetworkId))
			{
				stringBuilder.Append(", PartnerNetworkId=");
				stringBuilder.Append(this.PartnerNetworkId);
			}
			if (!string.IsNullOrEmpty(this.ParentDisplayName))
			{
				stringBuilder.Append(", ParentDisplayName=");
				stringBuilder.Append(this.ParentDisplayName);
			}
			return stringBuilder.ToString();
		}

		internal static IList<ContactInfoForSuggestion> ConvertAll(IList<IStorePropertyBag> inputContacts)
		{
			Util.ThrowOnNullArgument(inputContacts, "inputContacts");
			List<ContactInfoForSuggestion> list = new List<ContactInfoForSuggestion>(inputContacts.Count);
			foreach (IStorePropertyBag propertyBag in inputContacts)
			{
				list.Add(ContactInfoForSuggestion.Create(propertyBag));
			}
			return list;
		}

		internal static IEnumerable<ContactInfoForSuggestion> GetContactsEnumerator(MailboxSession mailboxSession)
		{
			return ContactsEnumerator<ContactInfoForSuggestion>.CreateContactsOnlyEnumerator(mailboxSession, DefaultFolderType.AllContacts, ContactInfoForSuggestion.Properties, new Func<IStorePropertyBag, ContactInfoForSuggestion>(ContactInfoForSuggestion.Create), new XSOFactory());
		}

		internal static ContactInfoForSuggestion Create(IStorePropertyBag propertyBag)
		{
			Util.ThrowOnNullArgument(propertyBag, "propertyBag");
			StoreId valueOrDefault = propertyBag.GetValueOrDefault<StoreId>(ItemSchema.Id, null);
			PersonId valueOrDefault2 = propertyBag.GetValueOrDefault<PersonId>(ContactSchema.PersonId, null);
			PersonId[] valueOrDefault3 = propertyBag.GetValueOrDefault<PersonId[]>(ContactSchema.LinkRejectHistory, null);
			string @string = ContactInfoForSuggestion.GetString(new Func<string, string>(ContactInfoForSuggestion.NormalizeName), propertyBag, ContactSchema.GivenName);
			string string2 = ContactInfoForSuggestion.GetString(new Func<string, string>(ContactInfoForSuggestion.NormalizeName), propertyBag, ContactSchema.Surname);
			string valueOrDefault4 = propertyBag.GetValueOrDefault<string>(ContactSchema.PartnerNetworkId, null);
			string valueOrDefault5 = propertyBag.GetValueOrDefault<string>(ItemSchema.ParentDisplayName, null);
			HashSet<string> stringSet = ContactInfoForSuggestion.GetStringSet(new Func<string, string>(ContactInfoForSuggestion.NormalizeEmailAddress), propertyBag, new PropertyDefinition[]
			{
				ContactSchema.Email1EmailAddress,
				ContactSchema.Email2EmailAddress,
				ContactSchema.Email3EmailAddress,
				ContactSchema.PrimarySmtpAddress
			});
			HashSet<string> aliasOfEmailAddresses = ContactInfoForSuggestion.GetAliasOfEmailAddresses(stringSet);
			HashSet<string> stringSet2 = ContactInfoForSuggestion.GetStringSet(new Func<string, string>(Util.NormalizePhoneNumber), propertyBag, new PropertyDefinition[]
			{
				ContactSchema.BusinessPhoneNumber,
				ContactSchema.BusinessPhoneNumber2,
				ContactSchema.HomePhone,
				ContactSchema.MobilePhone,
				ContactSchema.OtherTelephone
			});
			return new ContactInfoForSuggestion
			{
				ItemId = valueOrDefault,
				PersonId = valueOrDefault2,
				LinkRejectHistory = valueOrDefault3,
				GivenName = @string,
				Surname = string2,
				EmailAddresses = stringSet,
				AliasOfEmailAddresses = aliasOfEmailAddresses,
				PhoneNumbers = stringSet2,
				PartnerNetworkId = valueOrDefault4,
				ParentDisplayName = valueOrDefault5
			};
		}

		private static HashSet<string> GetStringSet(Func<string, string> normalize, IStorePropertyBag propertyBag, params PropertyDefinition[] properties)
		{
			HashSet<string> hashSet = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
			foreach (PropertyDefinition property in properties)
			{
				string @string = ContactInfoForSuggestion.GetString(normalize, propertyBag, property);
				if (@string != null)
				{
					hashSet.Add(@string);
				}
			}
			return hashSet;
		}

		private static string GetString(Func<string, string> normalize, IStorePropertyBag propertyBag, PropertyDefinition property)
		{
			string text = normalize(propertyBag.GetValueOrDefault<string>(property, string.Empty));
			if (!string.IsNullOrEmpty(text))
			{
				return text;
			}
			return null;
		}

		private static HashSet<string> GetAliasOfEmailAddresses(HashSet<string> emailAddresses)
		{
			HashSet<string> hashSet = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
			foreach (string text in emailAddresses)
			{
				int num = text.IndexOf("@");
				if (num > 1)
				{
					hashSet.Add(text.Substring(0, num));
				}
			}
			return hashSet;
		}

		private static string NormalizeEmailAddress(string emailAddress)
		{
			return emailAddress.Trim();
		}

		private static string NormalizeName(string name)
		{
			string text = name.Trim();
			do
			{
				name = text;
				text = name.Replace("  ", " ");
			}
			while (!StringComparer.OrdinalIgnoreCase.Equals(text, name));
			return text;
		}

		internal static readonly PropertyDefinition[] Properties = new PropertyDefinition[]
		{
			ItemSchema.Id,
			ContactSchema.PersonId,
			ContactSchema.LinkRejectHistory,
			ContactSchema.GivenName,
			ContactSchema.Surname,
			StoreObjectSchema.DisplayName,
			ContactSchema.Email1EmailAddress,
			ContactSchema.Email2EmailAddress,
			ContactSchema.Email3EmailAddress,
			ContactSchema.PrimarySmtpAddress,
			ContactSchema.BusinessPhoneNumber,
			ContactSchema.BusinessPhoneNumber2,
			ContactSchema.HomePhone,
			ContactSchema.MobilePhone,
			ContactSchema.OtherTelephone,
			ContactSchema.PartnerNetworkId,
			ItemSchema.ParentDisplayName
		};
	}
}
