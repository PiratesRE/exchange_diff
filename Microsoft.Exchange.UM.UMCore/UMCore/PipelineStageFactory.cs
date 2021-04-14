using System;

namespace Microsoft.Exchange.UM.UMCore
{
	internal class PipelineStageFactory<T> : IPipelineStageFactory where T : PipelineStageBase, new()
	{
		public PipelineStageBase CreateStage(PipelineWorkItem workItem)
		{
			T t = Activator.CreateInstance<T>();
			t.Initialize(workItem);
			return t;
		}
	}
}
