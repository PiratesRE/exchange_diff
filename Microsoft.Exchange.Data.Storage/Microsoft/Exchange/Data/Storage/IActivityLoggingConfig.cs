using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IActivityLoggingConfig
	{
		TimeSpan MaxLogFileAge { get; }

		ByteQuantifiedSize MaxLogDirectorySize { get; }

		ByteQuantifiedSize MaxLogFileSize { get; }

		bool IsDumpCollectionEnabled { get; }
	}
}
