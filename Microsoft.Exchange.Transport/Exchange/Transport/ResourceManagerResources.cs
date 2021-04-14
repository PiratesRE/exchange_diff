using System;

namespace Microsoft.Exchange.Transport
{
	[Flags]
	internal enum ResourceManagerResources
	{
		None = 0,
		MailDatabase = 1,
		MailDatabaseLoggingFolder = 2,
		VersionBuckets = 4,
		PrivateBytes = 8,
		TotalBytes = 16,
		SubmissionQueue = 64,
		TempDrive = 128,
		All = 255
	}
}
