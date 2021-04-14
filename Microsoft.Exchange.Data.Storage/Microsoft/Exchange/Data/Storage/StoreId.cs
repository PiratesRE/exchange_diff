using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;

namespace Microsoft.Exchange.Data.Storage
{
	[KnownType(typeof(StoreObjectId))]
	[KnownType(typeof(VersionedId))]
	[KnownType(typeof(ConversationId))]
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[KnownType(typeof(PersonId))]
	[Serializable]
	internal abstract class StoreId : ObjectId, IEquatable<StoreId>
	{
		internal StoreId()
		{
		}

		public static StoreObjectId GetStoreObjectId(StoreId id)
		{
			if (id == null)
			{
				return null;
			}
			VersionedId versionedId = id as VersionedId;
			if (versionedId != null)
			{
				return versionedId.ObjectId;
			}
			return id as StoreObjectId;
		}

		public static string StoreIdToEwsId(Guid mailboxGuid, StoreId storeId)
		{
			Util.ThrowOnNullArgument(storeId, "storeId");
			MailboxId mailboxId = new MailboxId(mailboxGuid);
			IdHeaderInformation idHeaderInformation = new IdHeaderInformation();
			StoreObjectId storeObjectId = StoreId.GetStoreObjectId(storeId);
			if (storeObjectId.ObjectType == StoreObjectType.CalendarItemOccurrence)
			{
				idHeaderInformation.StoreIdBytes = storeObjectId.GetBytes();
				idHeaderInformation.IdProcessingInstruction = IdProcessingInstruction.Recurrence;
			}
			else if (storeObjectId.ObjectType == StoreObjectType.CalendarItemSeries)
			{
				idHeaderInformation.StoreIdBytes = storeObjectId.ProviderLevelItemId;
				idHeaderInformation.IdProcessingInstruction = IdProcessingInstruction.Series;
			}
			else
			{
				idHeaderInformation.StoreIdBytes = storeObjectId.ProviderLevelItemId;
				idHeaderInformation.IdProcessingInstruction = IdProcessingInstruction.Normal;
			}
			idHeaderInformation.IdStorageType = IdStorageType.MailboxItemMailboxGuidBased;
			idHeaderInformation.MailboxId = mailboxId;
			return ServiceIdConverter.ConvertToConcatenatedId(idHeaderInformation, null, true);
		}

		public static string PublicFolderStoreIdToEwsId(StoreId storeId, StoreId parentFolderId)
		{
			Util.ThrowOnNullArgument(storeId, "storeId");
			IdHeaderInformation idHeaderInformation = new IdHeaderInformation();
			StoreObjectId storeObjectId = StoreId.GetStoreObjectId(storeId);
			if (storeObjectId.ObjectType == StoreObjectType.CalendarItemOccurrence)
			{
				idHeaderInformation.StoreIdBytes = storeObjectId.GetBytes();
				idHeaderInformation.IdProcessingInstruction = IdProcessingInstruction.Recurrence;
			}
			else if (storeObjectId.ObjectType == StoreObjectType.CalendarItemSeries)
			{
				idHeaderInformation.StoreIdBytes = storeObjectId.ProviderLevelItemId;
				idHeaderInformation.IdProcessingInstruction = IdProcessingInstruction.Series;
			}
			else
			{
				idHeaderInformation.StoreIdBytes = storeObjectId.ProviderLevelItemId;
				idHeaderInformation.IdProcessingInstruction = IdProcessingInstruction.Normal;
			}
			if (Folder.IsFolderId(storeObjectId))
			{
				idHeaderInformation.IdStorageType = IdStorageType.PublicFolder;
			}
			else
			{
				Util.ThrowOnNullArgument(parentFolderId, "parentFolderId");
				StoreObjectId storeObjectId2 = StoreId.GetStoreObjectId(parentFolderId);
				idHeaderInformation.FolderIdBytes = storeObjectId2.ProviderLevelItemId;
				idHeaderInformation.IdStorageType = IdStorageType.PublicFolderItem;
			}
			return ServiceIdConverter.ConvertToConcatenatedId(idHeaderInformation, null, true);
		}

		public static StoreObjectId EwsIdToFolderStoreObjectId(string id)
		{
			IdHeaderInformation idHeaderInformation = ServiceIdConverter.ConvertFromConcatenatedId(id, BasicTypes.Folder, null);
			return idHeaderInformation.ToStoreObjectId();
		}

		public static StoreObjectId EwsIdToStoreObjectId(string id)
		{
			IdHeaderInformation idHeaderInformation = ServiceIdConverter.ConvertFromConcatenatedId(id, BasicTypes.Item, null);
			return idHeaderInformation.ToStoreObjectId();
		}

		public abstract string ToBase64String();

		public abstract bool Equals(StoreId id);

		internal static byte[][] StoreIdsToEntryIds(params StoreId[] itemIds)
		{
			byte[][] array = new byte[itemIds.Length][];
			for (int i = 0; i < itemIds.Length; i++)
			{
				if (itemIds[i] == null)
				{
					ExTraceGlobals.StorageTracer.TraceError(0L, "ItemId::ItemIdsToEntryIds. The in itemId cannot be null.");
					throw new ArgumentException(ServerStrings.ExNullItemIdParameter(i));
				}
				array[i] = StoreId.StoreIdToEntryId(itemIds[i], i);
			}
			return array;
		}

		internal static void SplitStoreObjectIdAndChangeKey(StoreId id, out StoreObjectId storeObjectId, out byte[] changeKey)
		{
			VersionedId versionedId = id as VersionedId;
			if (versionedId != null)
			{
				changeKey = versionedId.ChangeKeyAsByteArray();
				storeObjectId = versionedId.ObjectId;
				return;
			}
			changeKey = null;
			storeObjectId = (StoreObjectId)id;
		}

		internal static byte[] Base64ToByteArray(string base64String)
		{
			byte[] result;
			try
			{
				result = Convert.FromBase64String(base64String);
			}
			catch (FormatException innerException)
			{
				throw new CorruptDataException(ServerStrings.InvalidBase64String, innerException);
			}
			return result;
		}

		private static byte[] StoreIdToEntryId(StoreId id, int index)
		{
			if (id == null)
			{
				ExTraceGlobals.StorageTracer.TraceError<int>(0L, "Folder::IItemIdToEntryId. The element cannot be null. Index = {0}.", index);
				throw new ArgumentException(ServerStrings.ExNullItemIdParameter(index));
			}
			return StoreId.GetStoreObjectId(id).ProviderLevelItemId;
		}

		public static readonly IEqualityComparer<StoreId> EqualityComparer = new StoreId.Comparer();

		private sealed class Comparer : IEqualityComparer<StoreId>
		{
			public bool Equals(StoreId a, StoreId b)
			{
				return object.ReferenceEquals(a, b) || (a != null && b != null && a.Equals(b));
			}

			public int GetHashCode(StoreId storeId)
			{
				return storeId.GetHashCode();
			}
		}
	}
}
