using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring;
using Microsoft.Exchange.MailboxAssistants.Assistants;
using Microsoft.Exchange.MailboxAssistants.Assistants.MailboxProcessor;
using Microsoft.Exchange.MailboxAssistants.Assistants.MailboxProcessor.MailboxProcessorDefinitions;
using Microsoft.Exchange.MailboxAssistants.Assistants.MailboxProcessor.MailboxProcessorHelpers;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Migration.Responders
{
	public class MailboxLockedResponder : ResponderWorkItem
	{
		public static ResponderDefinition CreateDefinition(string responderName, string targetResource, string alertTypeId, string alertMask, int recurrenceInterval, Component component, ServiceHealthStatus targetHealthState)
		{
			return new ResponderDefinition
			{
				AssemblyPath = Assembly.GetExecutingAssembly().Location,
				TypeName = typeof(MailboxLockedResponder).FullName,
				Name = responderName,
				TargetResource = targetResource,
				RecurrenceIntervalSeconds = recurrenceInterval,
				AlertMask = alertMask,
				AlertTypeId = alertTypeId,
				ServiceName = component.Name,
				TargetHealthState = targetHealthState,
				TimeoutSeconds = 100,
				WaitIntervalSeconds = 1,
				MaxRetryAttempts = 1
			};
		}

		protected override void DoResponderWork(CancellationToken cancellationToken)
		{
			this.GetMailboxInfoFromProbe(cancellationToken);
			foreach (MailboxProcessorNotificationEntry mailboxProcessorInfo in this.lockedMailboxes)
			{
				this.DoPerMailboxResponderWork(mailboxProcessorInfo);
			}
		}

		private void GetMailboxInfoFromProbe(CancellationToken cancellationToken)
		{
			List<ProbeResult> list = new List<ProbeResult>();
			LocalDataAccess localDataAccess = new LocalDataAccess();
			string resultName = NotificationItem.GenerateResultName(ExchangeComponent.MailboxMigration.Name, ExchangeComponent.MailboxMigration.Name, "MailboxLocked");
			int num = Math.Max(base.Definition.RecurrenceIntervalSeconds * 10, (int)AssistantConfiguration.MailboxProcessorWorkCycle.Read().TotalSeconds);
			DateTime d = DateTime.UtcNow.ToLocalTime();
			DateTime startTime = d - TimeSpan.FromSeconds((double)num);
			IOrderedEnumerable<ProbeResult> query = from r in localDataAccess.GetTable<ProbeResult, string>(WorkItemResultIndex<ProbeResult>.ResultNameAndExecutionEndTime(resultName, startTime))
			where r.ResultName.Contains(resultName) && r.ExecutionEndTime >= startTime && r.IsNotified
			orderby r.ExecutionStartTime descending
			select r;
			IDataAccessQuery<ProbeResult> dataAccessQuery = base.Broker.AsDataAccessQuery<ProbeResult>(query);
			dataAccessQuery.ExecuteAsync(new Action<ProbeResult>(list.Add), cancellationToken, base.TraceContext).Wait(cancellationToken);
			foreach (ProbeResult probeResult in list)
			{
				MailboxProcessorNotificationEntry item;
				try
				{
					item = new MailboxProcessorNotificationEntry(probeResult);
				}
				catch (FailedToReadProbeResultException ex)
				{
					WTFDiagnostics.TraceError<string>(ExTraceGlobals.MigrationTracer, base.TraceContext, "MailboxLockedResponder was unable to read probe results. Error: {0}", ex.Message, null, "GetMailboxInfoFromProbe", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Migration\\MailboxLockedResponder.cs", 154);
					continue;
				}
				this.lockedMailboxes.Add(item);
			}
		}

		private void DoPerMailboxResponderWork(MailboxProcessorNotificationEntry mailboxProcessorInfo)
		{
			ADUser aduserFromMailboxGuid;
			try
			{
				aduserFromMailboxGuid = ADHelper.GetADUserFromMailboxGuid(mailboxProcessorInfo.MailboxGuid, mailboxProcessorInfo.ExternalDirectoryOrganizationId);
			}
			catch (ADTransientException)
			{
				return;
			}
			if (UnlockMoveTargetUtil.IsMailboxLocked(this.serverFQDN, mailboxProcessorInfo.DatabaseGuid, mailboxProcessorInfo.MailboxGuid))
			{
				if (UnlockMoveTargetUtil.IsValidLockedStatus(aduserFromMailboxGuid.MailboxMoveStatus))
				{
					return;
				}
				try
				{
					UnlockMoveTargetUtil.UnlockMoveTarget(this.serverFQDN, mailboxProcessorInfo.DatabaseGuid, mailboxProcessorInfo.MailboxGuid, aduserFromMailboxGuid.OrganizationId);
				}
				catch (Exception ex)
				{
					if (mailboxProcessorInfo.IssueDetectedCount > 2)
					{
						this.PublishOnCallAlert(aduserFromMailboxGuid, new string[]
						{
							ex.Message,
							ex.StackTrace
						});
					}
					throw;
				}
				return;
			}
		}

		private void PublishOnCallAlert(ADUser mailbox, params string[] args)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine(string.Format("MailboxIdentity: {0}\\{1}", mailbox.OrganizationId.OrganizationalUnit.Name, mailbox.Identity));
			stringBuilder.AppendLine(string.Format("MailboxGuid: {0}", mailbox.Guid));
			stringBuilder.AppendLine(string.Format("Database: {0}", mailbox.Database));
			stringBuilder.AppendLine(string.Format("OrganizationId: {0}", mailbox.OrganizationId));
			foreach (string value in args)
			{
				stringBuilder.AppendLine(value);
			}
			EventNotificationItem eventNotificationItem = new EventNotificationItem(ExchangeComponent.MailboxMigration.Name, ExchangeComponent.MailboxMigration.Name, "MailboxCannotBeUnlocked", stringBuilder.ToString(), ResultSeverityLevel.Error);
			eventNotificationItem.Publish(false);
		}

		private readonly List<MailboxProcessorNotificationEntry> lockedMailboxes = new List<MailboxProcessorNotificationEntry>();

		private readonly string serverFQDN = LocalServer.GetServer().Fqdn;
	}
}
