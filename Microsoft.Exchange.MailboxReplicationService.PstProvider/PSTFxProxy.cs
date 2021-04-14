using System;
using System.Text;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.PST;
using Microsoft.Exchange.RpcClientAccess;
using Microsoft.Exchange.RpcClientAccess.FastTransfer;
using Microsoft.Exchange.RpcClientAccess.FastTransfer.Handler;
using Microsoft.Exchange.RpcClientAccess.FastTransfer.Parser;
using Microsoft.Mapi;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal class PSTFxProxy : DisposeTrackableBase, IFxProxy, IMapiFxProxy, IDisposeTrackable, IDisposable
	{
		public PSTFxProxy(object targetObject)
		{
			this.targetObject = targetObject;
			this.targetObjectData = null;
			this.pstMailbox = ((targetObject is PstFxFolder) ? ((PstFxFolder)targetObject).PstMailbox : ((PSTMessage)targetObject).PstMailbox);
		}

		byte[] IMapiFxProxy.GetObjectData()
		{
			if (this.targetObjectData == null)
			{
				this.targetObjectData = MapiUtils.CreateObjectData((this.targetObject is PstFxFolder) ? InterfaceIds.IMAPIFolderGuid : InterfaceIds.IMessageGuid);
			}
			return this.targetObjectData;
		}

		void IMapiFxProxy.ProcessRequest(FxOpcodes opCode, byte[] data)
		{
			switch (opCode)
			{
			case FxOpcodes.Config:
				this.folderProcessor = FastTransferFolderCopyTo.CreateUploadStateMachine((PstFxFolder)this.targetObject);
				this.uploadContext = new FastTransferUploadContext(Encoding.ASCII, NullResourceTracker.Instance, PropertyFilterFactory.IncludeAllFactory, false);
				this.uploadContext.PushInitial(this.folderProcessor);
				return;
			case FxOpcodes.TransferBuffer:
				try
				{
					this.uploadContext.PutNextBuffer(new ArraySegment<byte>(data));
					return;
				}
				catch (PSTExceptionBase innerException)
				{
					throw new MailboxReplicationPermanentException(new LocalizedString("TransferBuffer"), innerException);
				}
				break;
			case FxOpcodes.IsInterfaceOk:
			case FxOpcodes.TellPartnerVersion:
				return;
			}
			throw new NotSupportedException();
		}

		void IFxProxy.Flush()
		{
			try
			{
				((PstFxFolder)this.targetObject).IPstFolder.Save();
			}
			catch (PSTExceptionBase innerException)
			{
				throw new MailboxReplicationPermanentException(new LocalizedString("TransferBuffer"), innerException);
			}
		}

		protected override void InternalDispose(bool calledFromDispose)
		{
			if (calledFromDispose && this.uploadContext != null)
			{
				this.folderProcessor.Dispose();
				this.uploadContext.Dispose();
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<PSTFxProxy>(this);
		}

		private IFastTransferProcessor<FastTransferUploadContext> folderProcessor;

		private FastTransferUploadContext uploadContext;

		private object targetObject;

		private byte[] targetObjectData;

		private PstMailbox pstMailbox;
	}
}
