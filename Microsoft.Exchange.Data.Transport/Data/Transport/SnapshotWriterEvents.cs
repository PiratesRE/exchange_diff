using System;

namespace Microsoft.Exchange.Data.Transport
{
	[Flags]
	internal enum SnapshotWriterEvents
	{
		FolderCreated = 1,
		OriginalWritten = 2
	}
}
