using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using Microsoft.Search.Platform.Parallax.Core.Model;

namespace Microsoft.Exchange.Assistants
{
	[EditorBrowsable(EditorBrowsableState.Never)]
	[GeneratedCode("microsoft.search.platform.parallax.tools.codegenerator.exe", "1.0.0.0")]
	internal sealed class _DynamicStorageSelection_IMailboxAssistantServiceSettings_DataAccessor_ : VariantObjectDataAccessorBase<IMailboxAssistantServiceSettings, _DynamicStorageSelection_IMailboxAssistantServiceSettings_Implementation_, _DynamicStorageSelection_IMailboxAssistantServiceSettings_DataAccessor_>
	{
		internal string _Name_MaterializedValue_;

		internal TimeSpan _EventPollingInterval_MaterializedValue_ = default(TimeSpan);

		internal ValueProvider<TimeSpan> _EventPollingInterval_ValueProvider_;

		internal TimeSpan _ActiveWatermarksSaveInterval_MaterializedValue_ = default(TimeSpan);

		internal ValueProvider<TimeSpan> _ActiveWatermarksSaveInterval_ValueProvider_;

		internal TimeSpan _IdleWatermarksSaveInterval_MaterializedValue_ = default(TimeSpan);

		internal ValueProvider<TimeSpan> _IdleWatermarksSaveInterval_ValueProvider_;

		internal TimeSpan _WatermarkCleanupInterval_MaterializedValue_ = default(TimeSpan);

		internal ValueProvider<TimeSpan> _WatermarkCleanupInterval_ValueProvider_;

		internal int _MaxThreadsForAllTimeBasedAssistants_MaterializedValue_;

		internal ValueProvider<int> _MaxThreadsForAllTimeBasedAssistants_ValueProvider_;

		internal int _MaxThreadsPerTimeBasedAssistantType_MaterializedValue_;

		internal ValueProvider<int> _MaxThreadsPerTimeBasedAssistantType_ValueProvider_;

		internal TimeSpan _HangDetectionTimeout_MaterializedValue_ = default(TimeSpan);

		internal ValueProvider<TimeSpan> _HangDetectionTimeout_ValueProvider_;

		internal TimeSpan _HangDetectionPeriod_MaterializedValue_ = default(TimeSpan);

		internal ValueProvider<TimeSpan> _HangDetectionPeriod_ValueProvider_;

		internal int _MaximumEventQueueSize_MaterializedValue_;

		internal ValueProvider<int> _MaximumEventQueueSize_ValueProvider_;

		internal bool _MemoryMonitorEnabled_MaterializedValue_;

		internal ValueProvider<bool> _MemoryMonitorEnabled_ValueProvider_;

		internal int _MemoryBarrierNumberOfSamples_MaterializedValue_;

		internal ValueProvider<int> _MemoryBarrierNumberOfSamples_ValueProvider_;

		internal TimeSpan _MemoryBarrierSamplingInterval_MaterializedValue_ = default(TimeSpan);

		internal ValueProvider<TimeSpan> _MemoryBarrierSamplingInterval_ValueProvider_;

		internal TimeSpan _MemoryBarrierSamplingDelay_MaterializedValue_ = default(TimeSpan);

		internal ValueProvider<TimeSpan> _MemoryBarrierSamplingDelay_ValueProvider_;

		internal long _MemoryBarrierPrivateBytesUsageLimit_MaterializedValue_;

		internal ValueProvider<long> _MemoryBarrierPrivateBytesUsageLimit_ValueProvider_;

		internal TimeSpan _WorkCycleUpdatePeriod_MaterializedValue_ = default(TimeSpan);

		internal ValueProvider<TimeSpan> _WorkCycleUpdatePeriod_ValueProvider_;

		internal TimeSpan _BatchDuration_MaterializedValue_ = default(TimeSpan);

		internal ValueProvider<TimeSpan> _BatchDuration_ValueProvider_;
	}
}
