using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Xml;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Compliance.Xml;
using Microsoft.Exchange.Conversion;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ForefrontActiveMonitoring;
using Microsoft.Exchange.Rpc.MailSubmission;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring.Smtp.Probes
{
	public class SafetyNetProbe : ProbeWorkItem
	{
		internal string MailInfo
		{
			get
			{
				SmtpProbeWorkDefinition.SendMailDefinition sendMail = this.WorkDefinition.SendMail;
				return string.Format("[SLA: {0}] [From: {1}] [To: {2}] [SenderTenantID: {3}] [RecipientTenantID: {4}] {5}", new object[]
				{
					TimeSpan.FromMinutes(sendMail.Sla).ToString(),
					sendMail.SenderUsername,
					sendMail.RecipientUsername,
					sendMail.SenderTenantID,
					sendMail.RecipientTenantID,
					sendMail.Message.ToString()
				});
			}
		}

		protected SmtpProbeWorkDefinition WorkDefinition
		{
			get
			{
				if (this.workDefinition == null)
				{
					this.workDefinition = new SmtpProbeWorkDefinition(base.Id, base.Definition, new DelTraceDebug(this.TraceDebug));
					base.Definition.RecurrenceIntervalSeconds = Math.Max((int)TimeSpan.FromMinutes(this.workDefinition.SendMail.Sla).TotalSeconds, base.Definition.RecurrenceIntervalSeconds);
					this.TraceDebug("Work definition created.", new object[0]);
				}
				return this.workDefinition;
			}
		}

		protected override void DoWork(CancellationToken cancellationToken)
		{
			this.TraceDebug("SafetyNetProbe started.", new object[0]);
			if (!this.ShouldRun())
			{
				this.TraceDebug("SafetyNetProbe skipped because ShouldRun returned false", new object[0]);
				return;
			}
			long num;
			long probeRunSequenceNumber = ProbeRunSequence.GetProbeRunSequenceNumber(base.Id.ToString(), out num);
			this.TraceDebug("Sequence number = {0}. Sequence number count = {1}.", new object[]
			{
				probeRunSequenceNumber,
				num
			});
			if (num > 2L)
			{
				if (!this.ProbeFailed(cancellationToken, probeRunSequenceNumber - 2L))
				{
					this.TraceDebug("Send probe message {0} succeeded. Executing resubmit verification.", new object[]
					{
						probeRunSequenceNumber - 2L
					});
					ProbeResult probeResult = this.CreateProbeResult(SafetyNetProbe.ProbeRecordType.CheckResubmit, probeRunSequenceNumber - 2L);
					bool flag = this.VerifyServiceResults(cancellationToken, probeResult, "SafetyNetResubmit", probeRunSequenceNumber - 2L);
					this.UpdateProbeResult(probeResult, flag, true);
					base.Broker.PublishResult(probeResult);
					this.TraceDebug("Resubmit verification {0}.", new object[]
					{
						flag ? "succeeded" : "failed"
					});
				}
				else
				{
					this.TraceDebug("Send probe message {0} failed. Skipping resubmit verification.", new object[]
					{
						probeRunSequenceNumber - 2L
					});
				}
			}
			if (num > 1L)
			{
				if (!this.ProbeFailed(cancellationToken, probeRunSequenceNumber - 1L))
				{
					long deliveryTime = 0L;
					Guid empty = Guid.Empty;
					this.TraceDebug("Send probe message {0} succeeded. Verifying delivery.", new object[]
					{
						probeRunSequenceNumber - 1L
					});
					ProbeResult probeResult2 = this.CreateProbeResult(SafetyNetProbe.ProbeRecordType.CheckDelivery, probeRunSequenceNumber - 1L);
					bool flag2 = this.VerifyServiceResults(cancellationToken, probeResult2, "Delivery", probeRunSequenceNumber - 1L) && this.GetProbeDeliveryInfo(cancellationToken, probeResult2, probeRunSequenceNumber - 1L, out deliveryTime, out empty);
					this.UpdateProbeResult(probeResult2, flag2, false);
					base.Broker.PublishResult(probeResult2);
					this.TraceDebug("Delivery verification {0}.", new object[]
					{
						flag2 ? "succeeded" : "failed"
					});
					if (flag2)
					{
						ProbeResult probeResult3 = this.CreateProbeResult(SafetyNetProbe.ProbeRecordType.Resubmit, probeRunSequenceNumber - 1L);
						flag2 = this.ResubmitMessage(probeResult3, deliveryTime, empty);
						this.UpdateProbeResult(probeResult3, flag2, false);
						base.Broker.PublishResult(probeResult3);
						this.TraceDebug("Resubmit message {0}.", new object[]
						{
							flag2 ? "succeeded" : "failed"
						});
					}
					else
					{
						this.TraceDebug("Resubmit message skipped", new object[0]);
					}
				}
				else
				{
					this.TraceDebug("Send probe message {0} failed. Skipping resubmit verification.", new object[]
					{
						probeRunSequenceNumber - 1L
					});
				}
			}
			this.SendProbeMessage(cancellationToken, this.WorkDefinition.SendMail, probeRunSequenceNumber);
			this.TraceDebug("SafetyNetProbe finished.", new object[0]);
		}

		protected virtual bool ShouldRun()
		{
			if (!TransportProbeCommon.IsProbeExecutionEnabled())
			{
				return false;
			}
			ServiceState effectiveState = ServerComponentStateManager.GetEffectiveState(ServerComponentEnum.HubTransport, false);
			return effectiveState == ServiceState.Active;
		}

		protected void SendProbeMessage(CancellationToken cancellationToken, SmtpProbeWorkDefinition.SendMailDefinition sendMail, long sequence)
		{
			Message message = sendMail.Message;
			this.TraceDebug("Send probe message. SenderTenant: {0}, RecipientTenant: {1}, From: {2}, To: {3}.", new object[]
			{
				sendMail.SenderTenantID,
				sendMail.RecipientTenantID,
				sendMail.SenderUsername,
				sendMail.RecipientUsername
			});
			try
			{
				base.Result.StateAttribute1 = message.MessageId;
				base.Result.StateAttribute2 = SafetyNetProbe.ProbeRecordType.SendProbe.ToString();
				base.Result.StateAttribute5 = sequence.ToString();
				base.Result.StateAttribute3 = this.MailInfo;
				if (cancellationToken.IsCancellationRequested)
				{
					throw new Exception("Cancellation requested.");
				}
				DateTime utcNow = DateTime.UtcNow;
				string notificationId = this.GetNotificationId(sequence);
				this.TraceDebug("Calling SendMail. NotificationID: {0}", new object[]
				{
					notificationId
				});
				SendMailHelper.SendMail(base.Definition.Name, sendMail, notificationId, null);
				this.TraceDebug("SendMail finished.", new object[0]);
				DateTime utcNow2 = DateTime.UtcNow;
				base.Result.SampleValue = (utcNow2 - utcNow).TotalMilliseconds;
				DateTime dateTime = utcNow2.AddMinutes(this.workDefinition.SendMail.Sla);
				base.Result.StateAttribute4 = string.Format("[MailSentTime: {0}] [MailDeliverySlaTime: {1}]", utcNow2.ToString("o"), dateTime.ToString("o"));
				base.Result.StateAttribute6 = (double)utcNow2.Ticks;
				base.Result.StateAttribute7 = (double)dateTime.Ticks;
				this.UpdateProbeResult(base.Result, true, false);
			}
			catch (Exception e)
			{
				this.LogSendMailException(e);
				this.UpdateProbeResult(base.Result, false, false);
				this.TraceError("SendProbeMessage failed.", new object[0]);
			}
			finally
			{
				message.CleanupAttachment();
			}
			this.TraceError("SendProbeMessage succeeded.", new object[0]);
		}

		private bool ResubmitMessage(ProbeResult probeResult, long deliveryTime, Guid targetMdb)
		{
			try
			{
				MdbefPropertyCollection mdbefPropertyCollection = new MdbefPropertyCollection();
				mdbefPropertyCollection[65547U] = true;
				byte[] bytes = mdbefPropertyCollection.GetBytes();
				using (MailSubmissionServiceRpcClient mailSubmissionServiceRpcClient = new MailSubmissionServiceRpcClient("localhost"))
				{
					AddResubmitRequestStatus addResubmitRequestStatus = mailSubmissionServiceRpcClient.AddMdbResubmitRequest(CombGuidGenerator.NewGuid(), targetMdb, deliveryTime - 1L, deliveryTime + 1L, null, bytes);
					if (addResubmitRequestStatus != AddResubmitRequestStatus.Success)
					{
						this.LogToExecutionContext(probeResult, "Create resubmit request failed. Status: {0}", new object[]
						{
							addResubmitRequestStatus
						});
						return false;
					}
				}
			}
			catch (Exception ex)
			{
				this.LogToExecutionContext(probeResult, "Resubmit request failed. Exception: {0}", new object[]
				{
					ex.ToString()
				});
				return false;
			}
			return true;
		}

		private string GetNotificationId(long sequence)
		{
			return Guid.Parse(string.Format("{0:X8}-0000-0000-0000-{1:X12}", base.Id, (int)sequence)).ToString();
		}

		private bool ProbeFailed(CancellationToken cancellationToken, long sequence)
		{
			DateTime minExecutionEndTime = DateTime.UtcNow.ToLocalTime().AddMinutes(-60.0);
			List<ProbeResult> results = new List<ProbeResult>();
			LocalDataAccess localDataAccess = new LocalDataAccess();
			IOrderedEnumerable<ProbeResult> query = from r in localDataAccess.GetTable<ProbeResult, int>(WorkItemResultIndex<ProbeResult>.WorkItemIdAndExecutionEndTime(base.Id, minExecutionEndTime))
			where r.DeploymentId == Settings.DeploymentId && r.WorkItemId == this.Id && r.StateAttribute5 == sequence.ToString()
			orderby r.ExecutionStartTime
			select r;
			IDataAccessQuery<ProbeResult> dataAccessQuery = localDataAccess.AsDataAccessQuery<ProbeResult>(query);
			dataAccessQuery.ExecuteAsync(delegate(ProbeResult r)
			{
				if (r.ResultType != ResultType.Succeeded)
				{
					results.Add(r);
				}
			}, cancellationToken, SafetyNetProbe.traceContext);
			return results.Count != 0;
		}

		private bool GetProbeDeliveryInfo(CancellationToken cancellationToken, ProbeResult probeResult, long sequence, out long deliveryTime, out Guid targetMdb)
		{
			string notificationId = this.GetNotificationId(sequence);
			foreach (ProbeResult probeResult2 in this.GetServiceResults(cancellationToken, "SafetyNet", notificationId))
			{
				if (!string.IsNullOrWhiteSpace(probeResult2.ExtensionXml))
				{
					XmlNode propertiesNode = this.GetPropertiesNode(probeResult2.ExtensionXml);
					if (propertiesNode != null)
					{
						string property = this.GetProperty(propertiesNode, "DeliveryTime", "");
						string property2 = this.GetProperty(propertiesNode, "TargetMdb", "");
						if (long.TryParse(property, out deliveryTime) && Guid.TryParse(property2, out targetMdb))
						{
							return true;
						}
					}
				}
			}
			deliveryTime = 0L;
			targetMdb = Guid.Empty;
			return false;
		}

		private bool VerifyServiceResults(CancellationToken cancellationToken, ProbeResult probeResult, string category, long sequence)
		{
			string notificationId = this.GetNotificationId(sequence);
			List<ProbeResult> serviceResults = this.GetServiceResults(cancellationToken, "SafetyNet", notificationId);
			this.LogToExecutionContext(probeResult, "# of service results: {0}. ", new object[]
			{
				serviceResults.Count
			});
			bool flag = true;
			List<Notification> notificationsFromResults = this.GetNotificationsFromResults(serviceResults);
			using (IEnumerator<Notification> enumerator = this.GetExpectedNotifications(category).GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					Notification expectedNotification = enumerator.Current;
					bool flag2 = notificationsFromResults.Find((Notification item) => this.DoNotificationsMatch(expectedNotification, item)) != null;
					this.LogToExecutionContext(probeResult, "[Type:{0}, Value:{1}, MatchType:{2}, Verified:{3}] ", new object[]
					{
						expectedNotification.Type,
						expectedNotification.Value,
						expectedNotification.Method.ToString(),
						flag2
					});
					if (!flag2 && expectedNotification.Mandatory)
					{
						flag = false;
					}
				}
			}
			this.LogToExecutionContext(probeResult, "Checked Mail {0}. ", new object[]
			{
				flag ? "successful" : "failed"
			});
			return flag;
		}

		private IEnumerable<Notification> GetExpectedNotifications(string category)
		{
			if (string.IsNullOrWhiteSpace(category))
			{
				return this.WorkDefinition.ExpectedNotifications;
			}
			return from item in this.WorkDefinition.ExpectedNotifications
			where string.Compare(item.Category, category, true) == 0 || string.IsNullOrWhiteSpace(item.Category)
			select item;
		}

		private bool DoNotificationsMatch(Notification expected, Notification retrieved)
		{
			if (string.Compare(expected.Type, retrieved.Type, true) != 0)
			{
				return false;
			}
			if (expected.Method == MatchType.SubString)
			{
				return retrieved.Value.Contains(expected.Value);
			}
			if (expected.Method == MatchType.Regex)
			{
				return Regex.IsMatch(expected.Value, retrieved.Value, RegexOptions.IgnoreCase);
			}
			return string.Compare(expected.Value, retrieved.Value, true) == 0;
		}

		private void LogToExecutionContext(ProbeResult probeResult, string format, params object[] args)
		{
			string text = string.Format(format, args);
			probeResult.ExecutionContext += text;
			WTFDiagnostics.TraceDebug(ExTraceGlobals.ShadowRedundancyTracer, new TracingContext(), text, null, "LogToExecutionContext", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ForefrontActiveMonitoring\\Components\\Transport\\SafetyNetProbe.cs", 536);
		}

		private List<ProbeResult> GetServiceResults(CancellationToken cancellationToken, string componentName, string notificationId)
		{
			string resultName = string.Format("Transport/{0}", componentName);
			DateTime startTime = DateTime.UtcNow.ToLocalTime().AddMinutes(-60.0);
			LocalDataAccess localDataAccess = new LocalDataAccess();
			IOrderedEnumerable<ProbeResult> query = from r in localDataAccess.GetTable<ProbeResult, string>(WorkItemResultIndex<ProbeResult>.ResultNameAndExecutionEndTime(resultName, startTime))
			where r.DeploymentId == Settings.DeploymentId && r.ResultName.StartsWith(resultName) && r.ExecutionEndTime >= startTime && r.StateAttribute1 == notificationId
			orderby r.ExecutionStartTime
			select r;
			IDataAccessQuery<ProbeResult> dataAccessQuery = localDataAccess.AsDataAccessQuery<ProbeResult>(query);
			List<ProbeResult> results = new List<ProbeResult>();
			dataAccessQuery.ExecuteAsync(delegate(ProbeResult r)
			{
				results.Add(r);
			}, cancellationToken, SafetyNetProbe.traceContext);
			return results;
		}

		private List<Notification> GetNotificationsFromResults(List<ProbeResult> results)
		{
			List<Notification> list = new List<Notification>();
			foreach (ProbeResult probeResult in results)
			{
				if (!string.IsNullOrWhiteSpace(probeResult.ExtensionXml))
				{
					XmlNode propertiesNode = this.GetPropertiesNode(probeResult.ExtensionXml);
					if (propertiesNode != null)
					{
						Notification item = new Notification
						{
							Type = this.GetProperty(propertiesNode, "Key", string.Empty),
							Value = this.GetProperty(propertiesNode, "Value", string.Empty)
						};
						list.Add(item);
					}
				}
			}
			return list;
		}

		private XmlNode GetPropertiesNode(string extensionXml)
		{
			if (string.IsNullOrWhiteSpace(extensionXml))
			{
				return null;
			}
			byte[] bytes = Encoding.Default.GetBytes(extensionXml);
			XmlNode result;
			using (MemoryStream memoryStream = new MemoryStream(bytes))
			{
				SafeXmlDocument safeXmlDocument = new SafeXmlDocument();
				safeXmlDocument.Load(memoryStream);
				XmlElement documentElement = safeXmlDocument.DocumentElement;
				result = documentElement.SelectSingleNode("/Properties");
			}
			return result;
		}

		private string GetProperty(XmlNode properties, string propertyName, string defaultValue = "")
		{
			if (properties == null)
			{
				return defaultValue;
			}
			XmlNode xmlNode = properties.SelectSingleNode(propertyName);
			if (xmlNode == null)
			{
				return defaultValue;
			}
			return xmlNode.InnerText;
		}

		private ProbeResult CreateProbeResult(SafetyNetProbe.ProbeRecordType recordType, long sequence)
		{
			return new ProbeResult
			{
				DeploymentId = base.Result.DeploymentId,
				ResultName = base.Result.ResultName,
				MachineName = base.Result.MachineName,
				ServiceName = base.Result.ServiceName,
				WorkItemId = base.Result.WorkItemId,
				ExecutionStartTime = base.Result.ExecutionStartTime,
				ExecutionEndTime = base.Result.ExecutionEndTime,
				Version = base.Result.Version,
				StateAttribute2 = recordType.ToString(),
				StateAttribute5 = sequence.ToString()
			};
		}

		private void UpdateProbeResult(ProbeResult result, bool success, bool isLast = false)
		{
			if (!success)
			{
				result.FailureContext = string.Format("{0} failed", result.StateAttribute2);
			}
			if (success && !isLast)
			{
				ProbeHelper.ModifyResultName(result);
			}
			result.SetCompleted(success ? ResultType.Succeeded : ResultType.Failed);
		}

		private void LogSendMailException(Exception e)
		{
			string text;
			if (e is SmtpException)
			{
				text = string.Format("SmtpException - StatusCode: {0}, {1}", ((SmtpException)e).StatusCode, e.Message);
			}
			else
			{
				if (e.Message == "Cancellation requested.")
				{
					this.TraceError("Cancellation requested.", new object[0]);
					return;
				}
				text = string.Format("Exception - {0}", e.Message);
			}
			base.Result.FailureContext = text;
			this.TraceError(text, new object[0]);
		}

		private void TraceDebug(string format, params object[] args)
		{
			string text = string.Format(format, args);
			ProbeResult result = base.Result;
			result.ExecutionContext = result.ExecutionContext + text + " ";
			WTFDiagnostics.TraceDebug(ExTraceGlobals.ShadowRedundancyTracer, new TracingContext(), text, null, "TraceDebug", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ForefrontActiveMonitoring\\Components\\Transport\\SafetyNetProbe.cs", 740);
		}

		private void TraceError(string format, params object[] args)
		{
			string text = string.Format(format, args);
			ProbeResult result = base.Result;
			result.ExecutionContext = result.ExecutionContext + text + " ";
			WTFDiagnostics.TraceError(ExTraceGlobals.ShadowRedundancyTracer, new TracingContext(), text, null, "TraceError", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ForefrontActiveMonitoring\\Components\\Transport\\SafetyNetProbe.cs", 752);
		}

		private const string CancellationMessage = "Cancellation requested.";

		private const string ComponentName = "SafetyNet";

		private static readonly TracingContext traceContext = new TracingContext();

		private SmtpProbeWorkDefinition workDefinition;

		private enum ProbeRecordType
		{
			SendProbe,
			CheckDelivery,
			Resubmit,
			CheckResubmit
		}
	}
}
