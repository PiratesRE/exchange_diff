using System;

namespace Microsoft.Exchange.AnchorService
{
	internal enum AnchorJobProcessorResult
	{
		Working,
		Waiting,
		Completed,
		Failed,
		Deleted
	}
}
