using System;

namespace Microsoft.Exchange.UM.UMCore
{
	internal class HealthCheckPipeline : Pipeline
	{
		static HealthCheckPipeline()
		{
			HealthCheckPipeline.healthCheckPipeline = new IPipelineStageFactory[3];
			for (int i = 0; i < HealthCheckPipeline.healthCheckPipeline.Length; i++)
			{
				HealthCheckPipeline.healthCheckPipeline[i] = new PipelineStageFactory<HealthCheckStage>();
			}
		}

		private HealthCheckPipeline()
		{
		}

		internal static Pipeline Instance
		{
			get
			{
				return HealthCheckPipeline.instance;
			}
		}

		protected override IPipelineStageFactory[] MyStages
		{
			get
			{
				return HealthCheckPipeline.healthCheckPipeline;
			}
		}

		private static IPipelineStageFactory[] healthCheckPipeline;

		private static Pipeline instance = new HealthCheckPipeline();
	}
}
