using System;

namespace Microsoft.Exchange.UM.UMCore
{
	internal class TextPipeline : Pipeline
	{
		private TextPipeline()
		{
		}

		internal static Pipeline Instance
		{
			get
			{
				return TextPipeline.instance;
			}
		}

		protected override IPipelineStageFactory[] MyStages
		{
			get
			{
				return TextPipeline.textPipeline;
			}
		}

		private static IPipelineStageFactory[] textPipeline = new IPipelineStageFactory[]
		{
			new PipelineStageFactory<ResolveCallerStage>(),
			new PipelineStageFactory<CreateUnProtectedMessageStage>(),
			new PipelineStageFactory<SmtpSubmitStage>(),
			new PipelineStageFactory<LogPipelineStatisticsStage>()
		};

		private static Pipeline instance = new TextPipeline();
	}
}
