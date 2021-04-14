using System;

namespace Microsoft.Exchange.UM.UMCore
{
	internal class XSOAudioPipeline : Pipeline
	{
		private XSOAudioPipeline()
		{
		}

		internal static Pipeline Instance
		{
			get
			{
				return XSOAudioPipeline.instance;
			}
		}

		protected override IPipelineStageFactory[] MyStages
		{
			get
			{
				return XSOAudioPipeline.audioPipeline;
			}
		}

		private static IPipelineStageFactory[] audioPipeline = new IPipelineStageFactory[]
		{
			new PipelineStageFactory<TranscriptionConfigurationStage>(),
			new PipelineStageFactory<AudioCompressionStage>(),
			new PipelineStageFactory<TranscriptionStage>(),
			new PipelineStageFactory<CreateUnProtectedMessageStage>(),
			new PipelineStageFactory<XSOSubmitStage>(),
			new PipelineStageFactory<LogPipelineStatisticsStage>()
		};

		private static Pipeline instance = new XSOAudioPipeline();
	}
}
