using System;
using Microsoft.Exchange.RpcClientAccess.FastTransfer;
using Microsoft.Exchange.RpcClientAccess.FastTransfer.Handler;
using Microsoft.Exchange.RpcClientAccess.FastTransfer.Parser;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal class StorageFolderProxy : StorageFxProxy<StorageDestinationFolder>
	{
		public StorageFolderProxy(StorageDestinationFolder folder, bool isMoveUser)
		{
			base.IsMoveUser = isMoveUser;
			base.TargetObject = folder;
		}

		protected override byte[] GetObjectDataImplementation()
		{
			return MapiUtils.MapiFolderObjectData;
		}

		protected override IFastTransferProcessor<FastTransferUploadContext> GetFxProcessor(uint transferMethod)
		{
			IFastTransferProcessor<FastTransferUploadContext> result;
			if (transferMethod == 1U)
			{
				result = FastTransferFolderCopyTo.CreateUploadStateMachine(base.TargetObject.FxFolder);
			}
			else
			{
				if (transferMethod != 3U)
				{
					throw new FastTransferBufferException("transferMethod", (int)transferMethod);
				}
				result = new FastTransferMessageIterator(new MessageIteratorClient(base.TargetObject.CoreFolder), true);
			}
			return result;
		}
	}
}
