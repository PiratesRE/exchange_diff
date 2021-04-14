using System;

namespace Microsoft.Exchange.UM.UMCore
{
	internal class ProtectedAudioPipeline : Pipeline
	{
		private ProtectedAudioPipeline()
		{
		}

		internal static Pipeline Instance
		{
			get
			{
				return ProtectedAudioPipeline.instance;
			}
		}

		protected override IPipelineStageFactory[] MyStages
		{
			get
			{
				return ProtectedAudioPipeline.audioPipeline;
			}
		}

		private static IPipelineStageFactory[] audioPipeline = new IPipelineStageFactory[]
		{
			new PipelineStageFactory<TranscriptionConfigurationStage>(),
			new PipelineStageFactory<ResolveCallerStage>(),
			new PipelineStageFactory<AudioCompressionStage>(),
			new PipelineStageFactory<TranscriptionStage>(),
			new PipelineStageFactory<SearchFolderVerificationStage>(),
			new PipelineStageFactory<CreateProtectedMessageStage>(),
			new PipelineStageFactory<SmtpSubmitStage>(),
			new PipelineStageFactory<LogPipelineStatisticsStage>()
		};

		private static Pipeline instance = new ProtectedAudioPipeline();
	}
}
