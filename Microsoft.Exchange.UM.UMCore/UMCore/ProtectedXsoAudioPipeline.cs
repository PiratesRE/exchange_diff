using System;

namespace Microsoft.Exchange.UM.UMCore
{
	internal class ProtectedXsoAudioPipeline : Pipeline
	{
		private ProtectedXsoAudioPipeline()
		{
		}

		internal static Pipeline Instance
		{
			get
			{
				return ProtectedXsoAudioPipeline.instance;
			}
		}

		protected override IPipelineStageFactory[] MyStages
		{
			get
			{
				return ProtectedXsoAudioPipeline.audioPipeline;
			}
		}

		private static IPipelineStageFactory[] audioPipeline = new IPipelineStageFactory[]
		{
			new PipelineStageFactory<TranscriptionConfigurationStage>(),
			new PipelineStageFactory<AudioCompressionStage>(),
			new PipelineStageFactory<TranscriptionStage>(),
			new PipelineStageFactory<CreateProtectedMessageStage>(),
			new PipelineStageFactory<XSOSubmitStage>(),
			new PipelineStageFactory<LogPipelineStatisticsStage>()
		};

		private static Pipeline instance = new ProtectedXsoAudioPipeline();
	}
}
