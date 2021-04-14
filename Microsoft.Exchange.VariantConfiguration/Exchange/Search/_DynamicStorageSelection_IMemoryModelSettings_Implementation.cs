using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using Microsoft.Exchange.VariantConfiguration;
using Microsoft.Search.Platform.Parallax.Core.Model;

namespace Microsoft.Exchange.Search
{
	[EditorBrowsable(EditorBrowsableState.Never)]
	[GeneratedCode("microsoft.search.platform.parallax.tools.codegenerator.exe", "1.0.0.0")]
	internal sealed class _DynamicStorageSelection_IMemoryModelSettings_Implementation_ : IMemoryModelSettings, ISettings, IDataAccessorBackedObject<_DynamicStorageSelection_IMemoryModelSettings_DataAccessor_>, IVariantObjectInstance
	{
		VariantContextSnapshot IVariantObjectInstance.Context
		{
			get
			{
				return this.context;
			}
		}

		_DynamicStorageSelection_IMemoryModelSettings_DataAccessor_ IDataAccessorBackedObject<_DynamicStorageSelection_IMemoryModelSettings_DataAccessor_>.DataAccessor
		{
			get
			{
				return this.dataAccessor;
			}
		}

		void IDataAccessorBackedObject<_DynamicStorageSelection_IMemoryModelSettings_DataAccessor_>.Initialize(_DynamicStorageSelection_IMemoryModelSettings_DataAccessor_ dataAccessor, VariantContextSnapshot context)
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

		public float MemoryUsageAdjustmentMultiplier
		{
			get
			{
				if (this.dataAccessor._MemoryUsageAdjustmentMultiplier_ValueProvider_ != null)
				{
					return this.dataAccessor._MemoryUsageAdjustmentMultiplier_ValueProvider_.GetValue(this.context);
				}
				return this.dataAccessor._MemoryUsageAdjustmentMultiplier_MaterializedValue_;
			}
		}

		public int SandboxMaxPoolSize
		{
			get
			{
				if (this.dataAccessor._SandboxMaxPoolSize_ValueProvider_ != null)
				{
					return this.dataAccessor._SandboxMaxPoolSize_ValueProvider_.GetValue(this.context);
				}
				return this.dataAccessor._SandboxMaxPoolSize_MaterializedValue_;
			}
		}

		public int LowAvailableSystemWorkingSetMemoryRatio
		{
			get
			{
				if (this.dataAccessor._LowAvailableSystemWorkingSetMemoryRatio_ValueProvider_ != null)
				{
					return this.dataAccessor._LowAvailableSystemWorkingSetMemoryRatio_ValueProvider_.GetValue(this.context);
				}
				return this.dataAccessor._LowAvailableSystemWorkingSetMemoryRatio_MaterializedValue_;
			}
		}

		public long SearchMemoryModelBaseCost
		{
			get
			{
				if (this.dataAccessor._SearchMemoryModelBaseCost_ValueProvider_ != null)
				{
					return this.dataAccessor._SearchMemoryModelBaseCost_ValueProvider_.GetValue(this.context);
				}
				return this.dataAccessor._SearchMemoryModelBaseCost_MaterializedValue_;
			}
		}

		public long BaselineCostPerActiveItem
		{
			get
			{
				if (this.dataAccessor._BaselineCostPerActiveItem_ValueProvider_ != null)
				{
					return this.dataAccessor._BaselineCostPerActiveItem_ValueProvider_.GetValue(this.context);
				}
				return this.dataAccessor._BaselineCostPerActiveItem_MaterializedValue_;
			}
		}

		public long BaselineCostPerPassiveItem
		{
			get
			{
				if (this.dataAccessor._BaselineCostPerPassiveItem_ValueProvider_ != null)
				{
					return this.dataAccessor._BaselineCostPerPassiveItem_ValueProvider_.GetValue(this.context);
				}
				return this.dataAccessor._BaselineCostPerPassiveItem_MaterializedValue_;
			}
		}

		public long InstantSearchCostPerActiveItem
		{
			get
			{
				if (this.dataAccessor._InstantSearchCostPerActiveItem_ValueProvider_ != null)
				{
					return this.dataAccessor._InstantSearchCostPerActiveItem_ValueProvider_.GetValue(this.context);
				}
				return this.dataAccessor._InstantSearchCostPerActiveItem_MaterializedValue_;
			}
		}

		public long RefinersCostPerActiveItem
		{
			get
			{
				if (this.dataAccessor._RefinersCostPerActiveItem_ValueProvider_ != null)
				{
					return this.dataAccessor._RefinersCostPerActiveItem_ValueProvider_.GetValue(this.context);
				}
				return this.dataAccessor._RefinersCostPerActiveItem_MaterializedValue_;
			}
		}

		public bool DisableGracefulDegradationForInstantSearch
		{
			get
			{
				if (this.dataAccessor._DisableGracefulDegradationForInstantSearch_ValueProvider_ != null)
				{
					return this.dataAccessor._DisableGracefulDegradationForInstantSearch_ValueProvider_.GetValue(this.context);
				}
				return this.dataAccessor._DisableGracefulDegradationForInstantSearch_MaterializedValue_;
			}
		}

		public bool DisableGracefulDegradationForAutoSuspend
		{
			get
			{
				if (this.dataAccessor._DisableGracefulDegradationForAutoSuspend_ValueProvider_ != null)
				{
					return this.dataAccessor._DisableGracefulDegradationForAutoSuspend_ValueProvider_.GetValue(this.context);
				}
				return this.dataAccessor._DisableGracefulDegradationForAutoSuspend_MaterializedValue_;
			}
		}

		public int TimerForGracefulDegradation
		{
			get
			{
				if (this.dataAccessor._TimerForGracefulDegradation_ValueProvider_ != null)
				{
					return this.dataAccessor._TimerForGracefulDegradation_ValueProvider_.GetValue(this.context);
				}
				return this.dataAccessor._TimerForGracefulDegradation_MaterializedValue_;
			}
		}

		public long MemoryMeasureDrift
		{
			get
			{
				if (this.dataAccessor._MemoryMeasureDrift_ValueProvider_ != null)
				{
					return this.dataAccessor._MemoryMeasureDrift_ValueProvider_.GetValue(this.context);
				}
				return this.dataAccessor._MemoryMeasureDrift_MaterializedValue_;
			}
		}

		public long MaxRestoreAmount
		{
			get
			{
				if (this.dataAccessor._MaxRestoreAmount_ValueProvider_ != null)
				{
					return this.dataAccessor._MaxRestoreAmount_ValueProvider_.GetValue(this.context);
				}
				return this.dataAccessor._MaxRestoreAmount_MaterializedValue_;
			}
		}

		public bool ShouldConsiderSearchMemoryUsageBudget
		{
			get
			{
				if (this.dataAccessor._ShouldConsiderSearchMemoryUsageBudget_ValueProvider_ != null)
				{
					return this.dataAccessor._ShouldConsiderSearchMemoryUsageBudget_ValueProvider_.GetValue(this.context);
				}
				return this.dataAccessor._ShouldConsiderSearchMemoryUsageBudget_MaterializedValue_;
			}
		}

		public long SearchMemoryUsageBudget
		{
			get
			{
				if (this.dataAccessor._SearchMemoryUsageBudget_ValueProvider_ != null)
				{
					return this.dataAccessor._SearchMemoryUsageBudget_ValueProvider_.GetValue(this.context);
				}
				return this.dataAccessor._SearchMemoryUsageBudget_MaterializedValue_;
			}
		}

		public long SearchMemoryUsageBudgetFloatingAmount
		{
			get
			{
				if (this.dataAccessor._SearchMemoryUsageBudgetFloatingAmount_ValueProvider_ != null)
				{
					return this.dataAccessor._SearchMemoryUsageBudgetFloatingAmount_ValueProvider_.GetValue(this.context);
				}
				return this.dataAccessor._SearchMemoryUsageBudgetFloatingAmount_MaterializedValue_;
			}
		}

		private _DynamicStorageSelection_IMemoryModelSettings_DataAccessor_ dataAccessor;

		private VariantContextSnapshot context;
	}
}
