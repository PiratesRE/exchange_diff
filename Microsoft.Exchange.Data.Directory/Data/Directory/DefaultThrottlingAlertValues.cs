using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics.Components.Data.Directory;

namespace Microsoft.Exchange.Data.Directory
{
	internal static class DefaultThrottlingAlertValues
	{
		public static int MassUserOverBudgetPercent(BudgetType budgetType)
		{
			return DefaultThrottlingAlertValues.SafeGetValueFromMap(DefaultThrottlingAlertValues.massUserOverBudgetPercentMap, budgetType, 50);
		}

		public static int DelayTimeThreshold(BudgetType budgetType)
		{
			return DefaultThrottlingAlertValues.SafeGetValueFromMap(DefaultThrottlingAlertValues.delayTimeThresholdMap, budgetType, 10000);
		}

		private static int SafeGetValueFromMap(Dictionary<BudgetType, int> map, BudgetType budgetType, int defaultValue)
		{
			int result;
			if (!map.TryGetValue(budgetType, out result))
			{
				ExTraceGlobals.ClientThrottlingTracer.TraceDebug<int, BudgetType>(0L, "[DefaultThrottlingAlertValues::SafeGetValueFromMap] Using default alert value of {0} for budgetType {1}.", defaultValue, budgetType);
				result = defaultValue;
			}
			return result;
		}

		private const int DefaultMassUserOverBudgetPercent = 50;

		private const int DefaultDelayTimeThreshold = 10000;

		private static Dictionary<BudgetType, int> massUserOverBudgetPercentMap = new Dictionary<BudgetType, int>();

		private static Dictionary<BudgetType, int> delayTimeThresholdMap = new Dictionary<BudgetType, int>();
	}
}
