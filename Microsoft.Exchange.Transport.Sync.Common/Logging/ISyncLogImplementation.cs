using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Transport.Sync.Common.Logging
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal interface ISyncLogImplementation
	{
		void Configure(bool enabled, string path, long ageQuota, long directorySizeQuota, long perFileSizeQuota);

		bool IsEnabled();

		void Append(LogRowFormatter row, int timestampField);

		void Close();
	}
}
