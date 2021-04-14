using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using Microsoft.Search.Platform.Parallax.Core.Model;

namespace Microsoft.Exchange.Search
{
	[GeneratedCode("microsoft.search.platform.parallax.tools.codegenerator.exe", "1.0.0.0")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	internal sealed class _DynamicStorageSelection_IMemoryModelSettings_DataAccessor_ : VariantObjectDataAccessorBase<IMemoryModelSettings, _DynamicStorageSelection_IMemoryModelSettings_Implementation_, _DynamicStorageSelection_IMemoryModelSettings_DataAccessor_>
	{
		internal string _Name_MaterializedValue_;

		internal float _MemoryUsageAdjustmentMultiplier_MaterializedValue_;

		internal ValueProvider<float> _MemoryUsageAdjustmentMultiplier_ValueProvider_;

		internal int _SandboxMaxPoolSize_MaterializedValue_;

		internal ValueProvider<int> _SandboxMaxPoolSize_ValueProvider_;

		internal int _LowAvailableSystemWorkingSetMemoryRatio_MaterializedValue_;

		internal ValueProvider<int> _LowAvailableSystemWorkingSetMemoryRatio_ValueProvider_;

		internal long _SearchMemoryModelBaseCost_MaterializedValue_;

		internal ValueProvider<long> _SearchMemoryModelBaseCost_ValueProvider_;

		internal long _BaselineCostPerActiveItem_MaterializedValue_;

		internal ValueProvider<long> _BaselineCostPerActiveItem_ValueProvider_;

		internal long _BaselineCostPerPassiveItem_MaterializedValue_;

		internal ValueProvider<long> _BaselineCostPerPassiveItem_ValueProvider_;

		internal long _InstantSearchCostPerActiveItem_MaterializedValue_;

		internal ValueProvider<long> _InstantSearchCostPerActiveItem_ValueProvider_;

		internal long _RefinersCostPerActiveItem_MaterializedValue_;

		internal ValueProvider<long> _RefinersCostPerActiveItem_ValueProvider_;

		internal bool _DisableGracefulDegradationForInstantSearch_MaterializedValue_;

		internal ValueProvider<bool> _DisableGracefulDegradationForInstantSearch_ValueProvider_;

		internal bool _DisableGracefulDegradationForAutoSuspend_MaterializedValue_;

		internal ValueProvider<bool> _DisableGracefulDegradationForAutoSuspend_ValueProvider_;

		internal int _TimerForGracefulDegradation_MaterializedValue_;

		internal ValueProvider<int> _TimerForGracefulDegradation_ValueProvider_;

		internal long _MemoryMeasureDrift_MaterializedValue_;

		internal ValueProvider<long> _MemoryMeasureDrift_ValueProvider_;

		internal long _MaxRestoreAmount_MaterializedValue_;

		internal ValueProvider<long> _MaxRestoreAmount_ValueProvider_;

		internal bool _ShouldConsiderSearchMemoryUsageBudget_MaterializedValue_;

		internal ValueProvider<bool> _ShouldConsiderSearchMemoryUsageBudget_ValueProvider_;

		internal long _SearchMemoryUsageBudget_MaterializedValue_;

		internal ValueProvider<long> _SearchMemoryUsageBudget_ValueProvider_;

		internal long _SearchMemoryUsageBudgetFloatingAmount_MaterializedValue_;

		internal ValueProvider<long> _SearchMemoryUsageBudgetFloatingAmount_ValueProvider_;
	}
}
