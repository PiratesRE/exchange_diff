using System;
using Microsoft.Exchange.RpcClientAccess.FastTransfer;
using Microsoft.Exchange.RpcClientAccess.FastTransfer.Handler;
using Microsoft.Exchange.RpcClientAccess.FastTransfer.Parser;
using Microsoft.Mapi;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal class StorageMessageProxy : StorageFxProxy<MessageAdaptor>
	{
		public StorageMessageProxy(MessageAdaptor message, bool isMoveUser)
		{
			base.IsMoveUser = isMoveUser;
			base.TargetObject = message;
		}

		protected override byte[] GetObjectDataImplementation()
		{
			return StorageMessageProxy.ObjectData;
		}

		protected override IFastTransferProcessor<FastTransferUploadContext> GetFxProcessor(uint transferMethod)
		{
			if (transferMethod == 1U)
			{
				return new FastTransferMessageCopyTo(false, base.TargetObject, true);
			}
			throw new FastTransferBufferException("transferMethod", (int)transferMethod);
		}

		public static readonly byte[] ObjectData = StorageFxProxy<MessageAdaptor>.CreateObjectData(InterfaceIds.IMessageGuid);
	}
}
