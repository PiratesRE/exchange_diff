using System;
using System.Text;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ManagedStore.FastTransfer;
using Microsoft.Exchange.Protocols.MAPI;
using Microsoft.Exchange.RpcClientAccess;
using Microsoft.Exchange.RpcClientAccess.FastTransfer.Parser;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.PropTags;

namespace Microsoft.Exchange.Protocols.FastTransfer
{
	internal abstract class IcsDownloadContext : FastTransferDownloadContext
	{
		public IcsDownloadContext() : base(Array<PropertyTag>.Empty)
		{
		}

		public ErrorCode Configure(MapiLogon logon, FastTransferSendOption sendOptions)
		{
			ErrorCode errorCode = base.Configure(logon, sendOptions, null);
			if (errorCode == ErrorCode.NoError)
			{
				this.icsState = new IcsState();
			}
			return errorCode;
		}

		public IcsState IcsState
		{
			get
			{
				return this.icsState;
			}
		}

		public ErrorCode BeginUploadStateProperty(StorePropTag propTag, uint size)
		{
			base.ThrowIfNotValid(null);
			if (base.DownloadStarted)
			{
				throw new ExExceptionNoSupport((LID)46136U, "Uploading a state after download was started.");
			}
			return this.icsState.BeginUploadStateProperty(propTag, size);
		}

		public ErrorCode ContinueUploadStateProperty(ArraySegment<byte> data)
		{
			base.ThrowIfNotValid(null);
			return this.icsState.ContinueUploadStateProperty(data);
		}

		public ErrorCode EndUploadStateProperty()
		{
			base.ThrowIfNotValid(null);
			return this.icsState.EndUploadStateProperty();
		}

		public ErrorCode OpenIcsStateDownloadContext(out FastTransferDownloadContext outputContext)
		{
			base.ThrowIfNotValid(null);
			if (ExTraceGlobals.IcsDownloadStateTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				StringBuilder stringBuilder = new StringBuilder(100);
				stringBuilder.Append("CheckpointState=[");
				stringBuilder.Append(this.icsState.ToString());
				stringBuilder.Append("]");
				ExTraceGlobals.IcsDownloadStateTracer.TraceDebug(0L, stringBuilder.ToString());
			}
			return this.icsState.OpenIcsStateDownloadContext(base.Logon, out outputContext);
		}

		protected override FastTransferDownloadContext CreateFastTransferDownloadContext()
		{
			if (this.icsState.StateUploadStarted)
			{
				throw new ExExceptionNoSupport((LID)62520U, "Should properly end the state property upload.");
			}
			return FastTransferDownloadContext.CreateForIcs(base.SendOptions, CTSGlobals.AsciiEncoding, NullResourceTracker.Instance, new PropertyFilterFactory(this.ExcludedPropertyTags), base.Logon.IsMoveUser);
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<IcsDownloadContext>(this);
		}

		protected override void InternalDispose(bool calledFromDispose)
		{
			if (calledFromDispose && this.icsState != null)
			{
				this.icsState.Dispose();
				this.icsState = null;
			}
			base.InternalDispose(calledFromDispose);
		}

		private IcsState icsState;
	}
}
