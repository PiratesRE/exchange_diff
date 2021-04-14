using System;

namespace Microsoft.Exchange.UM.UMCore
{
	internal class AudioPipeline : Pipeline
	{
		private AudioPipeline()
		{
		}

		internal static Pipeline Instance
		{
			get
			{
				return AudioPipeline.instance;
			}
		}

		protected override IPipelineStageFactory[] MyStages
		{
			get
			{
				return AudioPipeline.audioPipeline;
			}
		}

		private static IPipelineStageFactory[] audioPipeline = new IPipelineStageFactory[]
		{
			new PipelineStageFactory<TranscriptionConfigurationStage>(),
			new PipelineStageFactory<ResolveCallerStage>(),
			new PipelineStageFactory<AudioCompressionStage>(),
			new PipelineStageFactory<TranscriptionStage>(),
			new PipelineStageFactory<SearchFolderVerificationStage>(),
			new PipelineStageFactory<CreateUnProtectedMessageStage>(),
			new PipelineStageFactory<SmtpSubmitStage>(),
			new PipelineStageFactory<LogPipelineStatisticsStage>(),
			new PipelineStageFactory<VoiceMessageCollectionStage>()
		};

		private static Pipeline instance = new AudioPipeline();
	}
}
