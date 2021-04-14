using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.FastTransfer.Parser
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal static class FastTransferAttachmentCopyTo
	{
		internal static IFastTransferProcessor<FastTransferDownloadContext> CreateDownloadStateMachine(IAttachment attachment)
		{
			return new FastTransferAttachmentContent(attachment, true);
		}

		internal static IFastTransferProcessor<FastTransferUploadContext> CreateUploadStateMachine(IAttachment attachment)
		{
			IFastTransferProcessor<FastTransferUploadContext> result;
			using (DisposeGuard disposeGuard = default(DisposeGuard))
			{
				FastTransferAttachmentContent fastTransferAttachmentContent = new FastTransferAttachmentContent(attachment, true);
				disposeGuard.Add<FastTransferAttachmentContent>(fastTransferAttachmentContent);
				FastTransferSkipDnPrefix fastTransferSkipDnPrefix = new FastTransferSkipDnPrefix(fastTransferAttachmentContent);
				disposeGuard.Add<FastTransferSkipDnPrefix>(fastTransferSkipDnPrefix);
				disposeGuard.Success();
				result = fastTransferSkipDnPrefix;
			}
			return result;
		}
	}
}
