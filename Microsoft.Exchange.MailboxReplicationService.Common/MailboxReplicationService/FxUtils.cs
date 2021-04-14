using System;
using System.Linq;
using System.Text;
using Microsoft.Exchange.RpcClientAccess;
using Microsoft.Exchange.RpcClientAccess.FastTransfer;
using Microsoft.Exchange.RpcClientAccess.FastTransfer.Handler;
using Microsoft.Exchange.RpcClientAccess.FastTransfer.Parser;
using Microsoft.Mapi;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal static class FxUtils
	{
		public static void TransferFxBuffers(FastTransferDownloadContext downloadContext, FxCollectorSerializer collectorSerializer)
		{
			ArraySegment<byte> buffer = new ArraySegment<byte>(new byte[31680]);
			for (int i = downloadContext.GetNextBuffer(buffer); i > 0; i = downloadContext.GetNextBuffer(buffer))
			{
				if (i < buffer.Array.Length)
				{
					byte[] array = new byte[i];
					Array.Copy(buffer.Array, 0, array, 0, i);
					collectorSerializer.TransferBuffer(array);
				}
				else
				{
					collectorSerializer.TransferBuffer(buffer.Array);
				}
				buffer = new ArraySegment<byte>(new byte[31680]);
			}
		}

		public static void CopyItem(MessageRec messageRec, IMessage message, IFolderProxy folderProxy, PropTag[] propsToExclude)
		{
			using (IMessageProxy messageProxy = folderProxy.OpenMessage(messageRec.EntryId))
			{
				FxCollectorSerializer fxCollectorSerializer = new FxCollectorSerializer(messageProxy);
				fxCollectorSerializer.Config(0, 1);
				using (FastTransferDownloadContext fastTransferDownloadContext = FastTransferDownloadContext.CreateForDownload(FastTransferSendOption.Unicode | FastTransferSendOption.UseCpId | FastTransferSendOption.ForceUnicode, 1U, Encoding.Default, NullResourceTracker.Instance, new PropertyFilterFactory(false, false, (from ptag in propsToExclude
				select new PropertyTag((uint)ptag)).ToArray<PropertyTag>()), false))
				{
					FastTransferMessageCopyTo fastTransferObject = new FastTransferMessageCopyTo(false, message, true);
					fastTransferDownloadContext.PushInitial(fastTransferObject);
					FxUtils.TransferFxBuffers(fastTransferDownloadContext, fxCollectorSerializer);
					messageProxy.SaveChanges();
				}
			}
		}

		private const int OutlookMaxFxBufferSize = 31680;
	}
}
