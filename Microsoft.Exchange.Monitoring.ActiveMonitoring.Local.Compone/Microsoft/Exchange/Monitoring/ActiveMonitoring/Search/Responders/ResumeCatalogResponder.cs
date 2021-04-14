using System;
using System.Reflection;
using System.Threading;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Common;
using Microsoft.Exchange.Search.Fast;
using Microsoft.Exchange.Search.OperatorSchema;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.Monitoring.ActiveMonitoring.Recovery;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Search.Responders
{
	public class ResumeCatalogResponder : ResponderWorkItem
	{
		internal string DatabaseName { get; set; }

		internal static ResponderDefinition CreateDefinition(string responderName, string targetResource, string alertMask, ServiceHealthStatus responderTargetState, string throttleGroupName = null)
		{
			ResponderDefinition responderDefinition = new ResponderDefinition();
			responderDefinition.AssemblyPath = ResumeCatalogResponder.AssemblyPath;
			responderDefinition.TypeName = ResumeCatalogResponder.TypeName;
			responderDefinition.Name = responderName;
			responderDefinition.TargetResource = targetResource;
			responderDefinition.ServiceName = ExchangeComponent.Search.Name;
			responderDefinition.AlertTypeId = "*";
			responderDefinition.AlertMask = alertMask;
			responderDefinition.RecurrenceIntervalSeconds = 600;
			responderDefinition.TimeoutSeconds = 300;
			responderDefinition.MaxRetryAttempts = 3;
			responderDefinition.TargetHealthState = responderTargetState;
			responderDefinition.WaitIntervalSeconds = 30;
			responderDefinition.Enabled = true;
			RecoveryActionRunner.SetThrottleProperties(responderDefinition, throttleGroupName, RecoveryActionId.ResumeCatalog, targetResource, null);
			return responderDefinition;
		}

		protected virtual void InitializeAttributes()
		{
			new AttributeHelper(base.Definition);
			this.DatabaseName = base.Definition.TargetResource;
		}

		protected override void DoResponderWork(CancellationToken cancellationToken)
		{
			SearchMonitoringHelper.LogRecoveryAction(this, "Invoked.", new object[0]);
			this.InitializeAttributes();
			RecoveryActionRunner recoveryActionRunner = new RecoveryActionRunner(RecoveryActionId.ResumeCatalog, this.DatabaseName, this, true, cancellationToken, null);
			try
			{
				recoveryActionRunner.Execute(delegate(RecoveryActionEntry startEntry)
				{
					this.InternalResumeCatalog(this.DatabaseName, cancellationToken);
				});
			}
			catch (ThrottlingRejectedOperationException)
			{
				SearchMonitoringHelper.LogRecoveryAction(this, "Resuming catalog is throttled.", new object[0]);
				throw;
			}
			SearchMonitoringHelper.LogRecoveryAction(this, "Completed.", new object[0]);
		}

		private void InternalResumeCatalog(string databaseName, CancellationToken cancellationToken)
		{
			MailboxDatabaseInfo databaseInfo = SearchMonitoringHelper.GetDatabaseInfo(databaseName);
			if (databaseInfo == null)
			{
				throw new ArgumentException("databaseName");
			}
			Guid mailboxDatabaseGuid = databaseInfo.MailboxDatabaseGuid;
			string indexSystemName = FastIndexVersion.GetIndexSystemName(mailboxDatabaseGuid);
			if (IndexManager.Instance.ResumeCatalog(indexSystemName))
			{
				IndexManager.Instance.UpdateConfiguration();
			}
		}

		private static readonly string AssemblyPath = Assembly.GetExecutingAssembly().Location;

		private static readonly string TypeName = typeof(ResumeCatalogResponder).FullName;

		internal static class AttributeNames
		{
			internal const string throttleGroupName = "throttleGroupName";
		}
	}
}
