using System;

namespace Microsoft.Exchange.Data.Storage.LinkedFolder
{
	internal enum OperationType
	{
		EndToEnd,
		SharePointQuery,
		AddFolder,
		UpdateFolder,
		AddFile,
		UpdateFile,
		MoveFile,
		DeleteItem,
		FolderLookupById,
		FolderLookupByUri,
		FileLookupById,
		Throttle
	}
}
