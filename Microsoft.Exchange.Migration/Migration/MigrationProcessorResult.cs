using System;

namespace Microsoft.Exchange.Migration
{
	internal enum MigrationProcessorResult
	{
		Working,
		Waiting,
		Completed,
		Failed,
		Deleted,
		Suspended
	}
}
