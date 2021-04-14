using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Mapi.Unmanaged;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal abstract class MapiSynchronizerExBase : MapiUnk
	{
		internal MapiSynchronizerExBase(SafeExInterfaceHandle iExchangeInterfaceHandle, MapiStore mapiStore) : base(iExchangeInterfaceHandle, null, mapiStore)
		{
			this.dataBlocks = new Queue<FastTransferBlock>();
			this.done = false;
			this.error = false;
		}

		public FastTransferBlock GetBuffer(out int residualCacheSize, out bool doneInCache)
		{
			base.CheckDisposed();
			base.LockStore();
			residualCacheSize = 0;
			doneInCache = false;
			FastTransferBlock result;
			try
			{
				FastTransferBlock fastTransferBlock;
				while (!this.TryGetDataBlock(out fastTransferBlock))
				{
					if (this.done)
					{
						doneInCache = true;
						return FastTransferBlock.Done;
					}
					if (this.error)
					{
						doneInCache = true;
						return FastTransferBlock.Error;
					}
					this.ReadMoreBlocks();
				}
				if (fastTransferBlock.State == FastTransferState.Done)
				{
					this.done = true;
				}
				else if (fastTransferBlock.State == FastTransferState.Error)
				{
					this.error = true;
				}
				this.GetCacheInfo(out residualCacheSize, out doneInCache);
				result = fastTransferBlock;
			}
			finally
			{
				base.UnlockStore();
			}
			return result;
		}

		protected abstract int GetBlocks(out SafeExLinkedMemoryHandle ppBlocks, out int cBlocks);

		private void ReadMoreBlocks()
		{
			SafeExLinkedMemoryHandle safeExLinkedMemoryHandle = null;
			int cBlocks = 0;
			try
			{
				int blocks = this.GetBlocks(out safeExLinkedMemoryHandle, out cBlocks);
				if (blocks != 0 && blocks != 264224)
				{
					base.ThrowIfError("Synchronization failure.", blocks);
				}
				if (blocks == 0)
				{
					this.done = true;
				}
				FastTransferBlock[] array = safeExLinkedMemoryHandle.ReadFastTransferBlockArray(cBlocks);
				foreach (FastTransferBlock item in array)
				{
					this.dataBlocks.Enqueue(item);
				}
			}
			finally
			{
				if (safeExLinkedMemoryHandle != null)
				{
					safeExLinkedMemoryHandle.Dispose();
				}
			}
		}

		private bool TryGetDataBlock(out FastTransferBlock fastTransferBlock)
		{
			if (this.dataBlocks.Count > 0)
			{
				fastTransferBlock = this.dataBlocks.Dequeue();
				return true;
			}
			fastTransferBlock = FastTransferBlock.Partial;
			return false;
		}

		private void GetCacheInfo(out int cacheSize, out bool doneInCache)
		{
			cacheSize = 0;
			doneInCache = false;
			if (this.dataBlocks.Count > 0)
			{
				using (Queue<FastTransferBlock>.Enumerator enumerator = this.dataBlocks.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						FastTransferBlock fastTransferBlock = enumerator.Current;
						if (fastTransferBlock.Buffer != null)
						{
							cacheSize += fastTransferBlock.Buffer.Length;
						}
						if (fastTransferBlock.State == FastTransferState.Done || fastTransferBlock.State == FastTransferState.Error)
						{
							doneInCache = true;
						}
					}
					return;
				}
			}
			cacheSize = 0;
			doneInCache = (this.done || this.error);
		}

		private readonly Queue<FastTransferBlock> dataBlocks;

		private bool done;

		private bool error;
	}
}
