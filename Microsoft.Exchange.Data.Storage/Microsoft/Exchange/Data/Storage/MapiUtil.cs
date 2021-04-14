using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal static class MapiUtil
	{
		internal static RecipientType RecipientItemTypeToMapiRecipientType(RecipientItemType recipientItemType, bool isSubmitted)
		{
			return (RecipientType)(recipientItemType | (isSubmitted ? ((RecipientItemType)(-2147483648)) : ((RecipientItemType)0)));
		}

		internal static SearchCriteriaFlags SetSearchCriteriaFlagsToMapiSearchCriteriaFlags(SetSearchCriteriaFlags setSearchCriteriaFlags)
		{
			return (SearchCriteriaFlags)setSearchCriteriaFlags;
		}

		internal static RecipientItemType MapiRecipientTypeToRecipientItemType(RecipientType recipientType)
		{
			if (recipientType == RecipientType.Unknown)
			{
				return RecipientItemType.Unknown;
			}
			RecipientType recipientType2 = recipientType & (RecipientType)2147483647;
			RecipientType recipientType3 = recipientType2;
			switch (recipientType3)
			{
			case RecipientType.To:
			case RecipientType.Cc:
			case RecipientType.Bcc:
				break;
			default:
				if (recipientType3 != RecipientType.P1)
				{
					return RecipientItemType.Unknown;
				}
				break;
			}
			return (RecipientItemType)recipientType2;
		}
	}
}
