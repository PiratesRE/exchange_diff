using System;
using System.Runtime.CompilerServices;
using System.Threading;
using Microsoft.Exchange.Compliance.TaskDistributionCommon.Diagnostics;
using Microsoft.Exchange.Compliance.TaskDistributionCommon.Extensibility;
using Microsoft.Exchange.Compliance.TaskDistributionCommon.Protocol;

namespace Microsoft.Exchange.Compliance.TaskDistributionFabric.Utility
{
	internal static class ApplicationHelper
	{
		public static bool TryPreprocess(ComplianceMessage target, WorkPayload workPayload, out WorkPayload processedPayload, out FaultDefinition faultDefinition, [CallerMemberName] string callerMember = null, [CallerFilePath] string callerFilePath = null, [CallerLineNumber] int callerLineNumber = 0)
		{
			processedPayload = null;
			WorkPayload newPayload = null;
			IApplicationPlugin plugin;
			if (Registry.Instance.TryGetInstance<IApplicationPlugin>(RegistryComponent.Application, target.WorkDefinitionType, out plugin, out faultDefinition, "TryPreprocess", "f:\\15.00.1497\\sources\\dev\\EDiscovery\\src\\TaskDistributionSystem\\TaskDistributionFabric\\Utility\\ApplicationHelper.cs", 51))
			{
				ExceptionHandler.Gray.TryRun(delegate
				{
					newPayload = plugin.Preprocess(target, workPayload);
				}, TaskDistributionSettings.ApplicationExecutionTime, out faultDefinition, target, null, default(CancellationToken), null, callerMember, callerFilePath, callerLineNumber);
				if (ExceptionHandler.IsFaulted(target))
				{
					faultDefinition = ExceptionHandler.GetFaultDefinition(target);
				}
			}
			processedPayload = newPayload;
			return processedPayload != null && faultDefinition == null;
		}

		public static bool TryDoWork(ComplianceMessage target, WorkPayload workPayload, out WorkPayload resultPayload, out FaultDefinition faultDefinition, [CallerMemberName] string callerMember = null, [CallerFilePath] string callerFilePath = null, [CallerLineNumber] int callerLineNumber = 0)
		{
			resultPayload = null;
			WorkPayload newPayload = null;
			IApplicationPlugin plugin;
			if (Registry.Instance.TryGetInstance<IApplicationPlugin>(RegistryComponent.Application, target.WorkDefinitionType, out plugin, out faultDefinition, "TryDoWork", "f:\\15.00.1497\\sources\\dev\\EDiscovery\\src\\TaskDistributionSystem\\TaskDistributionFabric\\Utility\\ApplicationHelper.cs", 104))
			{
				ExceptionHandler.Gray.TryRun(delegate
				{
					newPayload = plugin.DoWork(target, workPayload);
				}, TaskDistributionSettings.ApplicationExecutionTime, out faultDefinition, target, null, default(CancellationToken), null, callerMember, callerFilePath, callerLineNumber);
				if (ExceptionHandler.IsFaulted(target))
				{
					faultDefinition = ExceptionHandler.GetFaultDefinition(target);
				}
			}
			resultPayload = newPayload;
			return resultPayload != null && faultDefinition == null;
		}

		public static bool TryRecordResult(ComplianceMessage target, ResultBase existingResult, WorkPayload workPayload, out ResultBase processedResult, out FaultDefinition faultDefinition, [CallerMemberName] string callerMember = null, [CallerFilePath] string callerFilePath = null, [CallerLineNumber] int callerLineNumber = 0)
		{
			processedResult = null;
			ResultBase newResult = null;
			IApplicationPlugin plugin;
			if (Registry.Instance.TryGetInstance<IApplicationPlugin>(RegistryComponent.Application, target.WorkDefinitionType, out plugin, out faultDefinition, "TryRecordResult", "f:\\15.00.1497\\sources\\dev\\EDiscovery\\src\\TaskDistributionSystem\\TaskDistributionFabric\\Utility\\ApplicationHelper.cs", 159))
			{
				ExceptionHandler.Gray.TryRun(delegate
				{
					newResult = plugin.RecordResult(existingResult, workPayload);
				}, TaskDistributionSettings.ApplicationExecutionTime, out faultDefinition, target, null, default(CancellationToken), null, callerMember, callerFilePath, callerLineNumber);
			}
			processedResult = newResult;
			return processedResult != null && faultDefinition == null;
		}
	}
}
