using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Search.Core.Abstraction;

namespace Microsoft.Exchange.Inference.PeopleRelevance
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class NestedSentItemsPipelineFeederFactory : IPipelineComponentFactory
	{
		public IPipelineComponent CreateInstance(IPipelineComponentConfig config, IPipelineContext context)
		{
			throw new NotSupportedException("Doesn't support creating this component without the nested pipeline");
		}

		public IStartStopPipelineComponent CreateInstance(IPipelineComponentConfig config, IPipelineContext context, IPipeline nestedPipeline)
		{
			return new NestedSentItemsPipelineFeeder(config, context, nestedPipeline);
		}
	}
}
