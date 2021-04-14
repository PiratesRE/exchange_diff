using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Protocols.MAPI;
using Microsoft.Exchange.RpcClientAccess;
using Microsoft.Exchange.RpcClientAccess.FastTransfer;
using Microsoft.Exchange.RpcClientAccess.FastTransfer.Parser;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.StoreCommonServices;

namespace Microsoft.Exchange.Protocols.FastTransfer
{
	internal class FastTransferDownloadContext : FastTransferContext
	{
		public FastTransferDownloadContext(ICollection<PropertyTag> excludedPropertyTags)
		{
			this.ExcludedPropertyTags = excludedPropertyTags;
		}

		public ErrorCode Configure(MapiLogon logon, FastTransferSendOption sendOptions, Func<MapiContext, IFastTransferProcessor<FastTransferDownloadContext>> getProcessorDelegate)
		{
			ErrorCode errorCode = base.Configure(logon);
			if (errorCode == ErrorCode.NoError)
			{
				this.sendOptions = sendOptions;
				this.getProcessorDelegate = getProcessorDelegate;
			}
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

		public FastTransferSendOption SendOptions
		{
			get
			{
				return this.sendOptions;
			}
		}

		public bool DownloadStarted
		{
			get
			{
				return this.context != null;
			}
		}

		public int GetNextBuffer(MapiContext operationContext, ArraySegment<byte> buffer)
		{
			base.ThrowIfNotValid(null);
			if (this.context == null)
			{
				using (DisposeGuard disposeGuard = default(DisposeGuard))
				{
					FastTransferDownloadContext fastTransferDownloadContext = disposeGuard.Add<FastTransferDownloadContext>(this.CreateFastTransferDownloadContext());
					IFastTransferProcessor<FastTransferDownloadContext> fastTransferObject = disposeGuard.Add<IFastTransferProcessor<FastTransferDownloadContext>>(this.GetFastTransferProcessor(operationContext));
					fastTransferDownloadContext.PushInitial(fastTransferObject);
					disposeGuard.Success();
					this.context = fastTransferDownloadContext;
				}
			}
			return this.context.GetNextBuffer(buffer);
		}

		public virtual IChunked PrepareIndexes(MapiContext operationContext)
		{
			return null;
		}

		protected virtual FastTransferDownloadContext CreateFastTransferDownloadContext()
		{
			return FastTransferDownloadContext.CreateForDownload(this.sendOptions, 1U, CTSGlobals.AsciiEncoding, NullResourceTracker.Instance, new PropertyFilterFactory(this.ExcludedPropertyTags), base.Logon.IsMoveUser);
		}

		protected virtual IFastTransferProcessor<FastTransferDownloadContext> GetFastTransferProcessor(MapiContext operationContext)
		{
			return this.getProcessorDelegate(operationContext);
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<FastTransferDownloadContext>(this);
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

		private FastTransferSendOption sendOptions;

		private Func<MapiContext, IFastTransferProcessor<FastTransferDownloadContext>> getProcessorDelegate;

		private FastTransferDownloadContext context;

		protected readonly ICollection<PropertyTag> ExcludedPropertyTags;
	}
}
