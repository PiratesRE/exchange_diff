using System;
using Microsoft.Exchange.Data.Storage.Principal;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal static class IFolderExtensions
	{
		public static void SaveWithUniqueDisplayName(this IFolder folder, int maxSuffix = 50)
		{
			IStoreSession session = folder.Session;
			IExchangePrincipal mailboxOwner = session.MailboxOwner;
			string displayName = folder.DisplayName;
			string text = displayName;
			int num = 1;
			try
			{
				IL_1A:
				FolderSaveResult folderSaveResult = folder.Save();
				if (folderSaveResult.OperationResult != OperationResult.Succeeded)
				{
					ExTraceGlobals.StorageTracer.TraceError<IExchangePrincipal, string, FolderSaveResult>(0L, "{0}: Failed to create folder {1} due to {2}.", mailboxOwner, text, folderSaveResult);
					throw folderSaveResult.ToException(ServerStrings.ExCannotCreateFolder(folderSaveResult.ToString()));
				}
			}
			catch (ObjectExistedException)
			{
				if (num > maxSuffix)
				{
					throw;
				}
				text = ServerStrings.SharingFolderNameWithSuffix(displayName, num++);
				folder.DisplayName = text;
				ExTraceGlobals.StorageTracer.TraceDebug<IExchangePrincipal, string>(0L, "{0}: Folder exists. Recalculated folder name: {1}.", mailboxOwner, text);
				goto IL_1A;
			}
		}
	}
}
