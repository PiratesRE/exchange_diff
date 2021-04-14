using System;

namespace Microsoft.Exchange.UM.UMCore
{
	internal delegate void StageCompletionCallback(PipelineStageBase sender, PipelineWorkItem workitem, Exception error);
}
