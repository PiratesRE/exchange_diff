using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using Microsoft.Exchange.VariantConfiguration;
using Microsoft.Search.Platform.Parallax.Core.Model;

namespace Microsoft.Exchange.Assistants
{
	[GeneratedCode("microsoft.search.platform.parallax.tools.codegenerator.exe", "1.0.0.0")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	internal sealed class _DynamicStorageSelection_IMailboxAssistantServiceSettings_Implementation_ : IMailboxAssistantServiceSettings, ISettings, IDataAccessorBackedObject<_DynamicStorageSelection_IMailboxAssistantServiceSettings_DataAccessor_>, IVariantObjectInstance
	{
		VariantContextSnapshot IVariantObjectInstance.Context
		{
			get
			{
				return this.context;
			}
		}

		_DynamicStorageSelection_IMailboxAssistantServiceSettings_DataAccessor_ IDataAccessorBackedObject<_DynamicStorageSelection_IMailboxAssistantServiceSettings_DataAccessor_>.DataAccessor
		{
			get
			{
				return this.dataAccessor;
			}
		}

		void IDataAccessorBackedObject<_DynamicStorageSelection_IMailboxAssistantServiceSettings_DataAccessor_>.Initialize(_DynamicStorageSelection_IMailboxAssistantServiceSettings_DataAccessor_ dataAccessor, VariantContextSnapshot context)
		{
			this.dataAccessor = dataAccessor;
			this.context = context;
		}

		public string Name
		{
			get
			{
				return this.dataAccessor._Name_MaterializedValue_;
			}
		}

		public TimeSpan EventPollingInterval
		{
			get
			{
				if (this.dataAccessor._EventPollingInterval_ValueProvider_ != null)
				{
					return this.dataAccessor._EventPollingInterval_ValueProvider_.GetValue(this.context);
				}
				return this.dataAccessor._EventPollingInterval_MaterializedValue_;
			}
		}

		public TimeSpan ActiveWatermarksSaveInterval
		{
			get
			{
				if (this.dataAccessor._ActiveWatermarksSaveInterval_ValueProvider_ != null)
				{
					return this.dataAccessor._ActiveWatermarksSaveInterval_ValueProvider_.GetValue(this.context);
				}
				return this.dataAccessor._ActiveWatermarksSaveInterval_MaterializedValue_;
			}
		}

		public TimeSpan IdleWatermarksSaveInterval
		{
			get
			{
				if (this.dataAccessor._IdleWatermarksSaveInterval_ValueProvider_ != null)
				{
					return this.dataAccessor._IdleWatermarksSaveInterval_ValueProvider_.GetValue(this.context);
				}
				return this.dataAccessor._IdleWatermarksSaveInterval_MaterializedValue_;
			}
		}

		public TimeSpan WatermarkCleanupInterval
		{
			get
			{
				if (this.dataAccessor._WatermarkCleanupInterval_ValueProvider_ != null)
				{
					return this.dataAccessor._WatermarkCleanupInterval_ValueProvider_.GetValue(this.context);
				}
				return this.dataAccessor._WatermarkCleanupInterval_MaterializedValue_;
			}
		}

		public int MaxThreadsForAllTimeBasedAssistants
		{
			get
			{
				if (this.dataAccessor._MaxThreadsForAllTimeBasedAssistants_ValueProvider_ != null)
				{
					return this.dataAccessor._MaxThreadsForAllTimeBasedAssistants_ValueProvider_.GetValue(this.context);
				}
				return this.dataAccessor._MaxThreadsForAllTimeBasedAssistants_MaterializedValue_;
			}
		}

		public int MaxThreadsPerTimeBasedAssistantType
		{
			get
			{
				if (this.dataAccessor._MaxThreadsPerTimeBasedAssistantType_ValueProvider_ != null)
				{
					return this.dataAccessor._MaxThreadsPerTimeBasedAssistantType_ValueProvider_.GetValue(this.context);
				}
				return this.dataAccessor._MaxThreadsPerTimeBasedAssistantType_MaterializedValue_;
			}
		}

		public TimeSpan HangDetectionTimeout
		{
			get
			{
				if (this.dataAccessor._HangDetectionTimeout_ValueProvider_ != null)
				{
					return this.dataAccessor._HangDetectionTimeout_ValueProvider_.GetValue(this.context);
				}
				return this.dataAccessor._HangDetectionTimeout_MaterializedValue_;
			}
		}

		public TimeSpan HangDetectionPeriod
		{
			get
			{
				if (this.dataAccessor._HangDetectionPeriod_ValueProvider_ != null)
				{
					return this.dataAccessor._HangDetectionPeriod_ValueProvider_.GetValue(this.context);
				}
				return this.dataAccessor._HangDetectionPeriod_MaterializedValue_;
			}
		}

		public int MaximumEventQueueSize
		{
			get
			{
				if (this.dataAccessor._MaximumEventQueueSize_ValueProvider_ != null)
				{
					return this.dataAccessor._MaximumEventQueueSize_ValueProvider_.GetValue(this.context);
				}
				return this.dataAccessor._MaximumEventQueueSize_MaterializedValue_;
			}
		}

		public bool MemoryMonitorEnabled
		{
			get
			{
				if (this.dataAccessor._MemoryMonitorEnabled_ValueProvider_ != null)
				{
					return this.dataAccessor._MemoryMonitorEnabled_ValueProvider_.GetValue(this.context);
				}
				return this.dataAccessor._MemoryMonitorEnabled_MaterializedValue_;
			}
		}

		public int MemoryBarrierNumberOfSamples
		{
			get
			{
				if (this.dataAccessor._MemoryBarrierNumberOfSamples_ValueProvider_ != null)
				{
					return this.dataAccessor._MemoryBarrierNumberOfSamples_ValueProvider_.GetValue(this.context);
				}
				return this.dataAccessor._MemoryBarrierNumberOfSamples_MaterializedValue_;
			}
		}

		public TimeSpan MemoryBarrierSamplingInterval
		{
			get
			{
				if (this.dataAccessor._MemoryBarrierSamplingInterval_ValueProvider_ != null)
				{
					return this.dataAccessor._MemoryBarrierSamplingInterval_ValueProvider_.GetValue(this.context);
				}
				return this.dataAccessor._MemoryBarrierSamplingInterval_MaterializedValue_;
			}
		}

		public TimeSpan MemoryBarrierSamplingDelay
		{
			get
			{
				if (this.dataAccessor._MemoryBarrierSamplingDelay_ValueProvider_ != null)
				{
					return this.dataAccessor._MemoryBarrierSamplingDelay_ValueProvider_.GetValue(this.context);
				}
				return this.dataAccessor._MemoryBarrierSamplingDelay_MaterializedValue_;
			}
		}

		public long MemoryBarrierPrivateBytesUsageLimit
		{
			get
			{
				if (this.dataAccessor._MemoryBarrierPrivateBytesUsageLimit_ValueProvider_ != null)
				{
					return this.dataAccessor._MemoryBarrierPrivateBytesUsageLimit_ValueProvider_.GetValue(this.context);
				}
				return this.dataAccessor._MemoryBarrierPrivateBytesUsageLimit_MaterializedValue_;
			}
		}

		public TimeSpan WorkCycleUpdatePeriod
		{
			get
			{
				if (this.dataAccessor._WorkCycleUpdatePeriod_ValueProvider_ != null)
				{
					return this.dataAccessor._WorkCycleUpdatePeriod_ValueProvider_.GetValue(this.context);
				}
				return this.dataAccessor._WorkCycleUpdatePeriod_MaterializedValue_;
			}
		}

		public TimeSpan BatchDuration
		{
			get
			{
				if (this.dataAccessor._BatchDuration_ValueProvider_ != null)
				{
					return this.dataAccessor._BatchDuration_ValueProvider_.GetValue(this.context);
				}
				return this.dataAccessor._BatchDuration_MaterializedValue_;
			}
		}

		private _DynamicStorageSelection_IMailboxAssistantServiceSettings_DataAccessor_ dataAccessor;

		private VariantContextSnapshot context;
	}
}
