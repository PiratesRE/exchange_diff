using System;

namespace Microsoft.Exchange.UM.UMCore
{
	internal class CDRPipeline : Pipeline
	{
		private CDRPipeline()
		{
		}

		internal static Pipeline Instance
		{
			get
			{
				return CDRPipeline.instance;
			}
		}

		protected override IPipelineStageFactory[] MyStages
		{
			get
			{
				return CDRPipeline.cdrPipeline;
			}
		}

		private static IPipelineStageFactory[] cdrPipeline = new IPipelineStageFactory[]
		{
			new PipelineStageFactory<CDRPipelineStage>()
		};

		private static Pipeline instance = new CDRPipeline();
	}
}
