using System;

namespace Microsoft.Exchange.Search.Core.Abstraction
{
	internal interface IPipelineComponentFactory
	{
		IPipelineComponent CreateInstance(IPipelineComponentConfig config, IPipelineContext context);

		IStartStopPipelineComponent CreateInstance(IPipelineComponentConfig config, IPipelineContext context, IPipeline nestedPipeline);
	}
}
