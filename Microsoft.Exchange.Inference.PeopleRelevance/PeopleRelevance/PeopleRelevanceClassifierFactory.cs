using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Search.Core.Abstraction;

namespace Microsoft.Exchange.Inference.PeopleRelevance
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class PeopleRelevanceClassifierFactory : IPipelineComponentFactory
	{
		public IPipelineComponent CreateInstance(IPipelineComponentConfig config, IPipelineContext context)
		{
			return new PeopleRelevanceClassifier();
		}

		public IStartStopPipelineComponent CreateInstance(IPipelineComponentConfig config, IPipelineContext context, IPipeline nestedPipeline)
		{
			throw new NotSupportedException("Doesn't support creating a component that uses nested pipeline");
		}
	}
}
