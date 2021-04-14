using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Protocols.MAPI;
using Microsoft.Exchange.RpcClientAccess;
using Microsoft.Exchange.RpcClientAccess.FastTransfer;
using Microsoft.Exchange.RpcClientAccess.FastTransfer.Parser;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.LogicalDataModel;

namespace Microsoft.Exchange.Protocols.FastTransfer
{
	internal class FastTransferUploadContext : FastTransferContext
	{
		public ErrorCode Configure(MapiLogon logon, Func<MapiContext, IFastTransferProcessor<FastTransferUploadContext>> getProcessorDelegate, Func<MapiContext, bool> flushDelegate, Folder folderForQuotaCheck)
		{
			ErrorCode errorCode = base.Configure(logon);
			if (errorCode == ErrorCode.NoError)
			{
				this.getProcessorDelegate = getProcessorDelegate;
				this.flushDelegate = flushDelegate;
			}
			this.folderForQuotaCheck = folderForQuotaCheck;
			return errorCode;
		}

		public bool IsMovingMailbox
		{
			get
			{
				return base.Logon.IsMoveUser;
			}
		}

		public FastTransferState State
		{
			get
			{
				if (this.context != null)
				{
					return this.context.State;
				}
				return FastTransferState.Error;
			}
		}

		public bool UploadStarted
		{
			get
			{
				return this.context != null;
			}
		}

		public Folder FolderForQuotaCheck
		{
			get
			{
				return this.folderForQuotaCheck;
			}
		}

		public void PutNextBuffer(MapiContext operationContext, ArraySegment<byte> buffer)
		{
			base.ThrowIfNotValid(null);
			if (this.context == null)
			{
				using (DisposeGuard disposeGuard = default(DisposeGuard))
				{
					FastTransferUploadContext fastTransferUploadContext = disposeGuard.Add<FastTransferUploadContext>(new FastTransferUploadContext(CTSGlobals.AsciiEncoding, NullResourceTracker.Instance, PropertyFilterFactory.IncludeAllFactory, base.Logon.IsMoveUser));
					IFastTransferProcessor<FastTransferUploadContext> fastTransferObject = disposeGuard.Add<IFastTransferProcessor<FastTransferUploadContext>>(this.GetFastTransferProcessor(operationContext));
					fastTransferUploadContext.PushInitial(fastTransferObject);
					disposeGuard.Success();
					this.context = fastTransferUploadContext;
				}
			}
			this.context.PutNextBuffer(buffer);
		}

		public void Flush(MapiContext operationContext)
		{
			if (this.flushDelegate != null)
			{
				this.flushDelegate(operationContext);
			}
		}

		protected virtual IFastTransferProcessor<FastTransferUploadContext> GetFastTransferProcessor(MapiContext operationContext)
		{
			return this.getProcessorDelegate(operationContext);
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<FastTransferUploadContext>(this);
		}

		protected override void InternalDispose(bool calledFromDispose)
		{
			if (calledFromDispose && this.context != null)
			{
				this.context.Dispose();
				this.context = null;
			}
			base.InternalDispose(calledFromDispose);
		}

		private Func<MapiContext, IFastTransferProcessor<FastTransferUploadContext>> getProcessorDelegate;

		private Func<MapiContext, bool> flushDelegate;

		private FastTransferUploadContext context;

		private Folder folderForQuotaCheck;
	}
}
