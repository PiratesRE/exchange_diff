using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class ContentManifestProvider : ManifestProviderBase<MapiManifestEx, ContentSyncPhase>, IMapiManifestExCallback
	{
		internal ContentManifestProvider(CoreFolder folder, ManifestConfigFlags flags, QueryFilter filter, StorageIcsState initialState, PropertyDefinition[] includeProperties) : base(folder, flags, filter, initialState, includeProperties, Array<NativeStorePropertyDefinition>.Empty)
		{
		}

		protected override bool IsValidTransition(ContentSyncPhase oldPhase, ContentSyncPhase newPhase)
		{
			return oldPhase <= newPhase;
		}

		protected override MapiManifestEx MapiCreateManifest(ManifestConfigFlags flags, Restriction restriction, StorageIcsState initialState, ICollection<PropTag> includePropertyTags, ICollection<PropTag> excludePropertyTags)
		{
			return base.MapiFolder.CreateExportManifestEx(ContentManifestProvider.ConvertManifestConfigFlags(flags), restriction, initialState.StateIdsetGiven, initialState.StateCnsetSeen, initialState.StateCnsetSeenFAI, initialState.StateCnsetRead, this, includePropertyTags);
		}

		protected override void MapiGetFinalState(ref StorageIcsState finalState)
		{
			byte[] stateIdsetGiven;
			byte[] stateCnsetSeen;
			byte[] stateCnsetSeenFAI;
			byte[] stateCnsetRead;
			base.MapiManifest.GetState(out stateIdsetGiven, out stateCnsetSeen, out stateCnsetSeenFAI, out stateCnsetRead);
			finalState.StateIdsetGiven = stateIdsetGiven;
			finalState.StateCnsetSeen = stateCnsetSeen;
			finalState.StateCnsetSeenFAI = stateCnsetSeenFAI;
			finalState.StateCnsetRead = stateCnsetRead;
		}

		protected override ManifestStatus MapiSynchronize()
		{
			return base.MapiManifest.Synchronize();
		}

		protected override DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<ContentManifestProvider>(this);
		}

		public bool TryGetNextChange(out ManifestItemChange change)
		{
			this.CheckDisposed(null);
			return base.TryGetChange<ManifestItemChange>(ContentSyncPhase.Change, out change);
		}

		public bool TryGetDeletion(out ManifestItemDeletion deletion)
		{
			this.CheckDisposed(null);
			return base.TryGetChange<ManifestItemDeletion>(ContentSyncPhase.Delete, out deletion);
		}

		public bool TryGetReadUnread(out ManifestItemReadUnread readUnread)
		{
			this.CheckDisposed(null);
			return base.TryGetChange<ManifestItemReadUnread>(ContentSyncPhase.ReadUnread, out readUnread);
		}

		ManifestCallbackStatus IMapiManifestExCallback.Change(bool newMessage, PropValue[] headerPropertyValues, PropValue[] propertyValues)
		{
			if (headerPropertyValues == null || headerPropertyValues.Length == 0)
			{
				throw new ArgumentNullException("headerPropertyValues");
			}
			Util.ThrowOnNullArgument(propertyValues, "propertyValues");
			base.SetChange(ContentSyncPhase.Change, new ManifestItemChange(newMessage, base.FromMapiPropValueToXsoPropValue(headerPropertyValues), base.FromMapiPropValueToXsoPropValue(propertyValues)));
			return ManifestCallbackStatus.Yield;
		}

		ManifestCallbackStatus IMapiManifestExCallback.Delete(byte[] idsetDeleted, bool softDeleted, bool expired)
		{
			Util.ThrowOnNullArgument(idsetDeleted, "idsetDeleted");
			base.SetChange(ContentSyncPhase.Delete, new ManifestItemDeletion(idsetDeleted, softDeleted, expired));
			return ManifestCallbackStatus.Yield;
		}

		ManifestCallbackStatus IMapiManifestExCallback.ReadUnread(byte[] idsetReadUnread, bool read)
		{
			Util.ThrowOnNullArgument(idsetReadUnread, "idsetReadUnread");
			base.SetChange(ContentSyncPhase.ReadUnread, new ManifestItemReadUnread(idsetReadUnread, read));
			return ManifestCallbackStatus.Yield;
		}

		private static SyncConfigFlags ConvertManifestConfigFlags(ManifestConfigFlags syncFlag)
		{
			SyncConfigFlags result = SyncConfigFlags.None;
			EnumValidator.ThrowIfInvalid<ManifestConfigFlags>(syncFlag);
			ManifestProviderBase<MapiManifestEx, ContentSyncPhase>.TranslateFlag(ManifestConfigFlags.NoDeletions, SyncConfigFlags.NoDeletions, syncFlag, ref result);
			ManifestProviderBase<MapiManifestEx, ContentSyncPhase>.TranslateFlag(ManifestConfigFlags.NoSoftDeletions, SyncConfigFlags.NoSoftDeletions, syncFlag, ref result);
			ManifestProviderBase<MapiManifestEx, ContentSyncPhase>.TranslateFlag(ManifestConfigFlags.ReadState, SyncConfigFlags.ReadState, syncFlag, ref result);
			ManifestProviderBase<MapiManifestEx, ContentSyncPhase>.TranslateFlag(ManifestConfigFlags.Associated, SyncConfigFlags.Associated, syncFlag, ref result);
			ManifestProviderBase<MapiManifestEx, ContentSyncPhase>.TranslateFlag(ManifestConfigFlags.Normal, SyncConfigFlags.Normal, syncFlag, ref result);
			ManifestProviderBase<MapiManifestEx, ContentSyncPhase>.TranslateFlag(ManifestConfigFlags.Catchup, SyncConfigFlags.Catchup, syncFlag, ref result);
			ManifestProviderBase<MapiManifestEx, ContentSyncPhase>.TranslateFlag(ManifestConfigFlags.NoChanges, SyncConfigFlags.NoChanges, syncFlag, ref result);
			ManifestProviderBase<MapiManifestEx, ContentSyncPhase>.TranslateFlag(ManifestConfigFlags.OrderByDeliveryTime, SyncConfigFlags.OrderByDeliveryTime, syncFlag, ref result);
			return result;
		}
	}
}
