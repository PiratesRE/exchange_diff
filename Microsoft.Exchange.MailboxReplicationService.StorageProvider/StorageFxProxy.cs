using System;
using System.Text;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.RpcClientAccess;
using Microsoft.Exchange.RpcClientAccess.FastTransfer;
using Microsoft.Exchange.RpcClientAccess.FastTransfer.Handler;
using Microsoft.Exchange.RpcClientAccess.FastTransfer.Parser;
using Microsoft.Mapi;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal abstract class StorageFxProxy<T> : DisposeTrackableBase, IFxProxy, IMapiFxProxy, IDisposeTrackable, IDisposable
	{
		protected bool IsMoveUser { get; set; }

		protected T TargetObject { get; set; }

		public static byte[] CreateObjectData(Guid objectType)
		{
			byte[] exchangeVersionBlob = new byte[]
			{
				(byte)VersionInformation.MRS.ProductMinor,
				(byte)VersionInformation.MRS.ProductMajor,
				(byte)(VersionInformation.MRS.BuildMajor % 256),
				(byte)(VersionInformation.MRS.BuildMajor / 256 + 128),
				(byte)(VersionInformation.MRS.BuildMinor % 256),
				(byte)(VersionInformation.MRS.BuildMinor / 256)
			};
			return BinarySerializer.Serialize(delegate(BinarySerializer serializer)
			{
				serializer.Write(objectType);
				serializer.Write(exchangeVersionBlob);
			});
		}

		byte[] IMapiFxProxy.GetObjectData()
		{
			return this.GetObjectDataImplementation();
		}

		void IMapiFxProxy.ProcessRequest(FxOpcodes opCode, byte[] data)
		{
			switch (opCode)
			{
			case FxOpcodes.Config:
			{
				if (data == null || data.Length != 8)
				{
					throw new FastTransferBufferException("data", (data == null) ? -1 : data.Length);
				}
				uint transferMethod = BitConverter.ToUInt32(data, 4);
				IFastTransferProcessor<FastTransferUploadContext> fxProcessor = this.GetFxProcessor(transferMethod);
				this.uploadContext = new FastTransferUploadContext(Encoding.ASCII, NullResourceTracker.Instance, PropertyFilterFactory.IncludeAllFactory, this.IsMoveUser);
				this.uploadContext.PushInitial(fxProcessor);
				return;
			}
			case FxOpcodes.TransferBuffer:
				ExAssert.RetailAssert(this.uploadContext != null, "StorageFxProxy:ProcessRequest: null upload context");
				try
				{
					this.uploadContext.PutNextBuffer(new ArraySegment<byte>(data));
					return;
				}
				catch (ArgumentException innerException)
				{
					throw new FastTransferArgumentException(innerException);
				}
				break;
			case FxOpcodes.IsInterfaceOk:
			case FxOpcodes.TellPartnerVersion:
				return;
			}
			throw new FastTransferBufferException("opCode", (int)opCode);
		}

		void IFxProxy.Flush()
		{
		}

		protected abstract byte[] GetObjectDataImplementation();

		protected abstract IFastTransferProcessor<FastTransferUploadContext> GetFxProcessor(uint transferMethod);

		protected override void InternalDispose(bool calledFromDispose)
		{
			if (calledFromDispose && this.uploadContext != null)
			{
				this.uploadContext.Dispose();
				this.uploadContext = null;
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<StorageFxProxy<T>>(this);
		}

		protected const uint TransferMethodCopyTo = 1U;

		protected const uint TransferMethodCopyMessages = 3U;

		private FastTransferUploadContext uploadContext;
	}
}
