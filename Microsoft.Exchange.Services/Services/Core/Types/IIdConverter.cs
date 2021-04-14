using System;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal interface IIdConverter
	{
		IdAndSession ConvertFolderIdToIdAndSessionReadOnly(BaseFolderId folderId);
	}
}
