using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.FastTransfer.Parser
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal static class FastTransferFolderCopyTo
	{
		internal static IFastTransferProcessor<FastTransferDownloadContext> CreateDownloadStateMachine(IFolder folder, FastTransferFolderContentBase.IncludeSubObject includeSubObject)
		{
			return new FastTransferFolderContentWithDelProp(folder, includeSubObject);
		}

		internal static IFastTransferProcessor<FastTransferUploadContext> CreateUploadStateMachine(IFolder folder)
		{
			IFastTransferProcessor<FastTransferUploadContext> result;
			using (DisposeGuard disposeGuard = default(DisposeGuard))
			{
				FastTransferFolderContentWithDelProp fastTransferFolderContentWithDelProp = new FastTransferFolderContentWithDelProp(folder);
				disposeGuard.Add<FastTransferFolderContentWithDelProp>(fastTransferFolderContentWithDelProp);
				FastTransferSkipDnPrefix fastTransferSkipDnPrefix = new FastTransferSkipDnPrefix(fastTransferFolderContentWithDelProp);
				disposeGuard.Add<FastTransferSkipDnPrefix>(fastTransferSkipDnPrefix);
				disposeGuard.Success();
				result = fastTransferSkipDnPrefix;
			}
			return result;
		}
	}
}
