using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class HierarchyManifestProvider : ManifestProviderBase<MapiHierarchyManifestEx, HierarchySyncPhase>, IMapiHierarchyManifestCallback
	{
		internal HierarchyManifestProvider(CoreFolder folder, ManifestConfigFlags flags, StorageIcsState initialState, PropertyDefinition[] includeProperties, PropertyDefinition[] excludeProperties) : base(folder, flags, null, initialState, includeProperties, excludeProperties)
		{
		}

		protected override bool IsValidTransition(HierarchySyncPhase oldPhase, HierarchySyncPhase newPhase)
		{
			return (oldPhase == HierarchySyncPhase.None && newPhase != HierarchySyncPhase.None) || (oldPhase == HierarchySyncPhase.Change && (newPhase == HierarchySyncPhase.Change || newPhase == HierarchySyncPhase.Delete));
		}

		protected override MapiHierarchyManifestEx MapiCreateManifest(ManifestConfigFlags flags, Restriction restriction, StorageIcsState initialState, ICollection<PropTag> includePropertyTags, ICollection<PropTag> excludePropertyTags)
		{
			return base.MapiFolder.CreateExportHierarchyManifestEx(HierarchyManifestProvider.ConvertManifestConfigFlags(flags), initialState.StateIdsetGiven, initialState.StateCnsetSeen, this, includePropertyTags, excludePropertyTags);
		}

		protected override void MapiGetFinalState(ref StorageIcsState finalState)
		{
			byte[] stateIdsetGiven;
			byte[] stateCnsetSeen;
			base.MapiManifest.GetState(out stateIdsetGiven, out stateCnsetSeen);
			finalState.StateIdsetGiven = stateIdsetGiven;
			finalState.StateCnsetSeen = stateCnsetSeen;
		}

		protected override ManifestStatus MapiSynchronize()
		{
			return base.MapiManifest.Synchronize();
		}

		protected override DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<HierarchyManifestProvider>(this);
		}

		public bool TryGetNextChange(out ManifestFolderChange change)
		{
			this.CheckDisposed(null);
			return base.TryGetChange<ManifestFolderChange>(HierarchySyncPhase.Change, out change);
		}

		public bool TryGetDeletion(out ManifestFolderDeletion deletion)
		{
			this.CheckDisposed(null);
			return base.TryGetChange<ManifestFolderDeletion>(HierarchySyncPhase.Delete, out deletion);
		}

		ManifestCallbackStatus IMapiHierarchyManifestCallback.Change(PropValue[] propertyValues)
		{
			if (propertyValues == null || propertyValues.Length == 0)
			{
				throw new ArgumentNullException("propertyValues");
			}
			base.SetChange(HierarchySyncPhase.Change, new ManifestFolderChange(base.FromMapiPropValueToXsoPropValue(propertyValues)));
			return ManifestCallbackStatus.Yield;
		}

		ManifestCallbackStatus IMapiHierarchyManifestCallback.Delete(byte[] idsetDeleted)
		{
			Util.ThrowOnNullArgument(idsetDeleted, "idsetDeleted");
			base.SetChange(HierarchySyncPhase.Delete, new ManifestFolderDeletion(idsetDeleted));
			return ManifestCallbackStatus.Yield;
		}

		private static SyncConfigFlags ConvertManifestConfigFlags(ManifestConfigFlags syncFlag)
		{
			EnumValidator.ThrowIfInvalid<ManifestConfigFlags>(syncFlag, HierarchyManifestProvider.validConvertOptions);
			SyncConfigFlags result = SyncConfigFlags.None;
			ManifestProviderBase<MapiHierarchyManifestEx, HierarchySyncPhase>.TranslateFlag(ManifestConfigFlags.NoDeletions, SyncConfigFlags.NoDeletions, syncFlag, ref result);
			ManifestProviderBase<MapiHierarchyManifestEx, HierarchySyncPhase>.TranslateFlag(ManifestConfigFlags.Catchup, SyncConfigFlags.Catchup, syncFlag, ref result);
			ManifestProviderBase<MapiHierarchyManifestEx, HierarchySyncPhase>.TranslateFlag(ManifestConfigFlags.NoChanges, SyncConfigFlags.NoChanges, syncFlag, ref result);
			return result;
		}

		private static ManifestConfigFlags[] validConvertOptions = new ManifestConfigFlags[]
		{
			ManifestConfigFlags.None,
			ManifestConfigFlags.NoDeletions,
			ManifestConfigFlags.Catchup,
			ManifestConfigFlags.NoChanges
		};
	}
}
