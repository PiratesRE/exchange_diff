using System;
using System.CodeDom.Compiler;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.Search
{
	[GeneratedCode("microsoft.search.platform.parallax.tools.codegenerator.exe", "1.0.0.0")]
	public interface IMemoryModelSettings : ISettings
	{
		float MemoryUsageAdjustmentMultiplier { get; }

		int SandboxMaxPoolSize { get; }

		int LowAvailableSystemWorkingSetMemoryRatio { get; }

		long SearchMemoryModelBaseCost { get; }

		long BaselineCostPerActiveItem { get; }

		long BaselineCostPerPassiveItem { get; }

		long InstantSearchCostPerActiveItem { get; }

		long RefinersCostPerActiveItem { get; }

		bool DisableGracefulDegradationForInstantSearch { get; }

		bool DisableGracefulDegradationForAutoSuspend { get; }

		int TimerForGracefulDegradation { get; }

		long MemoryMeasureDrift { get; }

		long MaxRestoreAmount { get; }

		bool ShouldConsiderSearchMemoryUsageBudget { get; }

		long SearchMemoryUsageBudget { get; }

		long SearchMemoryUsageBudgetFloatingAmount { get; }
	}
}
