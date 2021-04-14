using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class HierarchySynchronizerProvider : SynchronizerProviderBase
	{
		public HierarchySynchronizerProvider(CoreFolder folder, SynchronizerConfigFlags flags, QueryFilter filter, StorageIcsState initialState, PropertyDefinition[] includeProperties, PropertyDefinition[] excludeProperties, short[] unspecifiedIncludeProperties, short[] unspecifiedExcludeProperties, int bufferSize) : base(folder, flags, filter, initialState, includeProperties, excludeProperties, unspecifiedIncludeProperties, unspecifiedExcludeProperties, bufferSize)
		{
		}

		public HierarchySynchronizerProvider(CoreFolder folder, SynchronizerConfigFlags flags, QueryFilter filter, StorageIcsState initialState, PropertyDefinition[] includeProperties, PropertyDefinition[] excludeProperties, int bufferSize) : base(folder, flags, filter, initialState, includeProperties, excludeProperties, null, null, bufferSize)
		{
		}

		protected override void InternalDispose(bool disposing)
		{
			if (disposing)
			{
				Util.DisposeIfPresent(this.synchronizer);
			}
			base.InternalDispose(disposing);
		}

		protected override DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<HierarchySynchronizerProvider>(this);
		}

		protected override void MapiCreateSynchronizer(SynchronizerConfigFlags flags, Restriction restriction, StorageIcsState initialState, ICollection<PropTag> includePropertyTags, ICollection<PropTag> excludePropertyTags, int fastTransferBlockSize)
		{
			this.synchronizer = base.MapiFolder.CreateHierarchySynchronizerEx(initialState.StateIdsetGiven, initialState.StateCnsetSeen, SynchronizerProviderBase.ConvertSynchronizerConfigFlags(flags), restriction, includePropertyTags, excludePropertyTags, fastTransferBlockSize);
		}

		protected override FastTransferBlock MapiGetBuffer(out int residualCacheSize, out bool doneInCache)
		{
			return this.synchronizer.GetBuffer(out residualCacheSize, out doneInCache);
		}

		protected override void MapiGetFinalState(ref StorageIcsState finalState)
		{
			byte[] stateIdsetGiven;
			byte[] stateCnsetSeen;
			this.synchronizer.GetState(out stateIdsetGiven, out stateCnsetSeen);
			finalState.StateIdsetGiven = stateIdsetGiven;
			finalState.StateCnsetSeen = stateCnsetSeen;
		}

		private MapiHierarchySynchronizerEx synchronizer;
	}
}
