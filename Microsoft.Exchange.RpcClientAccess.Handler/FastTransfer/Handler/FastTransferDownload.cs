using System;
using Microsoft.Exchange.RpcClientAccess.FastTransfer.Parser;
using Microsoft.Exchange.RpcClientAccess.Handler;

namespace Microsoft.Exchange.RpcClientAccess.FastTransfer.Handler
{
	internal class FastTransferDownload : FastTransferServerObject<FastTransferDownloadContext>
	{
		protected FastTransferDownload(FastTransferDownloadContext context, Logon logon) : base(context, logon)
		{
		}

		public FastTransferDownload(FastTransferSendOption sendOptions, IFastTransferProcessor<FastTransferDownloadContext> fastTransferObject, uint steps, IPropertyFilterFactory propertyFilterFactory, Logon logon) : this(FastTransferDownloadContext.CreateForDownload(sendOptions, steps, logon.String8Encoding, logon.ResourceTracker, propertyFilterFactory, false), logon)
		{
			base.Context.PushInitial(fastTransferObject);
		}

		public uint Progress
		{
			get
			{
				base.CheckDisposed();
				return base.Context.Progress;
			}
		}

		public uint Steps
		{
			get
			{
				base.CheckDisposed();
				return base.Context.Steps;
			}
		}

		public bool IsMovingMailbox
		{
			get
			{
				base.CheckDisposed();
				return base.Context.IsMovingMailbox;
			}
		}

		public FastTransferState State
		{
			get
			{
				base.CheckDisposed();
				return base.Context.State;
			}
		}

		public int GetNextBuffer(ArraySegment<byte> buffer)
		{
			base.CheckDisposed();
			return this.InternalGetNextBuffer(buffer);
		}

		protected virtual int InternalGetNextBuffer(ArraySegment<byte> buffer)
		{
			return base.Context.GetNextBuffer(buffer);
		}
	}
}
