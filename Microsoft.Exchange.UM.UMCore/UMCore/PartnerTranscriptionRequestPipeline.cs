using System;

namespace Microsoft.Exchange.UM.UMCore
{
	internal class PartnerTranscriptionRequestPipeline : Pipeline
	{
		private PartnerTranscriptionRequestPipeline()
		{
		}

		internal static Pipeline Instance
		{
			get
			{
				return PartnerTranscriptionRequestPipeline.instance;
			}
		}

		protected override IPipelineStageFactory[] MyStages
		{
			get
			{
				return PartnerTranscriptionRequestPipeline.myStages;
			}
		}

		private static IPipelineStageFactory[] myStages = new IPipelineStageFactory[]
		{
			new PipelineStageFactory<AudioCompressionStage>(),
			new PipelineStageFactory<CreateUnProtectedMessageStage>(),
			new PipelineStageFactory<SmtpSubmitStage>(),
			new PipelineStageFactory<LogPipelineStatisticsStage>()
		};

		private static Pipeline instance = new PartnerTranscriptionRequestPipeline();
	}
}
