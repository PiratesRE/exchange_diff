using System;
using System.Reflection;
using System.Threading;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.Monitoring.ActiveMonitoring.Recovery;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Search.Responders
{
	public class RestartHostControllerServiceResponder2 : RestartServiceResponder
	{
		internal static ResponderDefinition CreateDefinition(string responderName, string alertMask, ServiceHealthStatus responderTargetState, int serviceStopTimeoutInSeconds = 15, int serviceStartTimeoutInSeconds = 120, int serviceStartDelayInSeconds = 0, DumpMode dumpOnRestartMode = DumpMode.None, string dumpPath = null, double minimumFreeDiskPercent = 15.0, int maximumDumpDurationInSeconds = 0, string throttleGroupName = null)
		{
			ResponderDefinition responderDefinition = new ResponderDefinition();
			responderDefinition.AssemblyPath = RestartHostControllerServiceResponder2.AssemblyPath;
			responderDefinition.TypeName = RestartHostControllerServiceResponder2.TypeName;
			responderDefinition.Name = responderName;
			responderDefinition.ServiceName = ExchangeComponent.Search.Name;
			responderDefinition.AlertTypeId = "*";
			responderDefinition.AlertMask = alertMask;
			responderDefinition.RecurrenceIntervalSeconds = 0;
			responderDefinition.TimeoutSeconds = 600;
			responderDefinition.MaxRetryAttempts = 3;
			responderDefinition.TargetHealthState = responderTargetState;
			responderDefinition.WaitIntervalSeconds = 30;
			responderDefinition.Enabled = true;
			if (string.IsNullOrEmpty(dumpPath))
			{
				dumpPath = RestartServiceResponder.DefaultValues.DumpPath;
			}
			responderDefinition.Attributes["WindowsServiceName"] = "HostControllerService";
			responderDefinition.Attributes["ServiceStopTimeout"] = TimeSpan.FromSeconds((double)serviceStopTimeoutInSeconds).ToString();
			responderDefinition.Attributes["ServiceStartTimeout"] = TimeSpan.FromSeconds((double)serviceStartTimeoutInSeconds).ToString();
			responderDefinition.Attributes["ServiceStartDelay"] = TimeSpan.FromSeconds((double)serviceStartDelayInSeconds).ToString();
			responderDefinition.Attributes["IsMasterAndWorker"] = true.ToString();
			responderDefinition.Attributes["DumpOnRestart"] = dumpOnRestartMode.ToString();
			responderDefinition.Attributes["DumpPath"] = dumpPath;
			responderDefinition.Attributes["MinimumFreeDiskPercent"] = minimumFreeDiskPercent.ToString();
			responderDefinition.Attributes["MaximumDumpDurationInSeconds"] = maximumDumpDurationInSeconds.ToString();
			responderDefinition.Attributes["AdditionalProcessNameToKill"] = "NodeRunner";
			RecoveryActionRunner.SetThrottleProperties(responderDefinition, throttleGroupName, RecoveryActionId.RestartService, "HostControllerService", null);
			return responderDefinition;
		}

		protected override void DoResponderWork(CancellationToken cancellationToken)
		{
			SearchMonitoringHelper.LogRecoveryAction(this, "Invoked.", new object[0]);
			this.InitializeAttributes();
			RecoveryActionRunner recoveryActionRunner = new RecoveryActionRunner(RecoveryActionId.RestartService, base.WindowsServiceName, this, true, cancellationToken, null);
			try
			{
				recoveryActionRunner.Execute(delegate(RecoveryActionEntry startEntry)
				{
					this.InternalRestartService(startEntry, cancellationToken);
				});
			}
			catch (ThrottlingRejectedOperationException)
			{
				SearchMonitoringHelper.LogRecoveryAction(this, "Throttled.", new object[0]);
				throw;
			}
			SearchMonitoringHelper.LogRecoveryAction(this, "Completed.", new object[0]);
		}

		protected override void InitializeAttributes()
		{
			base.InitializeAttributes();
			new AttributeHelper(base.Definition);
		}

		private static readonly string AssemblyPath = Assembly.GetExecutingAssembly().Location;

		private static readonly string TypeName = typeof(RestartHostControllerServiceResponder2).FullName;
	}
}
