using System;

namespace Microsoft.Exchange.Configuration.Tasks
{
	internal enum TaskStage
	{
		NotStarted,
		BeginProcessing,
		ProcessRecord,
		EndProcessing
	}
}
