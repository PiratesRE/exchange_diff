using System;
using System.Collections.Generic;
using Microsoft.Exchange.Collections;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class WellKnownPublicFolders
	{
		public WellKnownPublicFolders(PropValue[] propValues)
		{
			this.folderIdMappings = new Dictionary<WellKnownPublicFolders.FolderType, byte[]>(propValues.Length);
			this.longTermIdMappings = new Dictionary<byte[], WellKnownPublicFolders.FolderType?>(propValues.Length, ArrayComparer<byte>.Comparer);
			foreach (PropValue propValue in propValues)
			{
				WellKnownPublicFolders.FolderType? folderType = WellKnownPublicFolders.GetFolderType(propValue.PropTag);
				if (folderType != null)
				{
					byte[] bytes = propValue.GetBytes();
					this.folderIdMappings.Add(folderType.Value, bytes);
					StoreObjectId storeObjectId = StoreObjectId.FromProviderSpecificId(bytes, StoreObjectType.Folder);
					this.longTermIdMappings.Add(storeObjectId.LongTermFolderId, new WellKnownPublicFolders.FolderType?(folderType.Value));
				}
			}
		}

		public static WellKnownPublicFolders.FolderType? GetFolderType(PropTag propTag)
		{
			EnumValidator.ThrowIfInvalid<PropTag>(propTag, "propTag");
			if (propTag <= PropTag.IpmOutboxEntryId)
			{
				if (propTag <= PropTag.IpmSubtreeEntryId)
				{
					if (propTag == PropTag.RootEntryId)
					{
						return new WellKnownPublicFolders.FolderType?(WellKnownPublicFolders.FolderType.Root);
					}
					if (propTag == PropTag.IpmSubtreeEntryId)
					{
						return new WellKnownPublicFolders.FolderType?(WellKnownPublicFolders.FolderType.EFormsRegistry);
					}
				}
				else
				{
					if (propTag == PropTag.IpmInboxEntryId)
					{
						return new WellKnownPublicFolders.FolderType?(WellKnownPublicFolders.FolderType.DumpsterRoot);
					}
					if (propTag == PropTag.IpmOutboxEntryId)
					{
						return new WellKnownPublicFolders.FolderType?(WellKnownPublicFolders.FolderType.TombstoneRoot);
					}
				}
			}
			else if (propTag <= PropTag.IpmSentMailEntryId)
			{
				if (propTag == PropTag.IpmWasteBasketEntryId)
				{
					return new WellKnownPublicFolders.FolderType?(WellKnownPublicFolders.FolderType.InternalSubmission);
				}
				if (propTag == PropTag.IpmSentMailEntryId)
				{
					return new WellKnownPublicFolders.FolderType?(WellKnownPublicFolders.FolderType.AsyncDeleteState);
				}
			}
			else
			{
				if (propTag == PropTag.SpoolerQueueEntryId)
				{
					return new WellKnownPublicFolders.FolderType?(WellKnownPublicFolders.FolderType.NonIpmSubtree);
				}
				if (propTag == PropTag.DeferredActionFolderEntryID)
				{
					return new WellKnownPublicFolders.FolderType?(WellKnownPublicFolders.FolderType.IpmSubtree);
				}
			}
			return null;
		}

		public bool GetFolderType(byte[] folderId, out WellKnownPublicFolders.FolderType? folderType)
		{
			folderType = null;
			foreach (KeyValuePair<WellKnownPublicFolders.FolderType, byte[]> keyValuePair in this.folderIdMappings)
			{
				if (ArrayComparer<byte>.Comparer.Equals(keyValuePair.Value, folderId))
				{
					folderType = new WellKnownPublicFolders.FolderType?(keyValuePair.Key);
					return true;
				}
			}
			return false;
		}

		public bool TryGetFolderTypeFromLongTermId(byte[] folderId, out WellKnownPublicFolders.FolderType? folderType)
		{
			ArgumentValidator.ThrowIfNull("folerId", folderId);
			ArgumentValidator.ThrowIfOutOfRange<int>("folerId.Length", folderId.Length, 22, 22);
			return this.longTermIdMappings.TryGetValue(folderId, out folderType);
		}

		public byte[] GetFolderId(WellKnownPublicFolders.FolderType folderType)
		{
			EnumValidator.ThrowIfInvalid<WellKnownPublicFolders.FolderType>(folderType, "folderType");
			return this.folderIdMappings[folderType];
		}

		public const PropTag RootEntryIdPropTag = PropTag.RootEntryId;

		public const PropTag IpmSubtreeEntryIdPropTag = PropTag.DeferredActionFolderEntryID;

		public const PropTag NonIpmSubtreeEntryIdPropTag = PropTag.SpoolerQueueEntryId;

		public const PropTag EFormsRegistryEntryIdPropTag = PropTag.IpmSubtreeEntryId;

		public const PropTag DumpsterRootEntryIdPropTag = PropTag.IpmInboxEntryId;

		public const PropTag AsyncDeleteStateEntryIdPropTag = PropTag.IpmSentMailEntryId;

		public const PropTag InternalSubmissionEntryIdPropTag = PropTag.IpmWasteBasketEntryId;

		public const PropTag TombstonesRootEntryIdPropTag = PropTag.IpmOutboxEntryId;

		public static PropTag[] EntryIdPropTags = new PropTag[]
		{
			PropTag.RootEntryId,
			PropTag.DeferredActionFolderEntryID,
			PropTag.SpoolerQueueEntryId,
			PropTag.IpmSubtreeEntryId,
			PropTag.IpmInboxEntryId,
			PropTag.IpmSentMailEntryId,
			PropTag.IpmWasteBasketEntryId,
			PropTag.IpmOutboxEntryId
		};

		private readonly Dictionary<WellKnownPublicFolders.FolderType, byte[]> folderIdMappings;

		private readonly Dictionary<byte[], WellKnownPublicFolders.FolderType?> longTermIdMappings;

		internal enum FolderType
		{
			Root,
			IpmSubtree,
			NonIpmSubtree,
			EFormsRegistry,
			DumpsterRoot,
			AsyncDeleteState,
			InternalSubmission,
			TombstoneRoot
		}
	}
}
