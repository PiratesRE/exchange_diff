using System;
using System.CodeDom.Compiler;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.Assistants
{
	[GeneratedCode("microsoft.search.platform.parallax.tools.codegenerator.exe", "1.0.0.0")]
	public interface IMailboxAssistantServiceSettings : ISettings
	{
		TimeSpan EventPollingInterval { get; }

		TimeSpan ActiveWatermarksSaveInterval { get; }

		TimeSpan IdleWatermarksSaveInterval { get; }

		TimeSpan WatermarkCleanupInterval { get; }

		int MaxThreadsForAllTimeBasedAssistants { get; }

		int MaxThreadsPerTimeBasedAssistantType { get; }

		TimeSpan HangDetectionTimeout { get; }

		TimeSpan HangDetectionPeriod { get; }

		int MaximumEventQueueSize { get; }

		bool MemoryMonitorEnabled { get; }

		int MemoryBarrierNumberOfSamples { get; }

		TimeSpan MemoryBarrierSamplingInterval { get; }

		TimeSpan MemoryBarrierSamplingDelay { get; }

		long MemoryBarrierPrivateBytesUsageLimit { get; }

		TimeSpan WorkCycleUpdatePeriod { get; }

		TimeSpan BatchDuration { get; }
	}
}
