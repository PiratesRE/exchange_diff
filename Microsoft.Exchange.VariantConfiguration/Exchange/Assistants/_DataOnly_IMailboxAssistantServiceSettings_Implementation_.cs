using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using Microsoft.Exchange.VariantConfiguration;
using Microsoft.Search.Platform.Parallax.Core.Model;

namespace Microsoft.Exchange.Assistants
{
	[GeneratedCode("microsoft.search.platform.parallax.tools.codegenerator.exe", "1.0.0.0")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	internal sealed class _DataOnly_IMailboxAssistantServiceSettings_Implementation_ : IMailboxAssistantServiceSettings, ISettings, IVariantObjectInstance, IVariantObjectInstanceProvider
	{
		VariantContextSnapshot IVariantObjectInstance.Context
		{
			get
			{
				return null;
			}
		}

		IVariantObjectInstance IVariantObjectInstanceProvider.GetVariantObjectInstance(VariantContextSnapshot context)
		{
			return this;
		}

		public string Name
		{
			get
			{
				return this._Name_MaterializedValue_;
			}
		}

		public TimeSpan EventPollingInterval
		{
			get
			{
				return this._EventPollingInterval_MaterializedValue_;
			}
		}

		public TimeSpan ActiveWatermarksSaveInterval
		{
			get
			{
				return this._ActiveWatermarksSaveInterval_MaterializedValue_;
			}
		}

		public TimeSpan IdleWatermarksSaveInterval
		{
			get
			{
				return this._IdleWatermarksSaveInterval_MaterializedValue_;
			}
		}

		public TimeSpan WatermarkCleanupInterval
		{
			get
			{
				return this._WatermarkCleanupInterval_MaterializedValue_;
			}
		}

		public int MaxThreadsForAllTimeBasedAssistants
		{
			get
			{
				return this._MaxThreadsForAllTimeBasedAssistants_MaterializedValue_;
			}
		}

		public int MaxThreadsPerTimeBasedAssistantType
		{
			get
			{
				return this._MaxThreadsPerTimeBasedAssistantType_MaterializedValue_;
			}
		}

		public TimeSpan HangDetectionTimeout
		{
			get
			{
				return this._HangDetectionTimeout_MaterializedValue_;
			}
		}

		public TimeSpan HangDetectionPeriod
		{
			get
			{
				return this._HangDetectionPeriod_MaterializedValue_;
			}
		}

		public int MaximumEventQueueSize
		{
			get
			{
				return this._MaximumEventQueueSize_MaterializedValue_;
			}
		}

		public bool MemoryMonitorEnabled
		{
			get
			{
				return this._MemoryMonitorEnabled_MaterializedValue_;
			}
		}

		public int MemoryBarrierNumberOfSamples
		{
			get
			{
				return this._MemoryBarrierNumberOfSamples_MaterializedValue_;
			}
		}

		public TimeSpan MemoryBarrierSamplingInterval
		{
			get
			{
				return this._MemoryBarrierSamplingInterval_MaterializedValue_;
			}
		}

		public TimeSpan MemoryBarrierSamplingDelay
		{
			get
			{
				return this._MemoryBarrierSamplingDelay_MaterializedValue_;
			}
		}

		public long MemoryBarrierPrivateBytesUsageLimit
		{
			get
			{
				return this._MemoryBarrierPrivateBytesUsageLimit_MaterializedValue_;
			}
		}

		public TimeSpan WorkCycleUpdatePeriod
		{
			get
			{
				return this._WorkCycleUpdatePeriod_MaterializedValue_;
			}
		}

		public TimeSpan BatchDuration
		{
			get
			{
				return this._BatchDuration_MaterializedValue_;
			}
		}

		internal string _Name_MaterializedValue_;

		internal TimeSpan _EventPollingInterval_MaterializedValue_ = default(TimeSpan);

		internal TimeSpan _ActiveWatermarksSaveInterval_MaterializedValue_ = default(TimeSpan);

		internal TimeSpan _IdleWatermarksSaveInterval_MaterializedValue_ = default(TimeSpan);

		internal TimeSpan _WatermarkCleanupInterval_MaterializedValue_ = default(TimeSpan);

		internal int _MaxThreadsForAllTimeBasedAssistants_MaterializedValue_;

		internal int _MaxThreadsPerTimeBasedAssistantType_MaterializedValue_;

		internal TimeSpan _HangDetectionTimeout_MaterializedValue_ = default(TimeSpan);

		internal TimeSpan _HangDetectionPeriod_MaterializedValue_ = default(TimeSpan);

		internal int _MaximumEventQueueSize_MaterializedValue_;

		internal bool _MemoryMonitorEnabled_MaterializedValue_;

		internal int _MemoryBarrierNumberOfSamples_MaterializedValue_;

		internal TimeSpan _MemoryBarrierSamplingInterval_MaterializedValue_ = default(TimeSpan);

		internal TimeSpan _MemoryBarrierSamplingDelay_MaterializedValue_ = default(TimeSpan);

		internal long _MemoryBarrierPrivateBytesUsageLimit_MaterializedValue_;

		internal TimeSpan _WorkCycleUpdatePeriod_MaterializedValue_ = default(TimeSpan);

		internal TimeSpan _BatchDuration_MaterializedValue_ = default(TimeSpan);
	}
}
