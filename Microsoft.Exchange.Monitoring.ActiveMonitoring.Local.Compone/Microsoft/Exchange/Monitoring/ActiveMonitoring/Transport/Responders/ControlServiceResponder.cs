using System;
using System.Diagnostics;
using System.Reflection;
using System.Threading;
using Microsoft.Exchange.Diagnostics.Audit;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.Monitoring.ActiveMonitoring.Recovery;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Transport.Responders
{
	public class ControlServiceResponder : ResponderWorkItem
	{
		internal string WindowsServiceName { get; set; }

		internal int ControlCode { get; set; }

		internal static ResponderDefinition CreateDefinition(string responderName, string monitorName, string windowsServiceName, ServiceHealthStatus responderTargetState, int controlCode, string throttleGroupName = null)
		{
			ResponderDefinition responderDefinition = new ResponderDefinition();
			responderDefinition.AssemblyPath = ControlServiceResponder.AssemblyPath;
			responderDefinition.TypeName = ControlServiceResponder.TypeName;
			responderDefinition.Name = responderName;
			responderDefinition.ServiceName = "Exchange";
			responderDefinition.AlertTypeId = "*";
			responderDefinition.AlertMask = monitorName;
			responderDefinition.RecurrenceIntervalSeconds = 300;
			responderDefinition.TimeoutSeconds = 300;
			responderDefinition.MaxRetryAttempts = 3;
			responderDefinition.TargetHealthState = responderTargetState;
			responderDefinition.WaitIntervalSeconds = 30;
			responderDefinition.Enabled = true;
			responderDefinition.Attributes["WindowsServiceName"] = windowsServiceName;
			responderDefinition.Attributes["ControlCode"] = controlCode.ToString();
			RecoveryActionRunner.SetThrottleProperties(responderDefinition, throttleGroupName, RecoveryActionId.ControlService, windowsServiceName, null);
			return responderDefinition;
		}

		protected override void DoResponderWork(CancellationToken cancellationToken)
		{
			this.InitializeAttributes();
			RecoveryActionRunner recoveryActionRunner = new RecoveryActionRunner(RecoveryActionId.ControlService, this.WindowsServiceName, this, true, cancellationToken, null);
			recoveryActionRunner.Execute(delegate()
			{
				this.InternalControlService(cancellationToken);
			});
		}

		private void InitializeServiceAttributes(AttributeHelper attributeHelper)
		{
			this.WindowsServiceName = attributeHelper.GetString("WindowsServiceName", true, null);
			this.ControlCode = attributeHelper.GetInt("ControlCode", true, -1, null, null);
		}

		private void InitializeAttributes()
		{
			AttributeHelper attributeHelper = new AttributeHelper(base.Definition);
			this.InitializeServiceAttributes(attributeHelper);
		}

		private void InternalControlService(CancellationToken cancellationToken)
		{
			Privilege.RunWithPrivilege("SeDebugPrivilege", true, delegate
			{
				using (ServiceHelper serviceHelper = new ServiceHelper(this.WindowsServiceName, cancellationToken))
				{
					using (Process process = serviceHelper.GetProcess())
					{
						if (process == null)
						{
							return;
						}
					}
					serviceHelper.ControlService(this.ControlCode);
				}
			});
		}

		private static readonly string AssemblyPath = Assembly.GetExecutingAssembly().Location;

		private static readonly string TypeName = typeof(ControlServiceResponder).FullName;

		internal static class AttributeNames
		{
			internal const string WindowsServiceName = "WindowsServiceName";

			internal const string ControlCode = "ControlCode";

			internal const string ThrottleGroupName = "ThrottleGroupName";
		}

		internal static class DefaultValues
		{
			internal const string ThrottleGroupName = "";
		}
	}
}
