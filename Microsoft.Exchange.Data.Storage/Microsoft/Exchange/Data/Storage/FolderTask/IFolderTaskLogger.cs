using System;
using Microsoft.Exchange.Data.Storage.PublicFolder;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.FolderTask
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IFolderTaskLogger : IPublicFolderMailboxLoggerBase
	{
	}
}
