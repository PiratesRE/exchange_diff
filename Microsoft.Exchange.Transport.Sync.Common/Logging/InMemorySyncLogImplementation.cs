using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Transport.Sync.Common.Logging
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class InMemorySyncLogImplementation : ISyncLogImplementation
	{
		public void Configure(bool enabled, string path, long ageQuota, long directorySizeQuota, long perFileSizeQuota)
		{
			throw new InvalidOperationException("The in memory sync log cannot be configured.");
		}

		public bool IsEnabled()
		{
			return false;
		}

		public void Append(LogRowFormatter row, int timestampField)
		{
		}

		public void Close()
		{
		}
	}
}
