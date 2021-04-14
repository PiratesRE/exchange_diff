using System;

namespace Microsoft.Exchange.UM.UMCore
{
	internal interface IPipelineStageFactory
	{
		PipelineStageBase CreateStage(PipelineWorkItem workItem);
	}
}
