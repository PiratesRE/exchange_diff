using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.Search;
using Microsoft.Exchange.Inference.Common;
using Microsoft.Exchange.Inference.Mdb;
using Microsoft.Exchange.Search.Core.Abstraction;

namespace Microsoft.Exchange.Inference.PeopleRelevance
{
	internal class MdbPeoplePropertyMap : MdbPropertyMap
	{
		public static MdbPeoplePropertyMap Instance
		{
			get
			{
				if (MdbPeoplePropertyMap.instance == null)
				{
					MdbPeoplePropertyMap.instance = new MdbPeoplePropertyMap();
				}
				return MdbPeoplePropertyMap.instance;
			}
		}

		private static object GetRecipientsTo(IItem item, StorePropertyDefinition propertyDefinition, IMdbPropertyMappingContext context)
		{
			return MdbPeoplePropertyMap.GetRecipients(item, RecipientItemType.To);
		}

		private static object GetRecipientsCc(IItem item, StorePropertyDefinition propertyDefinition, IMdbPropertyMappingContext context)
		{
			return MdbPeoplePropertyMap.GetRecipients(item, RecipientItemType.Cc);
		}

		private static object GetRecipients(IItem item, RecipientItemType recipientType)
		{
			MessageItem messageItem = item as MessageItem;
			List<IMessageRecipient> list = null;
			if (messageItem != null)
			{
				list = new List<IMessageRecipient>(16);
				foreach (Recipient recipient in messageItem.Recipients)
				{
					if (recipient.RecipientItemType == recipientType)
					{
						if (!string.IsNullOrEmpty(recipient.Participant.GetValueOrDefault<string>(ParticipantSchema.SmtpAddress)))
						{
							list.Add(new MdbRecipient(recipient));
						}
						else
						{
							ExTraceGlobals.CoreDocumentModelTracer.TraceError<string, RecipientItemType, StoreObjectId>(0L, "Recipient {0} (type {1}) contains a bad email address in message {2}", recipient.Participant.DisplayName, recipientType, messageItem.StoreObjectId);
						}
					}
				}
			}
			return list;
		}

		private static object GetIsReply(PropertyDefinition genericPropertyDefinition, StorePropertyDefinition storePropertyDefinition, object underlyingPropertyValue)
		{
			if (underlyingPropertyValue == null || underlyingPropertyValue is PropertyError)
			{
				return false;
			}
			return true;
		}

		[PropertyMapping]
		public static readonly MdbPropertyMapping SentTime = new MdbOneToOneSimplePropertyMapping(PeopleRelevanceSchema.SentTime, ItemSchema.SentTime);

		[PropertyMapping]
		public static readonly MdbPropertyMapping RecipientsTo = new MdbOneToOneTransformPropertyMapping(PeopleRelevanceSchema.RecipientsTo, ItemSchema.DisplayTo, new MdbOneToOnePropertyMapping.ItemGetterDelegate(MdbPeoplePropertyMap.GetRecipientsTo), null);

		[PropertyMapping]
		public static readonly MdbPropertyMapping RecipientsCc = new MdbOneToOneTransformPropertyMapping(PeopleRelevanceSchema.RecipientsCc, ItemSchema.DisplayCc, new MdbOneToOnePropertyMapping.ItemGetterDelegate(MdbPeoplePropertyMap.GetRecipientsCc), null);

		[PropertyMapping]
		public static readonly MdbPropertyMapping IsReply = new MdbOneToOneTransformPropertyMapping(PeopleRelevanceSchema.IsReply, ItemSchema.InReplyTo, new MdbOneToOneTransformPropertyMapping.TransformDelegate(MdbPeoplePropertyMap.GetIsReply), null);

		private static MdbPeoplePropertyMap instance = null;
	}
}
