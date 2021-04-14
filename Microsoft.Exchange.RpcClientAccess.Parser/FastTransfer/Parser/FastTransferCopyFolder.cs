using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.FastTransfer.Parser
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal static class FastTransferCopyFolder
	{
		internal static IFastTransferProcessor<FastTransferDownloadContext> CreateDownloadStateMachine(IFolder folder, FastTransferFolderContentBase.IncludeSubObject includeSubObject)
		{
			IFastTransferProcessor<FastTransferDownloadContext> result;
			using (DisposeGuard disposeGuard = default(DisposeGuard))
			{
				FastTransferFolderContent fastTransferFolderContent = new FastTransferFolderContent(folder, includeSubObject, true);
				disposeGuard.Add<FastTransferFolderContent>(fastTransferFolderContent);
				FastTransferDownloadDelimitedObject fastTransferDownloadDelimitedObject = new FastTransferDownloadDelimitedObject(fastTransferFolderContent, PropertyTag.StartTopFld, PropertyTag.EndFolder);
				disposeGuard.Add<FastTransferDownloadDelimitedObject>(fastTransferDownloadDelimitedObject);
				disposeGuard.Success();
				result = fastTransferDownloadDelimitedObject;
			}
			return result;
		}

		internal static IFastTransferProcessor<FastTransferUploadContext> CreateUploadStateMachine(IFolder folder)
		{
			IFastTransferProcessor<FastTransferUploadContext> result;
			using (DisposeGuard disposeGuard = default(DisposeGuard))
			{
				FastTransferFolderContent fastTransferFolderContent = new FastTransferFolderContent(folder, FastTransferFolderContentBase.IncludeSubObject.All, true);
				disposeGuard.Add<FastTransferFolderContent>(fastTransferFolderContent);
				FastTransferUploadDelimitedObject fastTransferUploadDelimitedObject = new FastTransferUploadDelimitedObject(fastTransferFolderContent, PropertyTag.StartTopFld, PropertyTag.EndFolder);
				disposeGuard.Add<FastTransferUploadDelimitedObject>(fastTransferUploadDelimitedObject);
				FastTransferSkipDnPrefix fastTransferSkipDnPrefix = new FastTransferSkipDnPrefix(fastTransferUploadDelimitedObject);
				disposeGuard.Add<FastTransferSkipDnPrefix>(fastTransferSkipDnPrefix);
				disposeGuard.Success();
				result = fastTransferSkipDnPrefix;
			}
			return result;
		}
	}
}
