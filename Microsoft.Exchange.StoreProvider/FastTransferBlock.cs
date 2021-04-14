using System;
using System.Runtime.InteropServices;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Mapi.Unmanaged;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal struct FastTransferBlock
	{
		internal unsafe FastTransferBlock(FxBlock* pBlock)
		{
			this.Buffer = Array<byte>.New(pBlock->bufferSize);
			if (pBlock->bufferSize > 0)
			{
				Marshal.Copy(pBlock->buffer, this.Buffer, 0, pBlock->bufferSize);
			}
			this.Steps = pBlock->steps;
			this.Progress = pBlock->progress;
			this.State = (FastTransferState)pBlock->state;
		}

		private FastTransferBlock(FastTransferState state)
		{
			this.Buffer = Array<byte>.Empty;
			this.Steps = 0U;
			this.Progress = 0U;
			this.State = state;
		}

		public static readonly FastTransferBlock Done = new FastTransferBlock(FastTransferState.Done);

		public static readonly FastTransferBlock Error = new FastTransferBlock(FastTransferState.Error);

		public static readonly FastTransferBlock Partial = new FastTransferBlock(FastTransferState.Partial);

		public readonly byte[] Buffer;

		public readonly uint Steps;

		public readonly uint Progress;

		public readonly FastTransferState State;
	}
}
