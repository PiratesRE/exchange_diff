using System;
using System.Reflection;
using System.Threading;
using Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Common;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.Monitoring.ActiveMonitoring.Recovery;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Directory
{
	public class PutDCInMMResponder : ResponderWorkItem
	{
		internal string TargetServerFQDN { get; set; }

		internal bool IgnoreGroupThrottlingWhenMajorityNotSucceeded { get; set; }

		public static ResponderDefinition CreateDefinition(string name, string serviceName, string alertTypeId, string alertMask, string targetServer, ServiceHealthStatus targetHealthState, bool isIgnoreGroupThrottlingWhenMajorityNotSucceeded = false, string throttleGroupName = "DomainController")
		{
			ResponderDefinition responderDefinition = new ResponderDefinition();
			responderDefinition.AssemblyPath = PutDCInMMResponder.AssemblyPath;
			responderDefinition.TypeName = PutDCInMMResponder.TypeName;
			responderDefinition.Name = name;
			responderDefinition.ServiceName = serviceName;
			responderDefinition.AlertTypeId = alertTypeId;
			responderDefinition.AlertMask = alertMask;
			responderDefinition.TargetResource = targetServer;
			responderDefinition.TargetHealthState = targetHealthState;
			responderDefinition.RecurrenceIntervalSeconds = 1200;
			responderDefinition.WaitIntervalSeconds = 60;
			responderDefinition.TimeoutSeconds = 600;
			responderDefinition.MaxRetryAttempts = 3;
			responderDefinition.Enabled = true;
			responderDefinition.StartTime = DateTime.UtcNow;
			responderDefinition.Attributes["TargetServerFQDN"] = targetServer;
			responderDefinition.Attributes["IgnoreGroupThrottling"] = isIgnoreGroupThrottlingWhenMajorityNotSucceeded.ToString();
			string resourceName = string.Empty;
			if (string.IsNullOrEmpty(targetServer))
			{
				resourceName = DirectoryGeneralUtils.GetLocalFQDN();
			}
			else
			{
				resourceName = targetServer;
			}
			RecoveryActionRunner.SetThrottleProperties(responderDefinition, throttleGroupName, RecoveryActionId.PutDCInMM, resourceName, null);
			return responderDefinition;
		}

		protected void InitializeAttributes(AttributeHelper attributeHelper)
		{
			this.TargetServerFQDN = attributeHelper.GetString("TargetServerFQDN", true, DirectoryGeneralUtils.GetLocalFQDN());
			this.IgnoreGroupThrottlingWhenMajorityNotSucceeded = attributeHelper.GetBool("IgnoreGroupThrottling", false, false);
		}

		protected virtual void InitializeAttributes()
		{
			AttributeHelper attributeHelper = new AttributeHelper(base.Definition);
			this.InitializeAttributes(attributeHelper);
		}

		protected override void DoResponderWork(CancellationToken cancellationToken)
		{
			WTFDiagnostics.TraceInformation(ExTraceGlobals.DirectoryTracer, base.TraceContext, "Inside PuttingDCInMMResponder::DoResponderWork.", null, "DoResponderWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Directory\\PuttingDCInMMResponder.cs", 154);
			this.InitializeAttributes();
			string resourceName = string.Empty;
			if (string.IsNullOrEmpty(this.TargetServerFQDN))
			{
				resourceName = DirectoryGeneralUtils.GetLocalFQDN();
			}
			else
			{
				resourceName = this.TargetServerFQDN;
			}
			RecoveryActionRunner recoveryActionRunner = new RecoveryActionRunner(RecoveryActionId.PutDCInMM, resourceName, this, true, cancellationToken, null);
			recoveryActionRunner.IsIgnoreResourceName = true;
			recoveryActionRunner.IgnoreGroupThrottlingWhenMajorityNotSucceeded = this.IgnoreGroupThrottlingWhenMajorityNotSucceeded;
			WTFDiagnostics.TraceInformation(ExTraceGlobals.DirectoryTracer, base.TraceContext, string.Format("Putting DC {0} in MM.", this.TargetServerFQDN), null, "DoResponderWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Directory\\PuttingDCInMMResponder.cs", 184);
			recoveryActionRunner.Execute(delegate()
			{
				this.DirectoryPutDCInMM(this.TargetServerFQDN);
			});
			WTFDiagnostics.TraceInformation(ExTraceGlobals.DirectoryTracer, base.TraceContext, "Putting DC into MM is Completed", null, "DoResponderWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Directory\\PuttingDCInMMResponder.cs", 191);
		}

		private void DirectoryPutDCInMM(string targetFqdn)
		{
			string stateAttribute = DirectoryGeneralUtils.InternalPutDCInMM(targetFqdn, base.TraceContext);
			base.Result.StateAttribute1 = stateAttribute;
		}

		private static readonly string AssemblyPath = Assembly.GetExecutingAssembly().Location;

		private static readonly string TypeName = typeof(PutDCInMMResponder).FullName;

		internal static class AttributeNames
		{
			internal const string TargetServerFQDN = "TargetServerFQDN";

			internal const string throttleGroupName = "throttleGroupName";

			internal const string IgnoreGroupThrottlingWhenMajorityNotSucceeded = "IgnoreGroupThrottling";
		}
	}
}
