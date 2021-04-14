using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Performance;

namespace Microsoft.Exchange.Data.HA.DirectoryServices
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal static class ADUtils
	{
		public static Exception RunADOperation(Action adOperation, int retryCount = 2)
		{
			Exception result = null;
			ADOperationResult adoperationResult = ADNotificationAdapter.TryRunADOperation(delegate()
			{
				adOperation();
			}, retryCount);
			if (!adoperationResult.Succeeded)
			{
				result = adoperationResult.Exception;
			}
			return result;
		}

		public static Exception RunADOperation(Action adOperation, IPerformanceDataLogger perfLogger, int retryCount = 2)
		{
			Exception result = null;
			string marker = "ADQuery";
			using (new StopwatchPerformanceTracker(marker, perfLogger))
			{
				result = ADUtils.RunADOperation(adOperation, retryCount);
			}
			return result;
		}
	}
}
