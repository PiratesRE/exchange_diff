using System;
using System.Reflection;
using System.Threading;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring;
using Microsoft.Exchange.HA.ManagedAvailability;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Common;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Local;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.Monitoring.ActiveMonitoring.Recovery;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Responders
{
	public class DatabaseFailoverResponder : ResponderWorkItem
	{
		internal string ComponentName { get; private set; }

		internal string DatabaseGuidString { get; private set; }

		internal static ResponderDefinition CreateDefinition(string name, string serviceName, string alertTypeId, string alertMask, string targetResource, string componentName, Guid databaseGuid, ServiceHealthStatus targetHealthState)
		{
			ResponderDefinition responderDefinition = new ResponderDefinition();
			responderDefinition.AssemblyPath = DatabaseFailoverResponder.assemblyPath;
			responderDefinition.TypeName = DatabaseFailoverResponder.databaseFailoverResponderTypeName;
			responderDefinition.Name = name;
			responderDefinition.ServiceName = serviceName;
			responderDefinition.AlertTypeId = alertTypeId;
			responderDefinition.AlertMask = alertMask;
			responderDefinition.TargetResource = targetResource;
			responderDefinition.TargetHealthState = targetHealthState;
			responderDefinition.TargetExtension = databaseGuid.ToString();
			responderDefinition.Attributes["ComponentName"] = componentName;
			responderDefinition.RecurrenceIntervalSeconds = 300;
			responderDefinition.WaitIntervalSeconds = 30;
			responderDefinition.TimeoutSeconds = 150;
			responderDefinition.MaxRetryAttempts = 3;
			responderDefinition.Enabled = true;
			RecoveryActionRunner.SetThrottleProperties(responderDefinition, "Dag", RecoveryActionId.DatabaseFailover, databaseGuid.ToString(), null);
			return responderDefinition;
		}

		internal virtual void InitializeAttributes()
		{
			AttributeHelper attributeHelper = new AttributeHelper(base.Definition);
			this.ComponentName = attributeHelper.GetString("ComponentName", true, null);
		}

		protected override void DoResponderWork(CancellationToken cancellationToken)
		{
			if (base.Definition.TargetHealthState == ServiceHealthStatus.None)
			{
				throw new InvalidOperationException("DatabaseFailoverResponder can only run as a chained responder with a target health state defined");
			}
			this.InitializeAttributes();
			this.DatabaseGuidString = base.Definition.TargetExtension;
			base.Result.StateAttribute1 = this.DatabaseGuidString;
			if (string.IsNullOrWhiteSpace(this.DatabaseGuidString))
			{
				base.Result.StateAttribute2 = "DatabaseGuidNotSupplied";
				WTFDiagnostics.TraceDebug(ExTraceGlobals.CommonComponentsTracer, base.TraceContext, "DatabaseFailoverResponder.DoWork: Unable to perform database failover as database Guid is not supplied", null, "DoResponderWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Common\\Responders\\DatabaseFailoverResponder.cs", 145);
				throw new DatabaseGuidNotSuppliedException();
			}
			Guid databaseGuid = new Guid(this.DatabaseGuidString);
			if (DirectoryAccessor.Instance.IsDatabaseCopyActiveOnLocalServer(databaseGuid))
			{
				RecoveryActionRunner recoveryActionRunner = new RecoveryActionRunner(RecoveryActionId.DatabaseFailover, this.DatabaseGuidString, this, true, cancellationToken, null);
				recoveryActionRunner.Execute(delegate()
				{
					this.InitiateDatabaseFailover(databaseGuid);
				});
				return;
			}
			base.Result.StateAttribute2 = "SkippingFailoverPassiveDatabase";
			WTFDiagnostics.TraceDebug(ExTraceGlobals.CommonComponentsTracer, base.TraceContext, "DatabaseFailoverResponder.DoWork: Skipping database failover as database is already passive on local server", null, "DoResponderWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Common\\Responders\\DatabaseFailoverResponder.cs", 174);
		}

		private void InitiateDatabaseFailover(Guid databaseGuid)
		{
			Component component = Component.FindWellKnownComponent(this.ComponentName);
			if (component != null)
			{
				WTFDiagnostics.TraceDebug<string>(ExTraceGlobals.CommonComponentsTracer, base.TraceContext, "DatabaseFailoverResponder.DoWork: Attempting to perform database failover (componentName={0})", component.ToString(), null, "InitiateDatabaseFailover", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Common\\Responders\\DatabaseFailoverResponder.cs", 193);
				string comment = string.Format("Managed availability database failover initiated by Responder={0} Component={1}.", base.Definition.Name, component.ToString());
				MailboxDatabase mailboxDatabaseFromGuid = DirectoryAccessor.Instance.GetMailboxDatabaseFromGuid(databaseGuid);
				ManagedAvailabilityHelper.PerformDatabaseFailover(component.ToString(), comment, mailboxDatabaseFromGuid);
				return;
			}
			throw new InvalidOperationException(string.Format("{0} is not a well known exchange component", this.ComponentName));
		}

		private static readonly string assemblyPath = Assembly.GetExecutingAssembly().Location;

		private static readonly string databaseFailoverResponderTypeName = typeof(DatabaseFailoverResponder).FullName;

		internal static class AttributeNames
		{
			internal const string ComponentName = "ComponentName";
		}

		internal class DefaultValues
		{
			internal const int RecurrenceIntervalSeconds = 300;

			internal const int WaitIntervalSeconds = 30;

			internal const int TimeoutSeconds = 150;

			internal const int MaxRetryAttempts = 3;
		}
	}
}
