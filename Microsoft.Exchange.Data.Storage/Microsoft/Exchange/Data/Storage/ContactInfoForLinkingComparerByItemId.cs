using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class ContactInfoForLinkingComparerByItemId : IEqualityComparer<ContactInfoForLinking>
	{
		private ContactInfoForLinkingComparerByItemId()
		{
		}

		public bool Equals(ContactInfoForLinking x, ContactInfoForLinking y)
		{
			return object.ReferenceEquals(x, y) || (x != null && y != null && ContactInfoForLinkingComparerByItemId.IsSameItemId(x, y));
		}

		public int GetHashCode(ContactInfoForLinking contact)
		{
			if (contact.ItemId == null)
			{
				return 0;
			}
			return contact.ItemId.GetHashCode();
		}

		private static bool IsSameItemId(ContactInfoForLinking contact1, ContactInfoForLinking contact2)
		{
			return contact1.ItemId != null && contact1.ItemId.ObjectId != null && contact2.ItemId != null && contact2.ItemId.ObjectId != null && contact1.ItemId.ObjectId.Equals(contact2.ItemId.ObjectId);
		}

		public static readonly IEqualityComparer<ContactInfoForLinking> Instance = new ContactInfoForLinkingComparerByItemId();
	}
}
