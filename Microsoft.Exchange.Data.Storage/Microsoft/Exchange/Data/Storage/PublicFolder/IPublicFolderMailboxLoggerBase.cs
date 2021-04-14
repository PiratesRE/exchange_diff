using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.PublicFolder
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IPublicFolderMailboxLoggerBase
	{
		void LogEvent(LogEventType eventType, string data);
	}
}
