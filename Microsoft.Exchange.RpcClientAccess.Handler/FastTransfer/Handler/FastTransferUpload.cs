using System;
using Microsoft.Exchange.RpcClientAccess.FastTransfer.Parser;
using Microsoft.Exchange.RpcClientAccess.Handler;

namespace Microsoft.Exchange.RpcClientAccess.FastTransfer.Handler
{
	internal class FastTransferUpload : FastTransferServerObject<FastTransferUploadContext>
	{
		protected FastTransferUpload(FastTransferUploadContext context, Logon logon) : base(context, logon)
		{
		}

		public FastTransferUpload(IFastTransferProcessor<FastTransferUploadContext> fastTransferObject, IPropertyFilterFactory propertyFilterFactory, Logon logon) : this(new FastTransferUploadContext(logon.String8Encoding, logon.ResourceTracker, propertyFilterFactory, logon.Session.IsMoveUser), logon)
		{
			base.Context.PushInitial(fastTransferObject);
		}

		public uint Progress
		{
			get
			{
				base.CheckDisposed();
				return 0U;
			}
		}

		public uint Steps
		{
			get
			{
				base.CheckDisposed();
				return 1U;
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

		public void PutNextBuffer(ArraySegment<byte> buffer)
		{
			base.CheckDisposed();
			this.InternalPutNextBuffer(buffer);
		}

		protected virtual void InternalPutNextBuffer(ArraySegment<byte> buffer)
		{
			base.Context.PutNextBuffer(buffer);
		}
	}
}
