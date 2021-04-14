using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.Auditing
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class UnifiedAuditLoggerSettings
	{
		internal string DirectoryPath { get; private set; }

		internal TimeSpan MaxAge { get; private set; }

		internal ByteQuantifiedSize MaxDirectorySize { get; private set; }

		internal ByteQuantifiedSize MaxFileSize { get; private set; }

		internal ByteQuantifiedSize CacheSize { get; private set; }

		internal TimeSpan FlushInterval { get; private set; }

		internal bool FlushToDisk { get; private set; }

		internal static UnifiedAuditLoggerSettings Load()
		{
			return new UnifiedAuditLoggerSettings
			{
				CacheSize = ByteQuantifiedSize.FromKB(128UL),
				DirectoryPath = "D:\\ComplianceAudit\\LocalQueue",
				FlushInterval = TimeSpan.FromMilliseconds(1000.0),
				FlushToDisk = true,
				MaxAge = TimeSpan.FromDays(3.0),
				MaxDirectorySize = ByteQuantifiedSize.FromMB(5000UL),
				MaxFileSize = ByteQuantifiedSize.FromMB(4UL)
			};
		}
	}
}
