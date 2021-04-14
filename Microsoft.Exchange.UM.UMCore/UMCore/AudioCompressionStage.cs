using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.UM.UMCore
{
	internal class AudioCompressionStage : SynchronousPipelineStageBase
	{
		internal override PipelineDispatcher.PipelineResourceType ResourceType
		{
			get
			{
				return PipelineDispatcher.PipelineResourceType.CpuBound;
			}
		}

		internal override TimeSpan ExpectedRunTime
		{
			get
			{
				return TimeSpan.FromMinutes(3.0);
			}
		}

		protected override void InternalDoSynchronousWork()
		{
			IUMCompressAudio iumcompressAudio = base.WorkItem.Message as IUMCompressAudio;
			ExAssert.RetailAssert(iumcompressAudio != null, "AudioCompressionStage must operate on PipelineContext whcih implements IUMCompressAudio.");
			ExDateTime utcNow = ExDateTime.UtcNow;
			if (iumcompressAudio.FileToCompressPath != null)
			{
				iumcompressAudio.CompressedAudioFile = MediaMethods.FromPcm(iumcompressAudio.FileToCompressPath, iumcompressAudio.AudioCodec);
			}
			base.WorkItem.PipelineStatisticsLogRow.AudioCodec = iumcompressAudio.AudioCodec;
			base.WorkItem.PipelineStatisticsLogRow.AudioCompressionElapsedTime = ExDateTime.UtcNow - utcNow;
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<AudioCompressionStage>(this);
		}
	}
}
