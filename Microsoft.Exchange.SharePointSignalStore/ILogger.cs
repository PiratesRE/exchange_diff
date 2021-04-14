using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.SharePointSignalStore
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal interface ILogger
	{
		void LogWarning(string format, params object[] args);

		void LogInfo(string format, params object[] args);
	}
}
