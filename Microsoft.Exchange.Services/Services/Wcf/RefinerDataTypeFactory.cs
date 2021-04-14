using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.Exchange.Conversion;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Search.Core.Common;
using Microsoft.Exchange.Search.Query;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Wcf.Types;

namespace Microsoft.Exchange.Services.Wcf
{
	internal class RefinerDataTypeFactory
	{
		internal static RefinerDataType TryCreate(RefinerData refinerData, MailboxSession mbxSession, MailboxId mailboxId)
		{
			if (!RefinerDataTypeFactory.PropertyDefinitionToCategoryMap.ContainsKey(refinerData.Property))
			{
				return null;
			}
			RefinerCategoryType refinerCategory = RefinerDataTypeFactory.PropertyDefinitionToCategoryMap[refinerData.Property];
			List<RefinerDataEntryType> list = new List<RefinerDataEntryType>(refinerData.Entries.Count);
			switch (refinerCategory)
			{
			case RefinerCategoryType.DateTimeReceived:
				return null;
			case RefinerCategoryType.SearchRecipients:
			case RefinerCategoryType.From:
				goto IL_10A;
			case RefinerCategoryType.HasAttachment:
				break;
			case RefinerCategoryType.FolderEntryId:
				using (IEnumerator<RefinerDataEntry> enumerator = refinerData.Entries.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						RefinerDataEntry refinerDataEntry = enumerator.Current;
						list.Add(new FolderRefinerDataEntryType(RefinerDataTypeFactory.GetDisplayNameForFolder(refinerDataEntry.DisplayName, mbxSession, mailboxId), refinerDataEntry.HitCount, refinerDataEntry.RefinementQuery));
					}
					goto IL_17A;
				}
				break;
			default:
				goto IL_17A;
			}
			using (IEnumerator<RefinerDataEntry> enumerator2 = refinerData.Entries.GetEnumerator())
			{
				while (enumerator2.MoveNext())
				{
					RefinerDataEntry refinerDataEntry2 = enumerator2.Current;
					list.Add(new AttachmentRefinerDataEntryType(!"0".Equals(refinerDataEntry2.DisplayName), refinerDataEntry2.HitCount, refinerDataEntry2.RefinementQuery));
				}
				goto IL_17A;
			}
			IL_10A:
			foreach (RefinerDataEntry refinerDataEntry3 in refinerData.Entries)
			{
				string displayName = refinerDataEntry3.DisplayName;
				SearchParticipant searchParticipant = SearchParticipant.FromParticipantString(refinerDataEntry3.DisplayName);
				if (searchParticipant != null)
				{
					list.Add(new PeopleRefinerDataEntryType(searchParticipant.DisplayName, searchParticipant.SmtpAddress, refinerDataEntry3.HitCount, refinerDataEntry3.RefinementQuery));
				}
			}
			IL_17A:
			if (list.Count == 0)
			{
				return null;
			}
			return new RefinerDataType(refinerCategory, list.ToArray());
		}

		private static FolderId GetDisplayNameForFolder(string fastFolderId, MailboxSession mbxSession, MailboxId mailboxId)
		{
			byte[] longTermId = HexConverter.HexStringToByteArray(fastFolderId);
			Array.Resize<byte>(ref longTermId, 22);
			long idFromLongTermId = mbxSession.IdConverter.GetIdFromLongTermId(longTermId);
			StoreObjectId storeId = mbxSession.IdConverter.CreateFolderId(idFromLongTermId);
			return IdConverter.GetFolderIdFromStoreId(storeId, mailboxId);
		}

		private static readonly IDictionary<PropertyDefinition, RefinerCategoryType> PropertyDefinitionToCategoryMap = new ReadOnlyDictionary<PropertyDefinition, RefinerCategoryType>(new Dictionary<PropertyDefinition, RefinerCategoryType>
		{
			{
				ItemSchema.ReceivedTime,
				RefinerCategoryType.DateTimeReceived
			},
			{
				ItemSchema.SearchRecipients,
				RefinerCategoryType.SearchRecipients
			},
			{
				ItemSchema.From,
				RefinerCategoryType.From
			},
			{
				ItemSchema.HasAttachment,
				RefinerCategoryType.HasAttachment
			},
			{
				StoreObjectSchema.ParentEntryId,
				RefinerCategoryType.FolderEntryId
			}
		});
	}
}
