using System;
using System.Reflection;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Office.Datacenter.Monitoring.ActiveMonitoring.Recovery
{
	internal static class StringsRecovery
	{
		public static LocalizedString ArbitrationQuotaCalculationFailed(int exhaustedQuota, int allowedQuota, bool isConcluded, bool isInvokedTooSoon)
		{
			return new LocalizedString("ArbitrationQuotaCalculationFailed", StringsRecovery.ResourceManager, new object[]
			{
				exhaustedQuota,
				allowedQuota,
				isConcluded,
				isInvokedTooSoon
			});
		}

		public static LocalizedString RecoveryActionExceptionCommon(string recoveryActionMsg)
		{
			return new LocalizedString("RecoveryActionExceptionCommon", StringsRecovery.ResourceManager, new object[]
			{
				recoveryActionMsg
			});
		}

		public static LocalizedString ArbitrationMinimumRequiredReadyNotSatisfied(int totalReady, int minimumRequired)
		{
			return new LocalizedString("ArbitrationMinimumRequiredReadyNotSatisfied", StringsRecovery.ResourceManager, new object[]
			{
				totalReady,
				minimumRequired
			});
		}

		public static LocalizedString ThrottlingInProgressException(long instanceId, string actionId, string resourceName, string currentRequester, string inProgressRequester, DateTime operationStartTime, DateTime expectedEndTime)
		{
			return new LocalizedString("ThrottlingInProgressException", StringsRecovery.ResourceManager, new object[]
			{
				instanceId,
				actionId,
				resourceName,
				currentRequester,
				inProgressRequester,
				operationStartTime,
				expectedEndTime
			});
		}

		public static LocalizedString ThrottlingOverlapException(long currentInstanceId, long overlapInstanceId, string currentRequester, string overlapRequester, DateTime currentStartTime, DateTime overlapStartTime)
		{
			return new LocalizedString("ThrottlingOverlapException", StringsRecovery.ResourceManager, new object[]
			{
				currentInstanceId,
				overlapInstanceId,
				currentRequester,
				overlapRequester,
				currentStartTime,
				overlapStartTime
			});
		}

		public static LocalizedString ThrottlingRejectedOperationException(string rejectedOperationMsg)
		{
			return new LocalizedString("ThrottlingRejectedOperationException", StringsRecovery.ResourceManager, new object[]
			{
				rejectedOperationMsg
			});
		}

		public static LocalizedString GroupThrottlingRejectedOperation(string actionId, string resourceName, string requester, string failedChecks)
		{
			return new LocalizedString("GroupThrottlingRejectedOperation", StringsRecovery.ResourceManager, new object[]
			{
				actionId,
				resourceName,
				requester,
				failedChecks
			});
		}

		public static LocalizedString LocalThrottlingRejectedOperation(string actionId, string resourceName, string requester, string failedChecks)
		{
			return new LocalizedString("LocalThrottlingRejectedOperation", StringsRecovery.ResourceManager, new object[]
			{
				actionId,
				resourceName,
				requester,
				failedChecks
			});
		}

		public static LocalizedString ArbitrationExceptionCommon(string arbitrationMsg)
		{
			return new LocalizedString("ArbitrationExceptionCommon", StringsRecovery.ResourceManager, new object[]
			{
				arbitrationMsg
			});
		}

		public static LocalizedString ArbitrationMaximumAllowedConcurrentNotSatisfied(int totalReady, int minimumRequired)
		{
			return new LocalizedString("ArbitrationMaximumAllowedConcurrentNotSatisfied", StringsRecovery.ResourceManager, new object[]
			{
				totalReady,
				minimumRequired
			});
		}

		public static LocalizedString DumpFreeSpaceRequirementNotSatisfied(string directory, double available, double required)
		{
			return new LocalizedString("DumpFreeSpaceRequirementNotSatisfied", StringsRecovery.ResourceManager, new object[]
			{
				directory,
				available,
				required
			});
		}

		public static LocalizedString DistributedThrottlingRejectedOperation(string actionId, string requester)
		{
			return new LocalizedString("DistributedThrottlingRejectedOperation", StringsRecovery.ResourceManager, new object[]
			{
				actionId,
				requester
			});
		}

		private static ExchangeResourceManager ResourceManager = ExchangeResourceManager.GetResourceManager("Microsoft.Office.Datacenter.Monitoring.ActiveMonitoring.Recovery.StringsRecovery", typeof(StringsRecovery).GetTypeInfo().Assembly);

		private enum ParamIDs
		{
			ArbitrationQuotaCalculationFailed,
			RecoveryActionExceptionCommon,
			ArbitrationMinimumRequiredReadyNotSatisfied,
			ThrottlingInProgressException,
			ThrottlingOverlapException,
			ThrottlingRejectedOperationException,
			GroupThrottlingRejectedOperation,
			LocalThrottlingRejectedOperation,
			ArbitrationExceptionCommon,
			ArbitrationMaximumAllowedConcurrentNotSatisfied,
			DumpFreeSpaceRequirementNotSatisfied,
			DistributedThrottlingRejectedOperation
		}
	}
}
