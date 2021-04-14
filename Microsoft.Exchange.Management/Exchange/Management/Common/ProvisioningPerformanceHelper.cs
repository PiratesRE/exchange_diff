using System;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.LatencyDetection;
using Microsoft.Exchange.Live.DomainServices;
using Microsoft.Exchange.Management.LiveServices;
using Microsoft.Mapi.Unmanaged;

namespace Microsoft.Exchange.Management.Common
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal static class ProvisioningPerformanceHelper
	{
		internal static LatencyDetectionContext StartLatencyDetection(Task task)
		{
			IPerformanceDataProvider[] providers = new IPerformanceDataProvider[]
			{
				PerformanceContext.Current,
				RpcDataProvider.Instance,
				TaskPerformanceData.CmdletInvoked,
				TaskPerformanceData.BeginProcessingInvoked,
				TaskPerformanceData.ProcessRecordInvoked,
				TaskPerformanceData.EndProcessingInvoked,
				DomainServicesPerformanceData.DomainServicesConnection,
				DomainServicesPerformanceData.DomainServicesCall,
				LiveServicesPerformanceData.SPFConnection,
				LiveServicesPerformanceData.SPFCall,
				LiveServicesPerformanceData.CredentialServicesCall,
				LiveServicesPerformanceData.ProfileServicesCall,
				LiveServicesPerformanceData.NamespaceServicesCall
			};
			return ProvisioningPerformanceHelper.latencyDetectionContextFactory.CreateContext(ProvisioningPerformanceHelper.applicationVersion, task.CurrentTaskContext.InvocationInfo.CommandName, providers);
		}

		internal static TaskPerformanceData[] StopLatencyDetection(LatencyDetectionContext latencyDetectionContext)
		{
			TaskPerformanceData[] result = null;
			if (latencyDetectionContext != null)
			{
				result = latencyDetectionContext.StopAndFinalizeCollection();
			}
			return result;
		}

		private static readonly string applicationVersion = typeof(ProvisioningPerformanceHelper).GetApplicationVersion();

		private static readonly LatencyDetectionContextFactory latencyDetectionContextFactory = LatencyDetectionContextFactory.CreateFactory("Provisioning.Cmdlets", LatencyReportingThreshold.MinimumThresholdValue, TimeSpan.FromSeconds(20.0));
	}
}
