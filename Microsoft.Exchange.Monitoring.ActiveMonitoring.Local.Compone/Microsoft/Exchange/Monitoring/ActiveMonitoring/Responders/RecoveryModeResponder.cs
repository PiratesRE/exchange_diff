using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Exchange.Monitoring.ServiceContextProvider;
using Microsoft.Forefront.RecoveryActionArbiter.Contract;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.Monitoring.ActiveMonitoring.Recovery;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Responders
{
	public class RecoveryModeResponder : ResponderWorkItem
	{
		internal string RecoveryID { get; set; }

		internal ArbitrationScope ArbitrationScope { get; set; }

		internal ArbitrationSource ArbitrationSource { get; set; }

		internal RequestedAction RequestedAction { get; set; }

		internal RecoveryFlags RecoveryFlags { get; set; }

		internal string FailureReason { get; set; }

		internal ServiceHealthStatus EnterRecoveryModeTargetHealthState { get; set; }

		internal ServiceHealthStatus ExitRecoveryModeTargetHealthState { get; set; }

		public static ResponderDefinition CreateDefinition(string name, string serviceName, string targetResource, string alertMask, string alertTypeId, string recoveryID, ServiceHealthStatus enterRecoveryModeTargetHealthState, ServiceHealthStatus exitRecoveryModeTargetHealthState, ArbitrationScope? arbitrationScope, ArbitrationSource? arbitrationSource, RequestedAction? requestedAction, RecoveryFlags? recoveryFlags, string failureReason)
		{
			ResponderDefinition responderDefinition = new ResponderDefinition();
			responderDefinition.AssemblyPath = typeof(RecoveryModeResponder).Assembly.Location;
			responderDefinition.TypeName = typeof(RecoveryModeResponder).FullName;
			responderDefinition.Name = name;
			responderDefinition.ServiceName = serviceName;
			responderDefinition.TargetResource = targetResource;
			responderDefinition.AlertMask = alertMask;
			responderDefinition.AlertTypeId = alertTypeId;
			responderDefinition.TargetHealthState = ServiceHealthStatus.None;
			responderDefinition.Attributes["RecoveryID"] = recoveryID;
			responderDefinition.Attributes["EnterRecoveryModeTargetHealthState"] = enterRecoveryModeTargetHealthState.ToString();
			responderDefinition.Attributes["ExitRecoveryModeTargetHealthState"] = exitRecoveryModeTargetHealthState.ToString();
			if (arbitrationScope != null)
			{
				responderDefinition.Attributes["ArbitrationScope"] = arbitrationScope.ToString();
			}
			if (arbitrationSource != null)
			{
				responderDefinition.Attributes["ArbitrationSource"] = arbitrationSource.ToString();
			}
			if (requestedAction != null)
			{
				responderDefinition.Attributes["RequestedAction"] = requestedAction.ToString();
			}
			if (recoveryFlags != null)
			{
				responderDefinition.Attributes["RecoveryFlags"] = recoveryFlags.ToString();
			}
			responderDefinition.Attributes["FailureReason"] = failureReason;
			return responderDefinition;
		}

		protected virtual void InitializeAttributes(AttributeHelper attributeHelper = null)
		{
			if (attributeHelper == null)
			{
				attributeHelper = new AttributeHelper(base.Definition);
			}
			this.EnterRecoveryModeTargetHealthState = attributeHelper.GetEnum<ServiceHealthStatus>("EnterRecoveryModeTargetHealthState", false, ServiceHealthStatus.None);
			this.ExitRecoveryModeTargetHealthState = attributeHelper.GetEnum<ServiceHealthStatus>("ExitRecoveryModeTargetHealthState", false, ServiceHealthStatus.None);
			if (this.EnterRecoveryModeTargetHealthState == ServiceHealthStatus.None && this.ExitRecoveryModeTargetHealthState == ServiceHealthStatus.None)
			{
				throw new ArgumentException("EnterRecoveryModeTargetHealthState and ExitRecoveryModeTargetHealthState cannot both be set to \"None.\"");
			}
			bool flag = this.EnterRecoveryModeTargetHealthState != ServiceHealthStatus.None;
			this.RecoveryID = attributeHelper.GetString("RecoveryID", true, "");
			if (this.RecoveryID.Equals(string.Empty))
			{
				throw new ArgumentException("RecoveryID is a mandatory attribute and cannot be empty.");
			}
			this.ArbitrationScope = attributeHelper.GetEnum<ArbitrationScope>("ArbitrationScope", flag, 0);
			this.ArbitrationSource = attributeHelper.GetEnum<ArbitrationSource>("ArbitrationSource", flag, 0);
			this.RequestedAction = attributeHelper.GetEnum<RequestedAction>("RequestedAction", flag, 2);
			this.RecoveryFlags = attributeHelper.GetEnum<RecoveryFlags>("RecoveryFlags", flag, 0);
			this.FailureReason = attributeHelper.GetString("FailureReason", flag, "");
			if (flag && this.FailureReason.Equals(string.Empty))
			{
				throw new ArgumentException("FailureReason is a mandatory attribute and cannot be empty.");
			}
		}

		protected override void DoResponderWork(CancellationToken cancellationToken)
		{
			this.InitializeAttributes(null);
			Task<MonitorResult> lastSuccessfulMonitorResult = base.GetLastSuccessfulMonitorResult(cancellationToken);
			if (lastSuccessfulMonitorResult != null)
			{
				lastSuccessfulMonitorResult.Continue(delegate(MonitorResult lastMonitorResult)
				{
					bool flag = ServiceContextProvider.RecoveryRequestExists(this.RecoveryID);
					if (lastMonitorResult != null)
					{
						if (lastMonitorResult.HealthState == this.EnterRecoveryModeTargetHealthState)
						{
							ServiceContextProvider.Instance.RequestApprovalForRecovery(this.RecoveryID, this.ArbitrationScope, this.ArbitrationSource, this.RequestedAction, this.RecoveryFlags, this.FailureReason, "");
							return;
						}
						if (lastMonitorResult.HealthState == this.ExitRecoveryModeTargetHealthState && flag)
						{
							ServiceContextProvider.Instance.NotifyRecoveryCompletion(this.RecoveryID, true, "");
						}
					}
				}, cancellationToken, TaskContinuationOptions.AttachedToParent | TaskContinuationOptions.NotOnFaulted | TaskContinuationOptions.NotOnCanceled);
			}
		}

		internal static class AttributeNames
		{
			internal const string RecoveryID = "RecoveryID";

			internal const string ArbitrationScope = "ArbitrationScope";

			internal const string ArbitrationSource = "ArbitrationSource";

			internal const string RequestedAction = "RequestedAction";

			internal const string RecoveryFlags = "RecoveryFlags";

			internal const string FailureReason = "FailureReason";

			internal const string EnterRecoveryModeTargetHealthState = "EnterRecoveryModeTargetHealthState";

			internal const string ExitRecoveryModeTargetHealthState = "ExitRecoveryModeTargetHealthState";
		}

		internal static class DefaultValues
		{
			internal const string RecoveryID = "";

			internal const ArbitrationScope ArbitrationScope = 0;

			internal const ArbitrationSource ArbitrationSource = 0;

			internal const RequestedAction RequestedAction = 2;

			internal const RecoveryFlags RecoveryFlags = 0;

			internal const string FailureReason = "";

			internal const ServiceHealthStatus EnterRecoveryModeTargetHealthState = ServiceHealthStatus.None;

			internal const ServiceHealthStatus ExitRecoveryModeTargetHealthState = ServiceHealthStatus.None;
		}
	}
}
