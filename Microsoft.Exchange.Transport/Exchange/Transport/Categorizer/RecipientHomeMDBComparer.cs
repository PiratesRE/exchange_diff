using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Transport.Categorizer
{
	internal sealed class RecipientHomeMDBComparer : IComparer<MailRecipient>
	{
		public static Guid GetHomeMDBGuid(MailRecipient recipient)
		{
			if (recipient == null || !recipient.IsActive)
			{
				return Guid.Empty;
			}
			ADObjectId value = recipient.ExtendedProperties.GetValue<ADObjectId>("Microsoft.Exchange.Transport.DirectoryData.Database", null);
			if (value != null)
			{
				return value.ObjectGuid;
			}
			return Guid.Empty;
		}

		public int Compare(MailRecipient leftRecipient, MailRecipient rightRecipient)
		{
			return RecipientHomeMDBComparer.GetHomeMDBGuid(leftRecipient).CompareTo(RecipientHomeMDBComparer.GetHomeMDBGuid(rightRecipient));
		}
	}
}
