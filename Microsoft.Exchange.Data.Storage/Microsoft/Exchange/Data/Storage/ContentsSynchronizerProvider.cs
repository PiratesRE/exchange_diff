using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class ContentsSynchronizerProvider : SynchronizerProviderBase
	{
		public ContentsSynchronizerProvider(CoreFolder folder, SynchronizerConfigFlags flags, QueryFilter filter, StorageIcsState initialState, PropertyDefinition[] includeProperties, PropertyDefinition[] excludeProperties, short[] unspecifiedIncludeProperties, short[] unspecifiedExcludeProperties, int bufferSize) : base(folder, flags, filter, initialState, includeProperties, excludeProperties, unspecifiedIncludeProperties, unspecifiedExcludeProperties, bufferSize)
		{
		}

		public ContentsSynchronizerProvider(CoreFolder folder, SynchronizerConfigFlags flags, QueryFilter filter, StorageIcsState initialState, PropertyDefinition[] includeProperties, PropertyDefinition[] excludeProperties, int bufferSize) : base(folder, flags, filter, initialState, includeProperties, excludeProperties, null, null, bufferSize)
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
			return DisposeTracker.Get<ContentsSynchronizerProvider>(this);
		}

		protected override void MapiCreateSynchronizer(SynchronizerConfigFlags flags, Restriction restriction, StorageIcsState initialState, ICollection<PropTag> includePropertyTags, ICollection<PropTag> excludePropertyTags, int fastTransferBlockSize)
		{
			this.synchronizer = base.MapiFolder.CreateSynchronizerEx(initialState.StateIdsetGiven, initialState.StateCnsetSeen, initialState.StateCnsetSeenFAI, initialState.StateCnsetRead, SynchronizerProviderBase.ConvertSynchronizerConfigFlags(flags), restriction, includePropertyTags, excludePropertyTags, fastTransferBlockSize);
		}

		protected override FastTransferBlock MapiGetBuffer(out int residualCacheSize, out bool doneInCache)
		{
			FastTransferBlock buffer = this.synchronizer.GetBuffer(out residualCacheSize, out doneInCache);
			if (buffer.Progress > 0U && !this.reportedMessagesWereDownloaded)
			{
				base.CoreFolder.Session.MessagesWereDownloaded = true;
				this.reportedMessagesWereDownloaded = true;
			}
			return buffer;
		}

		protected override void MapiGetFinalState(ref StorageIcsState finalState)
		{
			byte[] stateIdsetGiven;
			byte[] stateCnsetSeen;
			byte[] stateCnsetSeenFAI;
			byte[] stateCnsetRead;
			this.synchronizer.GetState(out stateIdsetGiven, out stateCnsetSeen, out stateCnsetSeenFAI, out stateCnsetRead);
			finalState.StateIdsetGiven = stateIdsetGiven;
			finalState.StateCnsetSeen = stateCnsetSeen;
			finalState.StateCnsetSeenFAI = stateCnsetSeenFAI;
			finalState.StateCnsetRead = stateCnsetRead;
		}

		private MapiSynchronizerEx synchronizer;

		private bool reportedMessagesWereDownloaded;
	}
}
