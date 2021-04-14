using System;
using Microsoft.Exchange.Search.Core.Abstraction;

namespace Microsoft.Exchange.Inference.Common
{
	internal static class PipelineContextPropertyDefinitions
	{
		public static readonly PropertyDefinition TrainingConfiguration = new SimplePropertyDefinition("TrainingConfiguration", typeof(ITrainingConfiguration));
	}
}
