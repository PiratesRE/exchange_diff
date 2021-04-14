using System;
using System.Text;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ManagedStore.FastTransfer;
using Microsoft.Exchange.Protocols.MAPI;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.PropTags;
using Microsoft.Exchange.Server.Storage.StoreCommonServices;

namespace Microsoft.Exchange.Protocols.FastTransfer
{
	internal abstract class IcsUploadContext : MapiBase
	{
		public IcsUploadContext() : base(MapiObjectType.IcsUploadContext)
		{
		}

		public IcsState IcsState
		{
			get
			{
				return this.icsState;
			}
		}

		public static bool ValidChangeKey(byte[] changeKey)
		{
			return IcsUploadContext.ValidForeignOrInternalId(changeKey);
		}

		public static bool ValidSourceKey(byte[] sourceKey)
		{
			return IcsUploadContext.ValidForeignOrInternalId(sourceKey);
		}

		public static bool ValidForeignOrInternalId(byte[] id)
		{
			return id != null && id.Length > 16 && id.Length < 256;
		}

		public virtual ErrorCode Configure(MapiContext operationContext, MapiFolder folder)
		{
			base.Logon = folder.Logon;
			this.icsState = new IcsState();
			base.IsValid = true;
			return ErrorCode.NoError;
		}

		public ErrorCode BeginUploadStateProperty(StorePropTag propTag, uint size)
		{
			base.ThrowIfNotValid(null);
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

		public abstract ErrorCode ImportDelete(MapiContext operationContext, byte[][] sourceKeys);

		public ErrorCode OpenIcsStateDownloadContext(out FastTransferDownloadContext outputContext)
		{
			base.ThrowIfNotValid(null);
			this.icsState.ReloadIfNecessary();
			if (ExTraceGlobals.IcsUploadStateTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				StringBuilder stringBuilder = new StringBuilder(100);
				stringBuilder.Append("CheckpointState=[");
				stringBuilder.Append(this.icsState.ToString());
				stringBuilder.Append("]");
				ExTraceGlobals.IcsUploadStateTracer.TraceDebug(0L, stringBuilder.ToString());
			}
			return this.icsState.OpenIcsStateDownloadContext(base.Logon, out outputContext);
		}

		protected void TraceInitialState(MapiContext operationContext)
		{
			if (!this.initialStateTraced)
			{
				this.initialStateTraced = true;
				this.icsState.ReloadIfNecessary();
				if (ExTraceGlobals.IcsUploadStateTracer.IsTraceEnabled(TraceType.DebugTrace))
				{
					StringBuilder stringBuilder = new StringBuilder(100);
					stringBuilder.Append("InitialState=[");
					stringBuilder.Append(this.icsState.ToString());
					stringBuilder.Append("]");
					ExTraceGlobals.IcsUploadStateTracer.TraceDebug(0L, stringBuilder.ToString());
				}
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<IcsUploadContext>(this);
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

		protected ExchangeId MessageIdFromSourceKey(MapiContext operationContext, ExchangeId folderId, ref byte[] sourceKey)
		{
			if (IcsUploadContext.IsValid22ByteId(sourceKey))
			{
				byte[] bytes = sourceKey;
				sourceKey = null;
				ExchangeId result = ExchangeId.CreateFrom22ByteArray(operationContext, base.Logon.StoreMailbox.ReplidGuidMap, bytes);
				if (!result.IsValid || result.Counter != 0UL)
				{
					return result;
				}
			}
			throw new StoreException((LID)54328U, ErrorCodeValue.InvalidParameter);
		}

		protected ExchangeId FolderIdFromSourceKey(MapiContext operationContext, ref byte[] sourceKey)
		{
			if (IcsUploadContext.IsValid22ByteId(sourceKey))
			{
				byte[] bytes = sourceKey;
				sourceKey = null;
				return ExchangeId.CreateFrom22ByteArray(operationContext, base.Logon.StoreMailbox.ReplidGuidMap, bytes);
			}
			throw new StoreException((LID)42040U, ErrorCodeValue.InvalidParameter);
		}

		private static bool IsValid22ByteId(byte[] sourceKey)
		{
			return sourceKey != null && sourceKey.Length == 22;
		}

		private IcsState icsState;

		private bool initialStateTraced;
	}
}
