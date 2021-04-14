using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Collections;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.RpcClientAccess.FastTransfer.Parser;
using Microsoft.Exchange.RpcClientAccess.Handler;

namespace Microsoft.Exchange.RpcClientAccess.FastTransfer.Handler
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class HierarchySynchronizer : SynchronizerBase, IHierarchySynchronizer, IDisposable
	{
		internal HierarchySynchronizer(ReferenceCount<CoreFolder> referenceCoreFolder, SyncFlag syncFlags, SyncExtraFlag extraFlags, IcsState icsState, params PropertyTag[] referencePropertyTags) : base(referenceCoreFolder, syncFlags, extraFlags, icsState)
		{
			using (DisposeGuard disposeGuard = this.Guard())
			{
				this.excludePropertyTags = new HashSet<PropertyTag>(referencePropertyTags);
				ManifestConfigFlags manifestFlags = HierarchySynchronizer.GetManifestFlags(syncFlags, extraFlags, ref this.excludePropertyTags);
				this.syncRootFolderLongTermId = referenceCoreFolder.ReferencedObject.Session.IdConverter.GetLongTermIdFromId(referenceCoreFolder.ReferencedObject.Session.IdConverter.GetFidFromId(referenceCoreFolder.ReferencedObject.Id.ObjectId));
				NativeStorePropertyDefinition[] propertyDefinitionsIgnoreTypeChecking = MEDSPropertyTranslator.GetPropertyDefinitionsIgnoreTypeChecking(referenceCoreFolder.ReferencedObject.Session, referenceCoreFolder.ReferencedObject.PropertyBag, this.excludePropertyTags.ToArray<PropertyTag>());
				IPropertyBag propertyBag = new MemoryPropertyBag(this.SessionAdaptor);
				icsState.Checkpoint(propertyBag);
				IcsStateStream icsStateStream = new IcsStateStream(propertyBag);
				this.manifestProvider = this.SyncRootFolder.ReferencedObject.GetHierarchyManifest(manifestFlags, icsStateStream.ToXsoState(), Array<StorePropertyDefinition>.Empty, propertyDefinitionsIgnoreTypeChecking);
				disposeGuard.Success();
			}
		}

		public static void CheckFlags(SyncFlag syncFlag, SyncExtraFlag extraFlags)
		{
			HashSet<PropertyTag> hashSet = new HashSet<PropertyTag>();
			HierarchySynchronizer.GetManifestFlags(syncFlag, extraFlags, ref hashSet);
		}

		public IEnumerator<IFolderChange> GetChanges()
		{
			base.CheckDisposed();
			ManifestFolderChange change;
			while (this.manifestProvider.TryGetNextChange(out change))
			{
				IPropertyBag propertyBag = new MemoryPropertyBag(this.SessionAdaptor);
				SynchronizerBase.SetPropertyValuesFromServer(propertyBag, this.SyncRootFolder.ReferencedObject.Session, change.PropertyValues);
				this.CheckAndFixFolderChanges(propertyBag);
				yield return new FolderChangeAdaptor(propertyBag);
			}
			yield break;
		}

		public IPropertyBag GetDeletions()
		{
			base.CheckDisposed();
			IPropertyBag propertyBag = new MemoryPropertyBag(this.SessionAdaptor);
			ManifestFolderDeletion manifestFolderDeletion;
			if (this.manifestProvider.TryGetDeletion(out manifestFolderDeletion))
			{
				propertyBag.SetProperty(new PropertyValue(PropertyTag.IdsetDeleted, manifestFolderDeletion.IdsetDeleted));
			}
			else
			{
				propertyBag.Delete(PropertyTag.IdsetDeleted);
			}
			return propertyBag;
		}

		public IIcsState GetFinalState()
		{
			base.CheckDisposed();
			IPropertyBag propertyBag = new MemoryPropertyBag(this.SessionAdaptor);
			IcsStateStream icsStateStream = new IcsStateStream(propertyBag);
			StorageIcsState state = icsStateStream.ToXsoState();
			this.manifestProvider.GetFinalState(ref state);
			icsStateStream.FromXsoState(state);
			this.IcsState.Load(IcsStateOrigin.ServerFinal, propertyBag);
			return new IcsStateAdaptor(this.IcsState, this.SyncRootFolder);
		}

		protected override void InternalDispose()
		{
			Util.DisposeIfPresent(this.manifestProvider);
			base.InternalDispose();
		}

		protected override DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<HierarchySynchronizer>(this);
		}

		private static ManifestConfigFlags GetManifestFlags(SyncFlag syncFlag, SyncExtraFlag extraFlags, ref HashSet<PropertyTag> excludePropertyTags)
		{
			ManifestConfigFlags result = ManifestConfigFlags.None;
			if ((ushort)(syncFlag & ~(SyncFlag.Unicode | SyncFlag.NoDeletions | SyncFlag.NoForeignKeys)) != 0)
			{
				throw new RopExecutionException(string.Format("Unsupported SyncFlag present: {0}", syncFlag), (ErrorCode)2147746050U);
			}
			SynchronizerBase.TranslateFlag(SyncFlag.NoDeletions, ManifestConfigFlags.NoDeletions, syncFlag, ref result);
			if ((extraFlags & (SyncExtraFlag.MessageSize | SyncExtraFlag.OrderByDeliveryTime | SyncExtraFlag.ManifestMode | SyncExtraFlag.CatchUpFull)) != SyncExtraFlag.None)
			{
				throw new RopExecutionException(string.Format("Unsupported SyncExtraFlag present: {0}", extraFlags), (ErrorCode)2147746050U);
			}
			excludePropertyTags.UnionWith(HierarchySynchronizer.MustExcludeProperties);
			excludePropertyTags.ExceptWith(IcsHierarchySynchronizer.RequiredFolderChangeProperties);
			if ((ushort)(syncFlag & SyncFlag.Unicode) == 0)
			{
				throw Feature.NotImplemented(212753, "codepage conversion");
			}
			return result;
		}

		private void HonorOptionNoForeignKeys(IPropertyBag propertyBag)
		{
			if ((ushort)(this.SyncFlags & SyncFlag.NoForeignKeys) == 256)
			{
				propertyBag.SetProperty(new PropertyValue(PropertyTag.ExternalFid, base.ConvertIdToLongTermId(propertyBag, PropertyTag.Fid)));
				propertyBag.SetProperty(new PropertyValue(PropertyTag.ExternalParentFid, base.ConvertIdToLongTermId(propertyBag, PropertyTag.ParentFid)));
			}
		}

		private void CheckAndFixFolderChanges(IPropertyBag propertyBag)
		{
			SynchronizerBase.CheckRequiredProperties(propertyBag, HierarchySynchronizer.RequiredInputProperties);
			this.HonorOptionNoForeignKeys(propertyBag);
			if (this.IsChildOfSyncRoot(propertyBag))
			{
				propertyBag.SetProperty(new PropertyValue(PropertyTag.ExternalParentFid, Array<byte>.Empty));
			}
		}

		private bool IsChildOfSyncRoot(IPropertyBag propertyBag)
		{
			return ArrayComparer<byte>.Comparer.Equals(this.syncRootFolderLongTermId, propertyBag.GetAnnotatedProperty(PropertyTag.ExternalParentFid).PropertyValue.GetValue<byte[]>());
		}

		private static readonly PropertyTag[] MustExcludeProperties = new PropertyTag[]
		{
			PropertyTag.FreeBusyNTSD
		};

		private static readonly PropertyTag[] RequiredInputProperties = Util.MergeArrays<PropertyTag>(new ICollection<PropertyTag>[]
		{
			IcsHierarchySynchronizer.RequiredFolderChangeProperties,
			new PropertyTag[]
			{
				PropertyTag.Fid,
				PropertyTag.ParentFid
			}
		});

		private readonly HierarchyManifestProvider manifestProvider;

		private readonly byte[] syncRootFolderLongTermId;

		private readonly HashSet<PropertyTag> excludePropertyTags;
	}
}
