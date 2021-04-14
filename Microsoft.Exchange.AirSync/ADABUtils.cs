using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.AirSync
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal static class ADABUtils
	{
		internal static bool CanEmailRecipientType(RecipientType recipientType)
		{
			switch (recipientType)
			{
			case RecipientType.UserMailbox:
			case RecipientType.MailUser:
			case RecipientType.MailContact:
			case RecipientType.MailUniversalDistributionGroup:
			case RecipientType.MailUniversalSecurityGroup:
			case RecipientType.MailNonUniversalGroup:
			case RecipientType.DynamicDistributionGroup:
			case RecipientType.PublicFolder:
				return true;
			}
			return false;
		}

		internal static bool GetOwnerId(ADRawEntry rawEntry, out ADABObjectId ownerId)
		{
			object obj;
			if (!rawEntry.TryGetValueWithoutDefault(ADGroupSchema.ManagedBy, out obj))
			{
				ownerId = null;
				return false;
			}
			if (obj == null)
			{
				ownerId = null;
				return true;
			}
			MultiValuedProperty<ADObjectId> multiValuedProperty = (MultiValuedProperty<ADObjectId>)obj;
			using (MultiValuedProperty<ADObjectId>.Enumerator enumerator = multiValuedProperty.GetEnumerator())
			{
				if (enumerator.MoveNext())
				{
					ADObjectId activeDirectoryObjectId = enumerator.Current;
					ownerId = new ADABObjectId(activeDirectoryObjectId);
					return true;
				}
			}
			ownerId = null;
			return true;
		}

		internal static bool GetWebPage(ADRawEntry rawEntry, out Uri webPageUri)
		{
			object obj;
			if (!rawEntry.TryGetValueWithoutDefault(ADRecipientSchema.WebPage, out obj))
			{
				webPageUri = null;
				return false;
			}
			if (obj == null)
			{
				webPageUri = null;
				return true;
			}
			string text = (string)obj;
			if (string.IsNullOrEmpty(text))
			{
				webPageUri = null;
				return true;
			}
			return Uri.TryCreate(text, UriKind.Absolute, out webPageUri) || true;
		}

		internal static bool GetId(ADRawEntry rawEntry, out ADABObjectId addressBookObjectId)
		{
			object obj;
			if (!rawEntry.TryGetValueWithoutDefault(ADObjectSchema.Id, out obj))
			{
				addressBookObjectId = null;
				return false;
			}
			if (obj == null)
			{
				addressBookObjectId = null;
				return true;
			}
			ADObjectId activeDirectoryObjectId = (ADObjectId)obj;
			addressBookObjectId = new ADABObjectId(activeDirectoryObjectId);
			return true;
		}

		internal static bool GetEmailAddress(ADRawEntry rawEntry, out string emailAddress)
		{
			emailAddress = ((SmtpAddress)ADRecipient.PrimarySmtpAddressGetter(rawEntry)).ToString();
			return !string.IsNullOrEmpty(emailAddress);
		}
	}
}
