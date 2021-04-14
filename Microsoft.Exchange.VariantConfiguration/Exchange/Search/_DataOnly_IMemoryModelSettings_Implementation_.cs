using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using Microsoft.Exchange.VariantConfiguration;
using Microsoft.Search.Platform.Parallax.Core.Model;

namespace Microsoft.Exchange.Search
{
	[GeneratedCode("microsoft.search.platform.parallax.tools.codegenerator.exe", "1.0.0.0")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	internal sealed class _DataOnly_IMemoryModelSettings_Implementation_ : IMemoryModelSettings, ISettings, IVariantObjectInstance, IVariantObjectInstanceProvider
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

		public float MemoryUsageAdjustmentMultiplier
		{
			get
			{
				return this._MemoryUsageAdjustmentMultiplier_MaterializedValue_;
			}
		}

		public int SandboxMaxPoolSize
		{
			get
			{
				return this._SandboxMaxPoolSize_MaterializedValue_;
			}
		}

		public int LowAvailableSystemWorkingSetMemoryRatio
		{
			get
			{
				return this._LowAvailableSystemWorkingSetMemoryRatio_MaterializedValue_;
			}
		}

		public long SearchMemoryModelBaseCost
		{
			get
			{
				return this._SearchMemoryModelBaseCost_MaterializedValue_;
			}
		}

		public long BaselineCostPerActiveItem
		{
			get
			{
				return this._BaselineCostPerActiveItem_MaterializedValue_;
			}
		}

		public long BaselineCostPerPassiveItem
		{
			get
			{
				return this._BaselineCostPerPassiveItem_MaterializedValue_;
			}
		}

		public long InstantSearchCostPerActiveItem
		{
			get
			{
				return this._InstantSearchCostPerActiveItem_MaterializedValue_;
			}
		}

		public long RefinersCostPerActiveItem
		{
			get
			{
				return this._RefinersCostPerActiveItem_MaterializedValue_;
			}
		}

		public bool DisableGracefulDegradationForInstantSearch
		{
			get
			{
				return this._DisableGracefulDegradationForInstantSearch_MaterializedValue_;
			}
		}

		public bool DisableGracefulDegradationForAutoSuspend
		{
			get
			{
				return this._DisableGracefulDegradationForAutoSuspend_MaterializedValue_;
			}
		}

		public int TimerForGracefulDegradation
		{
			get
			{
				return this._TimerForGracefulDegradation_MaterializedValue_;
			}
		}

		public long MemoryMeasureDrift
		{
			get
			{
				return this._MemoryMeasureDrift_MaterializedValue_;
			}
		}

		public long MaxRestoreAmount
		{
			get
			{
				return this._MaxRestoreAmount_MaterializedValue_;
			}
		}

		public bool ShouldConsiderSearchMemoryUsageBudget
		{
			get
			{
				return this._ShouldConsiderSearchMemoryUsageBudget_MaterializedValue_;
			}
		}

		public long SearchMemoryUsageBudget
		{
			get
			{
				return this._SearchMemoryUsageBudget_MaterializedValue_;
			}
		}

		public long SearchMemoryUsageBudgetFloatingAmount
		{
			get
			{
				return this._SearchMemoryUsageBudgetFloatingAmount_MaterializedValue_;
			}
		}

		internal string _Name_MaterializedValue_;

		internal float _MemoryUsageAdjustmentMultiplier_MaterializedValue_;

		internal int _SandboxMaxPoolSize_MaterializedValue_;

		internal int _LowAvailableSystemWorkingSetMemoryRatio_MaterializedValue_;

		internal long _SearchMemoryModelBaseCost_MaterializedValue_;

		internal long _BaselineCostPerActiveItem_MaterializedValue_;

		internal long _BaselineCostPerPassiveItem_MaterializedValue_;

		internal long _InstantSearchCostPerActiveItem_MaterializedValue_;

		internal long _RefinersCostPerActiveItem_MaterializedValue_;

		internal bool _DisableGracefulDegradationForInstantSearch_MaterializedValue_;

		internal bool _DisableGracefulDegradationForAutoSuspend_MaterializedValue_;

		internal int _TimerForGracefulDegradation_MaterializedValue_;

		internal long _MemoryMeasureDrift_MaterializedValue_;

		internal long _MaxRestoreAmount_MaterializedValue_;

		internal bool _ShouldConsiderSearchMemoryUsageBudget_MaterializedValue_;

		internal long _SearchMemoryUsageBudget_MaterializedValue_;

		internal long _SearchMemoryUsageBudgetFloatingAmount_MaterializedValue_;
	}
}
