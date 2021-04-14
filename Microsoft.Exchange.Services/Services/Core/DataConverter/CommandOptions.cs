using System;

namespace Microsoft.Exchange.Services.Core.DataConverter
{
	[Flags]
	internal enum CommandOptions
	{
		None = 0,
		ConvertParentFolderIdToPublicFolderId = 1,
		ConvertFolderIdToPublicFolderId = 2
	}
}
