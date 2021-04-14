using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Diagnostics.Components.ForefrontActiveMonitoring;
using Microsoft.Exchange.Hygiene.Data;
using Microsoft.Exchange.Hygiene.Data.MessageTrace;
using Microsoft.Exchange.Hygiene.Data.MessageTrace.Reports;
using Microsoft.Forefront.Monitoring.ActiveMonitoring.Local.Components;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring.Smtp.Probes
{
	internal class MessageTracingMessageProbe : TransportSmtpProbe
	{
		protected override bool VerifyPreviousResults(CancellationToken cancellationToken)
		{
			bool result = false;
			this.clientMessageId = TransportSmtpProbe.GenerateClientMessageId(base.Id, base.SeqNumber - 1L);
			if (MessageTracingMessageProbe.msgTraceSession == null)
			{
				MessageTracingMessageProbe.msgTraceSession = new MessageTraceSession();
			}
			if (base.Result.ExecutionContext == null)
			{
				base.Result.ExecutionContext = string.Empty;
			}
			this.TraceDebug("Started MessageTracingMessageProbe.VerifyPreviousResults.", new object[0]);
			SmtpProbeWorkDefinition.SendMailDefinition sendMail = base.WorkDefinition.SendMail;
			IEnumerable<MessageTrace> enumerable = null;
			try
			{
				Directionality direction = sendMail.Direction;
				Guid guid;
				if (direction == Directionality.Incoming)
				{
					guid = Guid.Parse(sendMail.RecipientTenantID);
				}
				else
				{
					guid = Guid.Parse(sendMail.SenderTenantID);
				}
				DateTime dateTime = DateTime.UtcNow.AddMinutes(-30.0);
				string senderUsername = sendMail.SenderUsername;
				string recipientUsername = sendMail.RecipientUsername;
				if (!SmtpAddress.IsValidSmtpAddress(senderUsername))
				{
					this.TraceDebug("Sender address {0} is invalid", new object[]
					{
						senderUsername
					});
				}
				else if (!SmtpAddress.IsValidSmtpAddress(recipientUsername))
				{
					this.TraceDebug("Recipient address {0} is invalid", new object[]
					{
						recipientUsername
					});
				}
				else
				{
					this.TraceDebug("FindReportObject query parameters 2: TenantId={0}, queryStartTime={1}, QueryEndTime={2}, clientMessageId={3}, sender={4}, recipient={5}", new object[]
					{
						guid,
						dateTime,
						DateTime.UtcNow,
						this.clientMessageId,
						senderUsername,
						recipientUsername
					});
					List<IDisposable> list = new List<IDisposable>();
					try
					{
						QueryFilter filter = MessageTracingMessageProbe.BuildQueryFilter(guid, dateTime, DateTime.UtcNow, this.clientMessageId, senderUsername, recipientUsername, list);
						enumerable = MessageTracingMessageProbe.msgTraceSession.FindReportObject<MessageTrace>(filter);
					}
					finally
					{
						foreach (IDisposable disposable in list)
						{
							disposable.Dispose();
						}
					}
				}
			}
			catch (Exception ex)
			{
				this.TraceError(string.Format("Caught an exception in VerifyPreviousResults(): {0}", ex.Message), new object[0]);
			}
			if (enumerable != null && enumerable.Count<MessageTrace>() > 0)
			{
				MessageTrace messageTrace = enumerable.FirstOrDefault((MessageTrace r) => r.ClientMessageId.Equals(this.clientMessageId, StringComparison.InvariantCultureIgnoreCase));
				if (messageTrace != null)
				{
					result = true;
					this.TraceDebug("Found test message trace.", new object[0]);
				}
				else
				{
					this.TraceDebug("Did not find test message trace.", new object[0]);
				}
				return result;
			}
			string format = string.Format("Failed to find the message trace with client message id {0} from message trace database.", this.clientMessageId);
			this.TraceError(format, new object[0]);
			throw new MessageTracingProbeException(Strings.TestMailNotFound(this.clientMessageId));
		}

		protected override void UpdateCheckMailResult(bool success, ProbeResult probeResult)
		{
			probeResult.StateAttribute2 = "CheckMailInMessageTracingDatabase";
			probeResult.ExecutionContext = base.Result.ExecutionContext;
			base.UpdateCommonProbeResultAttributes(success);
		}

		protected override void TraceDebug(string format, params object[] args)
		{
			string text = string.Format(format, args);
			ProbeResult result = base.Result;
			result.ExecutionContext = result.ExecutionContext + text + " ";
			WTFDiagnostics.TraceDebug(ExTraceGlobals.MessageTracingTracer, new TracingContext(), text, null, "TraceDebug", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ForefrontActiveMonitoring\\Components\\MessageTracing\\MessageTracingMessageProbe.cs", 199);
		}

		protected override void TraceError(string format, params object[] args)
		{
			string text = string.Format(format, args);
			ProbeResult result = base.Result;
			result.ExecutionContext = result.ExecutionContext + text + " ";
			WTFDiagnostics.TraceError(ExTraceGlobals.MessageTracingTracer, new TracingContext(), text, null, "TraceError", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ForefrontActiveMonitoring\\Components\\MessageTracing\\MessageTracingMessageProbe.cs", 211);
		}

		protected override bool ShouldRun()
		{
			bool flag = base.ShouldRun();
			return flag && ServerComponentStateManager.GetEffectiveState(ServerComponentEnum.HubTransport, false) != ServiceState.Draining;
		}

		private static QueryFilter BuildQueryFilter(Guid tenantId, DateTime startTime, DateTime endTime, string clientMessageId, string senderAddress, string recipientAddress, List<IDisposable> disposableObjects)
		{
			List<ComparisonFilter> list = new List<ComparisonFilter>();
			list.Add(new ComparisonFilter(ComparisonOperator.Equal, Schema.OrganizationQueryDefinition, tenantId));
			list.Add(new ComparisonFilter(ComparisonOperator.GreaterThanOrEqual, Schema.StartDateQueryDefinition, startTime));
			list.Add(new ComparisonFilter(ComparisonOperator.LessThan, Schema.EndDateQueryDefinition, endTime));
			ReportingFilterTable reportingFilterTable = new ReportingFilterTable();
			disposableObjects.Add(reportingFilterTable);
			if (clientMessageId != null)
			{
				reportingFilterTable.AddRow(clientMessageId);
			}
			list.Add(new ComparisonFilter(ComparisonOperator.Equal, Schema.MessageIdListQueryDefinition, reportingFilterTable.DataTable));
			ReportingFilterTable reportingFilterTable2 = new ReportingFilterTable();
			disposableObjects.Add(reportingFilterTable2);
			if (senderAddress != null)
			{
				reportingFilterTable2.AddRow(senderAddress);
			}
			list.Add(new ComparisonFilter(ComparisonOperator.Equal, Schema.SenderAddressListQueryDefinition, reportingFilterTable2.DataTable));
			ReportingFilterTable reportingFilterTable3 = new ReportingFilterTable();
			disposableObjects.Add(reportingFilterTable3);
			if (recipientAddress != null)
			{
				reportingFilterTable3.AddRow(recipientAddress);
			}
			list.Add(new ComparisonFilter(ComparisonOperator.Equal, Schema.RecipientAddressListQueryDefinition, reportingFilterTable3.DataTable));
			ReportingFilterTable reportingFilterTable4 = new ReportingFilterTable();
			disposableObjects.Add(reportingFilterTable4);
			list.Add(new ComparisonFilter(ComparisonOperator.Equal, Schema.MailDeliveryStatusListDefinition, reportingFilterTable4.DataTable));
			return new AndFilter(list.ToArray());
		}

		private const int QueryPeriodInMinutes = 30;

		private static MessageTraceSession msgTraceSession;

		private string clientMessageId;
	}
}
