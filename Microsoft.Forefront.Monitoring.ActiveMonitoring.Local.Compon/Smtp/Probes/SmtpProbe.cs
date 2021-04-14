using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ForefrontActiveMonitoring;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring.Smtp.Probes
{
	public class SmtpProbe : ProbeWorkItem
	{
		public SmtpProbe() : this(null, new BasicSmtpClientFactory())
		{
		}

		public SmtpProbe(IPop3Client popClient, IMinimalSmtpClientFactory smtpClientFactory)
		{
			this.popClient = popClient;
			this.smtpClientFactory = smtpClientFactory;
		}

		internal string MailInfo
		{
			get
			{
				SmtpProbeWorkDefinition.SendMailDefinition sendMail = this.WorkDefinition.SendMail;
				Message message = sendMail.Message;
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.AppendFormat("[SLA:{0}] ", TimeSpan.FromMinutes(sendMail.Sla).ToString());
				stringBuilder.AppendFormat("[From:{0}] ", sendMail.SenderUsername);
				stringBuilder.AppendFormat("[To:{0}] ", sendMail.RecipientUsername);
				stringBuilder.AppendFormat("[{0}]", message.ToString());
				return stringBuilder.ToString();
			}
		}

		private protected SmtpProbeWorkDefinition WorkDefinition { protected get; private set; }

		protected IPop3Client PopClient
		{
			get
			{
				return this.popClient;
			}
			set
			{
				this.popClient = value;
			}
		}

		private protected CancellationToken CancellationToken { protected get; private set; }

		protected List<ProbeStatus> AllPreviousSendResults { get; set; }

		protected List<ProbeStatus> PreviousSuccessSendResults { get; set; }

		protected List<ProbeStatus> PreviousDeliverResults { get; set; }

		public static string GetProbeErrorType(ProbeResult result)
		{
			return result.StateAttribute1;
		}

		public static void SetProbeErrorType(ProbeResult result, MailErrorType value)
		{
			result.StateAttribute1 = value.ToString();
			SmtpFailureCategory failureCategory;
			SmtpProbe.failureComponentMapping.TryGetValue(value, out failureCategory);
			result.FailureCategory = (int)failureCategory;
		}

		public static bool GetAuthOnly(ProbeResult result)
		{
			return result.StateAttribute6 == 14.0;
		}

		public static void SetAuthOnly(ProbeResult result, bool value)
		{
			result.StateAttribute6 = (double)(value ? 14 : 15);
		}

		public static string GetSendMailExecutionId(ProbeResult result)
		{
			if (SmtpProbe.GetProbeRecordType(result) == RecordType.SendMail.ToString())
			{
				return result.ExecutionId.ToString();
			}
			if (SmtpProbe.GetProbeRecordType(result) == RecordType.DeliverMail.ToString())
			{
				return result.StateAttribute11;
			}
			return "N/A";
		}

		public static void SetSendMailExecutionId(ProbeResult result, string value)
		{
			result.StateAttribute11 = value;
		}

		public static bool GetDeliveryExpected(ProbeResult result)
		{
			return result.StateAttribute16 == 1.0;
		}

		public static void SetDeliveryExpected(ProbeResult result, bool value)
		{
			result.StateAttribute16 = (double)(value ? 1 : 0);
		}

		public static double GetSentTime(ProbeResult result)
		{
			return result.StateAttribute17;
		}

		public static void SetSentTime(ProbeResult result, double value)
		{
			result.StateAttribute17 = value;
		}

		public static double GetExpectedDeliverTime(ProbeResult result)
		{
			return result.StateAttribute18;
		}

		public static void SetExpectedDeliverTime(ProbeResult result, double value)
		{
			result.StateAttribute18 = value;
		}

		public static double GetActualDeliverTime(ProbeResult result)
		{
			return result.StateAttribute19;
		}

		public static void SetActualDeliverTime(ProbeResult result, double value)
		{
			result.StateAttribute19 = value;
		}

		public static AddressFamily GetIpVersion(ProbeResult result)
		{
			if (result.StateAttribute7 == 4.0)
			{
				return AddressFamily.InterNetwork;
			}
			if (result.StateAttribute7 == 6.0)
			{
				return AddressFamily.InterNetworkV6;
			}
			return AddressFamily.Unspecified;
		}

		public static void SetIpVersion(ProbeResult result, AddressFamily value)
		{
			if (value == AddressFamily.InterNetwork)
			{
				result.StateAttribute7 = 4.0;
				return;
			}
			if (value == AddressFamily.InterNetworkV6)
			{
				result.StateAttribute7 = 6.0;
				return;
			}
			result.StateAttribute7 = 0.0;
		}

		public static double GetPort(ProbeResult result)
		{
			return result.StateAttribute8;
		}

		public static void SetPort(ProbeResult result, double value)
		{
			result.StateAttribute8 = value;
		}

		public static string GetInternalProbeId(ProbeResult result)
		{
			return result.StateAttribute22;
		}

		public static void SetInternalProbeId(ProbeResult result, string value)
		{
			result.StateAttribute22 = value;
		}

		public static string GetProbeMailInfo(ProbeResult result)
		{
			return result.StateAttribute24;
		}

		public static void SetProbeMailInfo(ProbeResult result, string value)
		{
			result.StateAttribute24 = value;
		}

		public static string GetProbeRecordType(ProbeResult result)
		{
			return result.StateAttribute12;
		}

		public static void SetProbeRecordType(ProbeResult result, RecordType value)
		{
			result.StateAttribute12 = value.ToString();
		}

		protected override void DoWork(CancellationToken cancellationToken)
		{
			this.TraceDebug("Started", new object[0]);
			this.SetDefaultAttributeValues();
			this.CancellationToken = cancellationToken;
			try
			{
				this.GetExtendedWorkDefinition();
			}
			catch (Exception e)
			{
				this.TraceError(e, "GetWorkDefinition error", new object[0]);
				SmtpProbe.SetProbeErrorType(base.Result, MailErrorType.ConfigurationError);
				throw;
			}
			try
			{
				this.LoadAdditionalData();
			}
			catch (Exception e2)
			{
				this.TraceError(e2, "LoadAdditionalData error", new object[0]);
				SmtpProbe.SetProbeErrorType(base.Result, MailErrorType.ConfigurationError);
				throw;
			}
			Stopwatch stopwatch = Stopwatch.StartNew();
			try
			{
				this.FindProbeResults();
				this.CheckCancellation();
			}
			catch (OperationCanceledException e3)
			{
				this.TraceError(e3, "SmtpProbe finished with cancellation", new object[0]);
				SmtpProbe.SetProbeErrorType(base.Result, MailErrorType.FindProbeResultsTimeOut);
				throw;
			}
			finally
			{
				this.TraceDebug("FPR Time:{0} seconds", new object[]
				{
					stopwatch.Elapsed.TotalSeconds
				});
			}
			Pop3Exception ex = null;
			try
			{
				stopwatch = Stopwatch.StartNew();
				this.CheckMail();
				this.CheckCancellation();
			}
			catch (Pop3Exception ex2)
			{
				this.TraceError(ex2, "Pop3Exception", new object[0]);
				ex = ex2;
				SmtpProbe.SetProbeErrorType(base.Result, MailErrorType.CheckMailException);
			}
			catch (OperationCanceledException e4)
			{
				this.TraceError(e4, "SmtpProbe finished with cancellation", new object[0]);
				this.HandleCheckMailTimeOutException(stopwatch.Elapsed);
				throw;
			}
			catch (Exception e5)
			{
				this.TraceError(e5, "CheckMail error", new object[0]);
			}
			finally
			{
				this.TraceDebug("CM Time:{0} seconds", new object[]
				{
					stopwatch.Elapsed.TotalSeconds
				});
			}
			try
			{
				stopwatch = Stopwatch.StartNew();
				this.UpdateUndeliveredMessages(ex);
				this.CheckCancellation();
			}
			catch (OperationCanceledException e6)
			{
				this.TraceError(e6, "SmtpProbe finished with cancellation", new object[0]);
				this.HandleUumTimeOutException(stopwatch.Elapsed);
				throw;
			}
			catch (Exception e7)
			{
				this.TraceError(e7, "UUM error", new object[0]);
			}
			finally
			{
				this.TraceDebug("UUM Time:{0} seconds", new object[]
				{
					stopwatch.Elapsed.TotalSeconds
				});
			}
			try
			{
				stopwatch = Stopwatch.StartNew();
				this.SendMail();
				if (!this.MailErrorAlreadySet())
				{
					base.Result.ResultType = ResultType.Succeeded;
				}
			}
			catch (OperationCanceledException e8)
			{
				this.TraceError(e8, "SmtpProbe finished with cancellation", new object[0]);
				this.HandleSmtpSendTimeOutException(stopwatch.Elapsed);
				throw;
			}
			catch (Exception ex3)
			{
				if (this.WorkDefinition.SendMail.IgnoreSendMailFailure)
				{
					this.TraceDebug("SendMail exception ignored: {0}", new object[]
					{
						ex3.ToString()
					});
					SmtpProbe.SetDeliveryExpected(base.Result, false);
					return;
				}
				this.LogSendMailException(ex3);
				this.TraceError("SmtpProbe finished with failure.", new object[0]);
				if (!this.MailErrorAlreadySet())
				{
					SmtpProbe.SetProbeErrorType(base.Result, MailErrorType.SendMailException);
				}
				throw;
			}
			finally
			{
				this.TraceDebug("SM Time:{0} seconds", new object[]
				{
					stopwatch.Elapsed.TotalSeconds
				});
				this.HandleResultPublished(base.Result);
			}
			this.TraceDebug("SmtpProbe finished successfully", new object[0]);
		}

		protected virtual void HandleResultPublished(ProbeResult result)
		{
		}

		protected virtual void HandleCheckMailTimeOutException(TimeSpan checkMailTime)
		{
			if (checkMailTime.TotalSeconds > (double)base.Definition.TimeoutSeconds / 2.0)
			{
				SmtpProbe.SetProbeErrorType(base.Result, MailErrorType.CheckMailTimeOut);
				return;
			}
			SmtpProbe.SetProbeErrorType(base.Result, MailErrorType.ProbeTimeOut);
		}

		protected virtual void HandleUumTimeOutException(TimeSpan uumTime)
		{
			if (uumTime.TotalSeconds > (double)base.Definition.TimeoutSeconds / 2.0)
			{
				SmtpProbe.SetProbeErrorType(base.Result, MailErrorType.UpdateUndeliveredTimeOut);
				return;
			}
			SmtpProbe.SetProbeErrorType(base.Result, MailErrorType.ProbeTimeOut);
		}

		protected virtual void HandleSmtpSendTimeOutException(TimeSpan sendMailTime)
		{
			if (sendMailTime.TotalSeconds > (double)base.Definition.TimeoutSeconds / 2.0)
			{
				SmtpProbe.SetProbeErrorType(base.Result, MailErrorType.SendMailTimeOut);
				return;
			}
			SmtpProbe.SetProbeErrorType(base.Result, MailErrorType.ProbeTimeOut);
		}

		protected void FindProbeResults()
		{
			if (!this.WorkDefinition.CheckMail.Enabled)
			{
				return;
			}
			this.CheckCancellation();
			try
			{
				WTFDiagnostics.TraceInformation(ExTraceGlobals.SMTPTracer, new TracingContext(), "FindProbeResults started.", null, "FindProbeResults", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ForefrontActiveMonitoring\\Components\\Smtp\\Probes\\SmtpProbe.cs", 836);
				this.FindProbeResultsInternal();
			}
			catch (AggregateException ex)
			{
				this.TraceError(ex.Flatten(), "FindProbeResults error", new object[0]);
				SmtpProbe.SetProbeErrorType(base.Result, MailErrorType.SqlQueryFailure);
			}
			catch (Exception e)
			{
				this.TraceError(e, "FindProbeResults error", new object[0]);
				SmtpProbe.SetProbeErrorType(base.Result, MailErrorType.SqlQueryFailure);
			}
			this.TraceDebug("Found {0} previous send results", new object[]
			{
				this.PreviousSuccessSendResults.Count
			});
			this.TraceDebug("Found {0} previous deliver results", new object[]
			{
				this.PreviousDeliverResults.Count
			});
			this.CheckCancellation();
		}

		protected virtual void FindProbeResultsInternal()
		{
			this.AllPreviousSendResults = new List<ProbeStatus>();
			this.PreviousSuccessSendResults = new List<ProbeStatus>();
			this.PreviousDeliverResults = new List<ProbeStatus>();
			int queryTimeWindow = this.WorkDefinition.CheckMail.QueryTimeWindow;
			DateTime executionStartTime = base.Result.ExecutionStartTime;
			DateTime startTime = base.Result.ExecutionStartTime.AddSeconds((double)(-2 * queryTimeWindow)).AddMinutes(-this.WorkDefinition.SendMail.Sla * 2.0);
			IDataAccessQuery<ProbeResult> probeResults = base.Broker.GetProbeResults(base.Definition, startTime);
			Task<int> task = probeResults.ExecuteAsync(delegate(ProbeResult r)
			{
				ProbeStatus probeStatus = new ProbeStatus(r);
				if (probeStatus.RecordType == RecordType.SendMail)
				{
					this.AllPreviousSendResults.Add(probeStatus);
					if (probeStatus.ResultType == ResultType.Succeeded)
					{
						this.PreviousSuccessSendResults.Add(probeStatus);
						return;
					}
				}
				else
				{
					if (probeStatus.RecordType == RecordType.DeliverMail)
					{
						this.PreviousDeliverResults.Add(probeStatus);
						return;
					}
					this.TraceError("Unknown record type={0}, ID={1}", new object[]
					{
						probeStatus.RecordType,
						probeStatus.InternalProbeId
					});
				}
			}, this.CancellationToken, base.TraceContext);
			task.Wait(this.CancellationToken);
		}

		protected void CheckMail()
		{
			SmtpProbeWorkDefinition.CheckMailDefinition checkMail = this.WorkDefinition.CheckMail;
			if (!checkMail.Enabled)
			{
				return;
			}
			using (IPop3Client pop3Client = this.popClient ?? new Pop3Client(this.CancellationToken))
			{
				this.TraceDebug("Connecting to POP. Server={0}, Port={1}, User={2}, EnableSsl={3}, ReadTimeout={4}s", new object[]
				{
					checkMail.PopServer,
					checkMail.Port,
					checkMail.Username,
					checkMail.EnableSsl,
					checkMail.ReadTimeout
				});
				try
				{
					pop3Client.Connect(checkMail.PopServer, checkMail.Port, checkMail.Username, checkMail.Password, checkMail.EnableSsl, checkMail.ReadTimeout);
				}
				catch (Pop3Exception e)
				{
					this.HandleLoginException(e);
					throw;
				}
				catch (OperationCanceledException)
				{
					SmtpProbe.SetProbeErrorType(base.Result, MailErrorType.MailboxLoginFailure);
					throw;
				}
				this.TraceDebug("Connected", new object[0]);
				List<Pop3Message> list = pop3Client.List();
				list.Reverse(0, list.Count);
				this.TraceDebug("MsgCount={0}", new object[]
				{
					list.Count
				});
				TimeSpan inboxQueryWindow = this.GetInboxQueryWindow();
				DateTime dateTime = DateTime.UtcNow - inboxQueryWindow;
				this.TraceDebug("QueryLimit={0}", new object[]
				{
					dateTime
				});
				int num = 0;
				int num2 = 0;
				int num3 = 0;
				List<long> list2 = new List<long>();
				try
				{
					foreach (Pop3Message pop3Message in list)
					{
						this.CheckCancellation();
						num2++;
						pop3Client.RetrieveHeader(pop3Message);
						if (pop3Message.ReceivedDate < dateTime)
						{
							this.TraceDebug("MsgOutsideQueryLimit={0}", new object[]
							{
								pop3Message.ReceivedDate.ToString()
							});
							break;
						}
						string text;
						if (checkMail.DisableCheckMailByMessageId)
						{
							if (this.VerifyMessage(pop3Client, pop3Message))
							{
								this.PublishMailWithNoMessageId(pop3Message);
								list2.Add(pop3Message.Number);
								pop3Client.Delete(pop3Message);
								num++;
							}
						}
						else if (this.MatchMessage(pop3Message, out text))
						{
							this.TraceDebug("Found: ID={0}", new object[]
							{
								text
							});
							bool flag = this.VerifyMessage(pop3Client, pop3Message);
							this.TraceDebug("Verified={0}", new object[]
							{
								flag
							});
							this.LogMessage(pop3Message, text);
							bool flag2 = false;
							bool flag3 = this.UpdateDeliveryStatus(pop3Message, text, flag, inboxQueryWindow, out flag2);
							if (flag3)
							{
								num++;
							}
							if (flag2)
							{
								list2.Add(pop3Message.Number);
								pop3Client.Delete(pop3Message);
								num3++;
							}
						}
					}
				}
				finally
				{
					this.TraceDebug("MsgScanned={0}", new object[]
					{
						num2
					});
					this.TraceDebug("MsgMatched={0}", new object[]
					{
						num
					});
					this.TraceDebug("MsgDeleted={0}", new object[]
					{
						num3
					});
				}
				if (!checkMail.DisableCheckMailByMessageId)
				{
					this.CleanUpMailbox(pop3Client, list, list2);
				}
			}
		}

		protected virtual void HandleLoginException(Pop3Exception e)
		{
			SmtpProbe.SetProbeErrorType(base.Result, MailErrorType.MailboxLoginFailure);
		}

		protected void SendMail()
		{
			SmtpProbeWorkDefinition.SendMailDefinition sendMail = this.WorkDefinition.SendMail;
			Message message = sendMail.Message;
			MailMessage mail = message.Mail;
			this.TraceDebug("SmtpServer={0}, Port={1}, From={2}, To={3}, Timeout={4}s", new object[]
			{
				sendMail.SmtpServer,
				sendMail.Port,
				sendMail.SenderUsername,
				sendMail.RecipientUsername,
				sendMail.Timeout
			});
			try
			{
				this.CheckCancellation();
				if (!sendMail.Enabled)
				{
					this.TraceDebug("SendMail is DISABLED", new object[0]);
					SmtpProbe.SetProbeRecordType(base.Result, RecordType.None);
				}
				else
				{
					SmtpProbe.SetInternalProbeId(base.Result, message.MessageId);
					SmtpProbe.SetProbeMailInfo(base.Result, this.MailInfo);
					mail.From = new MailAddress(sendMail.SenderUsername);
					mail.To.Add(sendMail.RecipientUsername);
					foreach (NameValuePair nameValuePair in message.Headers)
					{
						mail.Headers.Add(nameValuePair.Name, nameValuePair.Value);
					}
					mail.Subject = message.Subject;
					mail.Body = message.Body;
					foreach (KeyValuePair<string, object> entry in message.Attachments)
					{
						mail.Attachments.Add(Message.CreateMailAttachment(entry));
					}
					if (sendMail.IgnoreCertificateNameMismatchPolicyError)
					{
						ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(this.ValidateCertificate);
					}
					this.TraceDebug("Calling Send: ID={0}", new object[]
					{
						message.MessageId
					});
					DateTime utcNow = DateTime.UtcNow;
					this.SendMail(mail);
					DateTime utcNow2 = DateTime.UtcNow;
					base.Result.SampleValue = (utcNow2 - utcNow).TotalMilliseconds;
					this.TraceDebug("Send done", new object[0]);
					DateTime dateTime = utcNow2.AddMinutes(this.WorkDefinition.SendMail.Sla);
					SmtpProbe.SetSentTime(base.Result, (double)utcNow2.Ticks);
					SmtpProbe.SetDeliveryExpected(base.Result, message.ExpectDelivery);
					if (!sendMail.AuthOnly)
					{
						SmtpProbe.SetExpectedDeliverTime(base.Result, (double)dateTime.Ticks);
					}
				}
			}
			finally
			{
				message.CleanupAttachment();
			}
		}

		protected virtual void SendMail(MailMessage mail)
		{
			IMinimalSmtpClient minimalSmtpClient = null;
			try
			{
				string host = this.WorkDefinition.SendMail.ResolveEndPoint ? ResolverHelper.ResolveEndPoint(this.WorkDefinition.SendMail.SmtpServer, this.WorkDefinition.SendMail.RecordResolveType, new DelTraceDebug(this.TraceDebug), this.WorkDefinition.SendMail.SimpleResolution).ToString() : this.WorkDefinition.SendMail.SmtpServer;
				minimalSmtpClient = this.smtpClientFactory.CreateSmtpClient(host, this.WorkDefinition, new DelTraceDebug(this.TraceDebug));
				minimalSmtpClient.CancellationToken = this.CancellationToken;
				minimalSmtpClient.Send(mail);
			}
			catch (ResolverHelper.UnableToResolveException e)
			{
				this.HandleDnsFailure(e);
				throw;
			}
			finally
			{
				if (minimalSmtpClient != null)
				{
					this.UpdateProbeExecutionData(minimalSmtpClient);
					try
					{
						minimalSmtpClient.Dispose();
					}
					catch (Exception e2)
					{
						this.TraceError(e2, "SmtpClient Dispose exception ignored", new object[0]);
					}
				}
			}
		}

		protected void GetExtendedWorkDefinition()
		{
			if (this.WorkDefinition == null)
			{
				this.WorkDefinition = new SmtpProbeWorkDefinition(base.Id, base.Definition, new DelTraceDebug(this.TraceDebug));
				this.TraceDebug("Work definition processed", new object[0]);
			}
		}

		protected virtual void UpdateProbeExecutionData(IMinimalSmtpClient smtpClient)
		{
		}

		protected virtual void CopySendDataToDeliver(ProbeStatus sendStatus, ProbeResult deliverResult)
		{
			SmtpProbe.SetInternalProbeId(deliverResult, sendStatus.InternalProbeId);
			SmtpProbe.SetProbeMailInfo(deliverResult, sendStatus.ProbeMailInfo);
			SmtpProbe.SetDeliveryExpected(deliverResult, sendStatus.DeliveryExpected);
			SmtpProbe.SetSentTime(deliverResult, (double)sendStatus.SentTime.Ticks);
			SmtpProbe.SetSendMailExecutionId(deliverResult, sendStatus.SendMailExecutionId);
			SmtpProbe.SetExpectedDeliverTime(deliverResult, (double)(sendStatus.SentTime + TimeSpan.FromMinutes(this.WorkDefinition.SendMail.Sla)).Ticks);
			SmtpProbe.SetIpVersion(deliverResult, this.WorkDefinition.SendMail.IpVersion);
			SmtpProbe.SetAuthOnly(deliverResult, this.WorkDefinition.SendMail.AuthOnly);
			SmtpProbe.SetPort(deliverResult, (double)this.WorkDefinition.SendMail.Port);
		}

		protected virtual void HandleSlaMiss(ProbeResult deliverResult)
		{
			this.SetDeliverResultError(deliverResult, "Mail delivery exceeded the SLA ({0}s)", new object[]
			{
				TimeSpan.FromMinutes(this.WorkDefinition.SendMail.Sla).TotalSeconds
			});
			SmtpProbe.SetProbeErrorType(deliverResult, MailErrorType.SlaExceeded);
		}

		protected virtual void HandleDeliveryFailure(ProbeResult deliverResult, ProbeStatus previousSendStatus, Exception ex)
		{
			if (ex != null)
			{
				this.SetDeliverResultError(deliverResult, ex);
			}
			if (SmtpProbe.GetProbeErrorType(base.Result) == MailErrorType.MailboxLoginFailure.ToString() || (previousSendStatus != null && previousSendStatus.ProbeErrorType == MailErrorType.MailboxLoginFailure))
			{
				SmtpProbe.SetProbeErrorType(deliverResult, MailErrorType.MailboxLoginFailure);
			}
			else if (SmtpProbe.GetProbeErrorType(base.Result) == MailErrorType.PopProxyFailure.ToString() || (previousSendStatus != null && previousSendStatus.ProbeErrorType == MailErrorType.PopProxyFailure))
			{
				SmtpProbe.SetProbeErrorType(deliverResult, MailErrorType.PopProxyFailure);
			}
			else if (SmtpProbe.GetProbeErrorType(base.Result) == MailErrorType.CheckMailException.ToString() || (previousSendStatus != null && previousSendStatus.ProbeErrorType == MailErrorType.CheckMailException))
			{
				SmtpProbe.SetProbeErrorType(deliverResult, MailErrorType.CheckMailException);
			}
			else if (previousSendStatus != null && previousSendStatus.ProbeErrorType == MailErrorType.FindProbeResultsTimeOut)
			{
				SmtpProbe.SetProbeErrorType(deliverResult, MailErrorType.FindProbeResultsTimeOut);
			}
			else if (previousSendStatus != null && previousSendStatus.ProbeErrorType == MailErrorType.SqlQueryFailure)
			{
				SmtpProbe.SetProbeErrorType(deliverResult, MailErrorType.SqlQueryFailure);
			}
			else if (ex != null)
			{
				SmtpProbe.SetProbeErrorType(deliverResult, MailErrorType.CheckMailException);
			}
			else
			{
				this.SetDeliverResultError(deliverResult, "Mail was not delivered after {0}s and is considered a delivery failure", new object[]
				{
					this.WorkDefinition.CheckMail.QueryTimeWindow
				});
				SmtpProbe.SetProbeErrorType(deliverResult, MailErrorType.NoDelivery);
			}
			deliverResult.SetCompleted(ResultType.Failed);
		}

		protected virtual void HandleDnsFailure(ResolverHelper.UnableToResolveException e)
		{
			this.TraceError(e, "{0} Resolution error", new object[]
			{
				e.QueryType
			});
			SmtpProbe.SetProbeErrorType(base.Result, MailErrorType.AzureDnsFailure);
		}

		protected virtual void HandleVerificationFailure(ProbeResult deliverResult)
		{
			this.SetDeliverResultError(deliverResult, "Message verification failed.", new object[0]);
			SmtpProbe.SetProbeErrorType(deliverResult, MailErrorType.VerificationFailure);
		}

		protected virtual void SetDefaultAttributeValues()
		{
			SmtpProbe.SetProbeErrorType(base.Result, MailErrorType.Success);
			SmtpProbe.SetProbeRecordType(base.Result, RecordType.SendMail);
			SmtpProbe.SetInternalProbeId(base.Result, "None. Probe exited before creating subject");
			SmtpProbe.SetProbeMailInfo(base.Result, "None. Probe exited before creating mail info");
			SmtpProbe.SetPort(base.Result, 0.0);
			SmtpProbe.SetSendMailExecutionId(base.Result, "None");
			SmtpProbe.SetDeliveryExpected(base.Result, false);
			SmtpProbe.SetSentTime(base.Result, 0.0);
			SmtpProbe.SetProbeErrorType(base.Result, MailErrorType.Success);
			base.Result.ResultType = ResultType.Failed;
		}

		protected virtual void LoadAdditionalData()
		{
			SmtpProbe.SetAuthOnly(base.Result, this.WorkDefinition.SendMail.AuthOnly);
			SmtpProbe.SetIpVersion(base.Result, this.WorkDefinition.SendMail.IpVersion);
			SmtpProbe.SetPort(base.Result, (double)this.WorkDefinition.SendMail.Port);
		}

		protected void TraceDebug(string format, params object[] args)
		{
			string text = string.Format(format, args);
			ProbeResult result = base.Result;
			result.ExecutionContext += string.Format("[{0}] ", text);
			WTFDiagnostics.TraceInformation(ExTraceGlobals.SMTPTracer, new TracingContext(), text, null, "TraceDebug", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ForefrontActiveMonitoring\\Components\\Smtp\\Probes\\SmtpProbe.cs", 1336);
		}

		protected void TraceDebug(StringBuilder log, string format, params object[] args)
		{
			string text = string.Format(format, args);
			if (log != null)
			{
				log.AppendFormat("[{0}] ", text);
			}
			WTFDiagnostics.TraceInformation(ExTraceGlobals.SMTPTracer, new TracingContext(), text, null, "TraceDebug", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ForefrontActiveMonitoring\\Components\\Smtp\\Probes\\SmtpProbe.cs", 1354);
		}

		protected void TraceError(string format, params object[] args)
		{
			string text = string.Format(format, args);
			ProbeResult result = base.Result;
			result.ExecutionContext += string.Format("[{0}] ", text);
			WTFDiagnostics.TraceError(ExTraceGlobals.SMTPTracer, new TracingContext(), text, null, "TraceError", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ForefrontActiveMonitoring\\Components\\Smtp\\Probes\\SmtpProbe.cs", 1366);
		}

		protected void TraceError(Exception e, string format, params object[] args)
		{
			string arg = string.Format(format, args);
			if (e.InnerException == null)
			{
				ProbeResult result = base.Result;
				result.ExecutionContext += string.Format("[{0}. {1}] ", arg, e.Message);
			}
			else
			{
				ProbeResult result2 = base.Result;
				result2.ExecutionContext += string.Format("[{0}. {1} Inner: {2}] ", arg, e.Message, e.InnerException.Message);
			}
			WTFDiagnostics.TraceError<string, string>(ExTraceGlobals.SMTPTracer, new TracingContext(), "{0}. {1}", arg, e.ToString(), null, "TraceError", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ForefrontActiveMonitoring\\Components\\Smtp\\Probes\\SmtpProbe.cs", 1388);
		}

		private bool MailErrorAlreadySet()
		{
			string probeErrorType = SmtpProbe.GetProbeErrorType(base.Result);
			return !string.IsNullOrWhiteSpace(probeErrorType) && !string.Equals(probeErrorType, MailErrorType.Success.ToString());
		}

		private bool MatchMessage(Pop3Message message, out string messageId)
		{
			messageId = string.Empty;
			Match match = null;
			string subjectOverride = this.WorkDefinition.SendMail.Message.SubjectOverride;
			if (!this.WorkDefinition.SendMail.Message.UseSubjectVerbatim)
			{
				string pattern = "^" + subjectOverride + "-[0-9]{20}(\\w\\W)*";
				match = Regex.Match(message.Subject, pattern);
			}
			else
			{
				string value = string.Format("{0}:", Message.ProbeMessageIDHeaderTag);
				string pattern2 = "(\\w\\W)*" + subjectOverride + "-[0-9]{20}(\\w\\W)*";
				foreach (string text in message.HeaderComponents)
				{
					if (text.Trim().StartsWith(value))
					{
						match = Regex.Match(text, pattern2);
						if (match.Success)
						{
							break;
						}
					}
				}
			}
			if (match != null && match.Success)
			{
				messageId = match.ToString();
				return true;
			}
			return false;
		}

		private void CleanUpMailbox(IPop3Client pop3Client, List<Pop3Message> messages, IEnumerable<long> messagesDeleted)
		{
			SmtpProbeWorkDefinition.CheckMailDefinition checkMail = this.WorkDefinition.CheckMail;
			int num = base.Definition.TimeoutSeconds - 120;
			if (checkMail.DeleteMessageMinutes == 0 || num <= 0)
			{
				this.TraceDebug("Cleanup skipped", new object[0]);
				return;
			}
			messages.Reverse(0, messages.Count);
			DateTime t = DateTime.UtcNow.AddMinutes((double)(-1 * checkMail.DeleteMessageMinutes));
			string pattern = "^[0-9]{10}-[0-9]{20}(\\w\\W)*";
			int num2 = 0;
			int num3 = 0;
			DateTime t2 = DateTime.UtcNow.AddSeconds((double)num);
			this.TraceDebug("Starting cleanup ({0}s)", new object[]
			{
				num
			});
			foreach (Pop3Message pop3Message in messages)
			{
				if (DateTime.UtcNow >= t2)
				{
					break;
				}
				if (!messagesDeleted.Contains(pop3Message.Number))
				{
					pop3Client.RetrieveHeader(pop3Message);
					if (!(pop3Message.ReceivedDate < t))
					{
						break;
					}
					if (Regex.Match(pop3Message.Subject, pattern).Success)
					{
						try
						{
							pop3Client.Delete(pop3Message);
							num2++;
						}
						catch (Exception e)
						{
							this.TraceError(e, "Mailbox cleanup error", new object[0]);
						}
					}
					num3++;
					this.CheckCancellation();
				}
			}
			this.TraceDebug("Checked={0}, Deleted={1}", new object[]
			{
				num3,
				num2
			});
		}

		private bool VerifyMessage(IPop3Client pop3Client, Pop3Message message)
		{
			if (this.WorkDefinition.CheckMail.ExpectedMessage == null)
			{
				return true;
			}
			this.mailVerificationLog.TryAdd(message.Number, new StringBuilder());
			return ((this.WorkDefinition.CheckMail.ExpectedMessage.Body == null && this.WorkDefinition.CheckMail.ExpectedMessage.Attachment == null) || this.RetrieveMessageBody(pop3Client, message)) && (this.VerifySubject(message) && this.VerifyHeader(message) && this.VerifyMessageBody(message)) && this.VerifyAttachment(message);
		}

		private bool RetrieveMessageBody(IPop3Client pop3Client, Pop3Message message)
		{
			if (message.Components == null)
			{
				pop3Client.Retrieve(message);
			}
			int num = message.HeaderComponents.Count;
			int count = message.Components.Count;
			if (count > num)
			{
				message.BodyComponents = (from i in Enumerable.Range(num, count - num)
				select message.Components[i]).ToList<string>();
			}
			if (message.BodyComponents == null)
			{
				this.TraceError("Failed to retrieve message body components.", new object[0]);
				return false;
			}
			num = message.HeaderComponents.FindIndex((string s) => s.Contains("boundary="));
			StringBuilder log = this.GetMailVerificationLog(message.Number);
			if (num < 0)
			{
				this.TraceDebug(log, "Not a multipart message.", new object[0]);
				message.BodyText = string.Join(Environment.NewLine, message.BodyComponents.ToList<string>()).Trim();
				message.AttachmentCount = 0;
			}
			else
			{
				string boundary = message.HeaderComponents.ToList<string>()[num].Trim().Substring("boundary=".Length).Trim(new char[]
				{
					'"'
				});
				this.TraceDebug(log, "Multipart message", new object[]
				{
					boundary
				});
				IEnumerable<int> source = from i in Enumerable.Range(0, message.BodyComponents.Count)
				where message.BodyComponents[i].Contains(boundary)
				select i;
				int num2 = source.Count<int>() - 1;
				if (num2 <= 1)
				{
					this.TraceError("NumberOfBodyParts must > 1", new object[0]);
					return false;
				}
				this.TraceDebug(log, "NumberOfBodyParts={0}", new object[]
				{
					num2
				});
				int num3 = source.First<int>();
				int num4 = source.ToList<int>()[1] - 1;
				source = from i in Enumerable.Range(num3, num4 - num3)
				where string.IsNullOrWhiteSpace(message.BodyComponents[i])
				select i;
				num3 = ((source.Count<int>() == 0) ? num4 : (source.ToList<int>().First<int>() + 1));
				if (num3 < num4)
				{
					IEnumerable<string> source2 = from i in Enumerable.Range(num3, num4 - num3)
					select message.BodyComponents[i];
					message.BodyText = string.Join(Environment.NewLine, source2.ToList<string>()).TrimEnd(new char[]
					{
						'='
					}).TrimStart(new char[0]);
				}
				else
				{
					message.BodyText = string.Empty;
				}
				IEnumerable<string> source3 = message.BodyComponents.FindAll((string s) => s.StartsWith("Content-Disposition: attachment"));
				message.AttachmentCount = source3.Count<string>();
			}
			return true;
		}

		private bool VerifySubject(Pop3Message message)
		{
			Notification subject = this.WorkDefinition.CheckMail.ExpectedMessage.Subject;
			if (subject == null)
			{
				return true;
			}
			string text = string.Copy(message.Subject).Trim();
			if (!this.WorkDefinition.SendMail.Message.UseSubjectVerbatim)
			{
				int num = text.IndexOf(' ');
				text = ((num > 0) ? text.Substring(num + 1) : string.Empty);
			}
			StringBuilder log = this.GetMailVerificationLog(message.Number);
			this.TraceDebug(log, "Subject='{0}'", new object[]
			{
				text
			});
			bool flag = this.Verify(subject, "Subject", text);
			bool flag2 = true;
			if (subject.Mandatory && ((flag && !subject.MatchExpected) || (!flag && subject.MatchExpected)))
			{
				flag2 = false;
			}
			this.TraceDebug(log, "Type:{0}, Value:'{1}', MatchType:{2}, MatchExpected:{3}, Match:{4}, Verified:{5} ", new object[]
			{
				subject.Type,
				subject.Value,
				subject.Method.ToString(),
				subject.MatchExpected,
				flag,
				flag2
			});
			return flag2;
		}

		private bool VerifyHeader(Pop3Message message)
		{
			IEnumerable<Notification> headers = this.WorkDefinition.CheckMail.ExpectedMessage.Headers;
			if (headers == null || headers.Count<Notification>() == 0)
			{
				return true;
			}
			IEnumerable<string> headerComponents = message.HeaderComponents;
			using (IEnumerator<Notification> enumerator = headers.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					SmtpProbe.<>c__DisplayClass14 CS$<>8__locals1 = new SmtpProbe.<>c__DisplayClass14();
					CS$<>8__locals1.headerToMatch = enumerator.Current;
					string pattern = (!string.IsNullOrEmpty(CS$<>8__locals1.headerToMatch.Value)) ? string.Format("{0}: ", CS$<>8__locals1.headerToMatch.Type) : string.Format("{0}:", CS$<>8__locals1.headerToMatch.Type);
					int index = pattern.Length;
					bool flag = true;
					StringBuilder log = this.GetMailVerificationLog(message.Number);
					foreach (string text in from h in headerComponents
					where h.StartsWith(pattern)
					select h)
					{
						this.TraceDebug(log, "Header='{0}'", new object[]
						{
							text
						});
					}
					if (CS$<>8__locals1.headerToMatch.Method == MatchType.String)
					{
						flag = headerComponents.Any((string header) => header.StartsWith(pattern) && string.Compare(header.Substring(index), CS$<>8__locals1.headerToMatch.Value, true) == 0);
					}
					else if (CS$<>8__locals1.headerToMatch.Method == MatchType.SubString)
					{
						flag = headerComponents.Any((string header) => header.StartsWith(pattern) && header.Substring(index).Contains(CS$<>8__locals1.headerToMatch.Value));
					}
					else if (CS$<>8__locals1.headerToMatch.Method == MatchType.Regex)
					{
						flag = headerComponents.Any((string header) => header.StartsWith(pattern) && Regex.IsMatch(header.Substring(index), CS$<>8__locals1.headerToMatch.Value, RegexOptions.IgnoreCase));
					}
					bool flag2 = true;
					if (CS$<>8__locals1.headerToMatch.Mandatory && ((flag && !CS$<>8__locals1.headerToMatch.MatchExpected) || (!flag && CS$<>8__locals1.headerToMatch.MatchExpected)))
					{
						flag2 = false;
					}
					this.TraceDebug(log, "Tag:{0}, Value:'{1}', MatchType:{2}, MatchExpected:{3}, Match:{4}, Verified:{5}", new object[]
					{
						CS$<>8__locals1.headerToMatch.Type,
						CS$<>8__locals1.headerToMatch.Value,
						CS$<>8__locals1.headerToMatch.Method.ToString(),
						CS$<>8__locals1.headerToMatch.MatchExpected,
						flag,
						flag2
					});
					if (!flag2)
					{
						return false;
					}
				}
			}
			return true;
		}

		private bool VerifyMessageBody(Pop3Message message)
		{
			Notification body = this.WorkDefinition.CheckMail.ExpectedMessage.Body;
			if (body == null)
			{
				return true;
			}
			StringBuilder log = this.GetMailVerificationLog(message.Number);
			this.TraceDebug(log, "BodyText='{0}'", new object[]
			{
				message.BodyText
			});
			bool flag = this.Verify(body, "Body", message.BodyText);
			bool flag2 = true;
			if (body.Mandatory && ((flag && !body.MatchExpected) || (!flag && body.MatchExpected)))
			{
				flag2 = false;
			}
			this.TraceDebug(log, "Type:{0}, Value:'{1}', MatchType:{2}, MatchExpected:{3}, Match:{4}, Verified:{5}", new object[]
			{
				body.Type,
				body.Value,
				body.Method.ToString(),
				body.MatchExpected,
				flag,
				flag2
			});
			return flag2;
		}

		private bool VerifyAttachment(Pop3Message message)
		{
			Notification attachment = this.WorkDefinition.CheckMail.ExpectedMessage.Attachment;
			if (attachment == null)
			{
				return true;
			}
			StringBuilder log = this.GetMailVerificationLog(message.Number);
			this.TraceDebug(log, "Attachment#={0}", new object[]
			{
				message.AttachmentCount
			});
			bool flag = message.AttachmentCount == (int)Convert.ChangeType(attachment.Value, typeof(int));
			bool flag2 = true;
			if (attachment.Mandatory && ((flag && !attachment.MatchExpected) || (!flag && attachment.MatchExpected)))
			{
				flag2 = false;
			}
			this.TraceDebug(log, "Type:{0}, Value:'{1}', MatchExpected:{2}, Match:{3}, Verified:{4}", new object[]
			{
				attachment.Type,
				attachment.Value,
				attachment.MatchExpected,
				flag,
				flag2
			});
			return flag2;
		}

		private bool Verify(Notification matchItem, string typeToVerify, string valueToVerify)
		{
			if (string.Compare(typeToVerify, matchItem.Type, true) != 0)
			{
				return false;
			}
			if (string.IsNullOrEmpty(valueToVerify) && string.IsNullOrEmpty(matchItem.Value))
			{
				return true;
			}
			if (matchItem.Method == MatchType.SubString)
			{
				return valueToVerify.Contains(matchItem.Value);
			}
			if (matchItem.Method == MatchType.Regex)
			{
				return Regex.IsMatch(valueToVerify, matchItem.Value, RegexOptions.IgnoreCase);
			}
			return string.Compare(valueToVerify.Trim(), matchItem.Value, true) == 0;
		}

		private void LogMessage(Pop3Message message, string messageId)
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (message.Components != null)
			{
				stringBuilder.AppendLine(string.Join(Environment.NewLine, message.Components.ToArray()));
			}
			else if (message.HeaderComponents != null)
			{
				stringBuilder.AppendLine(string.Join(Environment.NewLine, message.HeaderComponents.ToArray()));
			}
			WTFDiagnostics.TraceInformation<string, string>(ExTraceGlobals.SMTPTracer, new TracingContext(), "Content of Message '{0}': {1}", messageId, stringBuilder.ToString(), null, "LogMessage", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ForefrontActiveMonitoring\\Components\\Smtp\\Probes\\SmtpProbe.cs", 1890);
			if (!string.IsNullOrWhiteSpace(this.WorkDefinition.CheckMail.LogFileLocation))
			{
				string path = string.Format("{0}\\{1}.txt", this.WorkDefinition.CheckMail.LogFileLocation, messageId);
				using (StreamWriter streamWriter = new StreamWriter(path))
				{
					streamWriter.WriteLine(stringBuilder.ToString());
					streamWriter.Close();
				}
			}
		}

		private bool UpdateDeliveryStatus(Pop3Message message, string messageId, bool verificationSuccess, TimeSpan queryWindow, out bool shouldDelete)
		{
			this.CheckCancellation();
			shouldDelete = false;
			bool flag = false;
			IEnumerable<ProbeStatus> source = from s in this.PreviousSuccessSendResults
			where s.InternalProbeId == messageId
			select s;
			IEnumerable<ProbeStatus> source2 = from s in this.PreviousDeliverResults
			where s.InternalProbeId == messageId
			select s;
			int num = source.Count<ProbeStatus>();
			int num2 = source2.Count<ProbeStatus>();
			if (num == 1 && num2 == 0)
			{
				ProbeStatus probeStatus = source.First<ProbeStatus>();
				ProbeResult probeResult = new ProbeResult(base.Definition);
				SmtpProbe.SetProbeErrorType(probeResult, MailErrorType.Success);
				probeResult.ExecutionStartTime = DateTime.UtcNow;
				probeResult.ExecutionEndTime = DateTime.UtcNow;
				SmtpProbe.SetProbeRecordType(probeResult, RecordType.DeliverMail);
				this.CopySendDataToDeliver(probeStatus, probeResult);
				DateTime t = probeStatus.SentTime + TimeSpan.FromMinutes(this.WorkDefinition.SendMail.Sla);
				DateTime receivedDate = message.ReceivedDate;
				SmtpProbe.SetActualDeliverTime(probeResult, (double)receivedDate.Ticks);
				probeResult.SampleValue = (receivedDate - probeStatus.SentTime).TotalMilliseconds;
				StringBuilder stringBuilder = this.GetMailVerificationLog(message.Number);
				if (stringBuilder != null)
				{
					probeResult.ExecutionContext = stringBuilder.ToString();
				}
				ResultType resultType;
				if (probeStatus.DeliveryExpected)
				{
					if (!verificationSuccess)
					{
						resultType = ResultType.Failed;
						this.HandleVerificationFailure(probeResult);
					}
					else if (DateTime.Compare(receivedDate, t) <= 0)
					{
						resultType = ResultType.Succeeded;
					}
					else
					{
						resultType = ResultType.Failed;
						this.HandleSlaMiss(probeResult);
					}
				}
				else
				{
					resultType = ResultType.Failed;
					this.SetDeliverResultError(probeResult, "Unexpected mail delivery, it was not supposed to be delivered.", new object[0]);
					SmtpProbe.SetProbeErrorType(probeResult, MailErrorType.UnexpectedDelivery);
				}
				probeResult.SetCompleted(resultType);
				if (this.mailDeliveryStatus.TryAdd(SmtpProbe.GetInternalProbeId(probeResult), resultType == ResultType.Succeeded))
				{
					ProbeResult probeResult2 = probeResult;
					probeResult2.ExecutionContext += "[COPIED SEND RESULT EXECUTION CONTEXT START]";
					ProbeResult probeResult3 = probeResult;
					probeResult3.ExecutionContext += probeStatus.ExecutionContext;
					base.Broker.PublishResult(probeResult);
					this.HandleResultPublished(probeResult);
					this.TraceDebug("DeliverMailStatus={0}", new object[]
					{
						(resultType == ResultType.Succeeded) ? "Succeeded" : "Failed"
					});
					flag = true;
				}
			}
			else
			{
				if (num2 >= 1 && num >= 1)
				{
					ProbeStatus probeStatus2 = source.First<ProbeStatus>();
					shouldDelete = true;
					this.TraceDebug("OK to delete", new object[]
					{
						probeStatus2.InternalProbeId
					});
				}
				if (num > 1)
				{
					this.TraceError("Unexpected # of SendMail results={0}, ID={1}", new object[]
					{
						num,
						messageId
					});
				}
				if (num2 > 1)
				{
					this.TraceError("Unexpected # of DeliverMail results={0}, ID={1}", new object[]
					{
						num2,
						messageId
					});
				}
			}
			if (!this.WorkDefinition.CheckMail.VerifyDeliverResultBeforeDelete)
			{
				shouldDelete = flag;
			}
			this.CheckCancellation();
			return flag;
		}

		private void UpdateUndeliveredMessages(Exception ex)
		{
			if (!this.WorkDefinition.CheckMail.Enabled)
			{
				return;
			}
			WTFDiagnostics.TraceInformation(ExTraceGlobals.SMTPTracer, new TracingContext(), "UpdateUndeliveredMessages started.", null, "UpdateUndeliveredMessages", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ForefrontActiveMonitoring\\Components\\Smtp\\Probes\\SmtpProbe.cs", 2043);
			int queryTimeWindow = this.WorkDefinition.CheckMail.QueryTimeWindow;
			DateTime dateTime = base.Result.ExecutionStartTime.AddSeconds((double)(-1 * queryTimeWindow));
			ProbeStatus previousSendStatus = null;
			if (this.AllPreviousSendResults.Any<ProbeStatus>())
			{
				previousSendStatus = (from s in this.AllPreviousSendResults
				orderby s.SentTime
				select s).Last<ProbeStatus>();
			}
			using (List<ProbeStatus>.Enumerator enumerator = this.PreviousSuccessSendResults.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					ProbeStatus sendStatus = enumerator.Current;
					this.CheckCancellation();
					if ((sendStatus.SentTime + TimeSpan.FromMinutes(this.WorkDefinition.SendMail.Sla)).Ticks < dateTime.Ticks)
					{
						IEnumerable<ProbeStatus> enumerable = from d in this.PreviousDeliverResults
						where sendStatus.InternalProbeId == d.InternalProbeId
						select d;
						if (enumerable == null || enumerable.Count<ProbeStatus>() == 0)
						{
							string internalProbeId = sendStatus.InternalProbeId;
							ProbeResult probeResult = new ProbeResult(base.Definition);
							SmtpProbe.SetProbeErrorType(probeResult, MailErrorType.Success);
							probeResult.ExecutionStartTime = DateTime.UtcNow;
							probeResult.ExecutionEndTime = DateTime.UtcNow;
							SmtpProbe.SetProbeRecordType(probeResult, RecordType.DeliverMail);
							this.CopySendDataToDeliver(sendStatus, probeResult);
							if (sendStatus.DeliveryExpected)
							{
								this.HandleDeliveryFailure(probeResult, previousSendStatus, ex);
							}
							else
							{
								this.TraceDebug("Mail was not delivered after {0}s, which was expected, and is considered a success", new object[]
								{
									queryTimeWindow
								});
								probeResult.SetCompleted(ResultType.Succeeded);
							}
							probeResult.SampleValue = TimeSpan.FromSeconds((double)(queryTimeWindow + 1)).TotalMilliseconds;
							if (this.mailDeliveryStatus.TryAdd(internalProbeId, probeResult.ResultType == ResultType.Succeeded))
							{
								ProbeResult probeResult2 = probeResult;
								probeResult2.ExecutionContext += "[COPIED SEND RESULT EXECUTION CONTEXT START]";
								ProbeResult probeResult3 = probeResult;
								probeResult3.ExecutionContext += sendStatus.ExecutionContext;
								base.Broker.PublishResult(probeResult);
								this.HandleResultPublished(probeResult);
								this.TraceError("DeliverMail ID={0}, Status=Failed", new object[]
								{
									internalProbeId
								});
							}
						}
						else if (enumerable.Count<ProbeStatus>() > 1)
						{
							this.TraceError("Unexpected # of DeliverMail results={0}, ID={1}", new object[]
							{
								this.PreviousDeliverResults.Count,
								sendStatus.InternalProbeId
							});
						}
					}
				}
			}
			this.CheckCancellation();
			WTFDiagnostics.TraceInformation(ExTraceGlobals.SMTPTracer, new TracingContext(), "UpdateUndeliveredMessages finished.", null, "UpdateUndeliveredMessages", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ForefrontActiveMonitoring\\Components\\Smtp\\Probes\\SmtpProbe.cs", 2115);
		}

		private void SetDeliverResultError(ProbeResult deliverResult, string format, params object[] args)
		{
			string text = string.Format(format, args);
			string text2 = string.Format("[{0}] [Latency: {1}s] [Sent Time: {2}] [Expected Delivery Time: {3}] [ActualDeliveryTime : {4}] [DeilveryExpected: {3}]", new object[]
			{
				text,
				TimeSpan.FromMilliseconds(deliverResult.SampleValue).TotalSeconds,
				new DateTime((long)SmtpProbe.GetSentTime(deliverResult), DateTimeKind.Utc),
				new DateTime((long)SmtpProbe.GetExpectedDeliverTime(deliverResult), DateTimeKind.Utc),
				new DateTime((long)SmtpProbe.GetActualDeliverTime(deliverResult), DateTimeKind.Utc),
				SmtpProbe.GetDeliveryExpected(deliverResult) ? "true" : "false"
			});
			deliverResult.FailureContext = text2;
			deliverResult.Error = string.Format("{0} {1}", text2, SmtpProbe.GetProbeMailInfo(deliverResult));
			WTFDiagnostics.TraceError(ExTraceGlobals.SMTPTracer, new TracingContext(), deliverResult.Error, null, "SetDeliverResultError", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ForefrontActiveMonitoring\\Components\\Smtp\\Probes\\SmtpProbe.cs", 2140);
		}

		private void SetDeliverResultError(ProbeResult deliverResult, Exception e)
		{
			string text = e.Message + " " + ((e.InnerException == null) ? string.Empty : e.InnerException.Message);
			deliverResult.FailureContext = text;
			deliverResult.ExecutionContext += string.Format("[{0}] ", text);
			deliverResult.Error = string.Format("{0} {1}", text, SmtpProbe.GetProbeMailInfo(deliverResult));
			deliverResult.Exception = e.ToString();
			WTFDiagnostics.TraceError(ExTraceGlobals.SMTPTracer, new TracingContext(), e.ToString(), null, "SetDeliverResultError", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ForefrontActiveMonitoring\\Components\\Smtp\\Probes\\SmtpProbe.cs", 2157);
		}

		private void LogSendMailException(Exception e)
		{
			string text = string.Format("SendMail failed. {0}", e.Message);
			if (e is SmtpException)
			{
				text = text + " SmtpException StatusCode: " + ((SmtpException)e).StatusCode;
			}
			base.Result.FailureContext = text;
			ProbeResult result = base.Result;
			result.ExecutionContext += string.Format("[{0}] ", text);
			WTFDiagnostics.TraceError<string>(ExTraceGlobals.SMTPTracer, new TracingContext(), "SendMail failed. {0}", e.ToString(), null, "LogSendMailException", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ForefrontActiveMonitoring\\Components\\Smtp\\Probes\\SmtpProbe.cs", 2175);
		}

		private void CheckCancellation()
		{
			if (this.CancellationToken.IsCancellationRequested)
			{
				SmtpProbe.SetProbeErrorType(base.Result, MailErrorType.ProbeTimeOut);
				throw new OperationCanceledException();
			}
		}

		private void PublishMailWithNoMessageId(Pop3Message message)
		{
			string text = string.Format("{0}-{1}", base.Id, CombGuidGenerator.NewGuid().ToString());
			this.TraceDebug("Found a matching message and assigned ID: {0}", new object[]
			{
				text
			});
			this.LogMessage(message, text);
			ProbeResult probeResult = new ProbeResult(base.Definition);
			probeResult.ExecutionStartTime = DateTime.UtcNow;
			probeResult.ExecutionEndTime = DateTime.UtcNow;
			SmtpProbe.SetInternalProbeId(probeResult, text);
			SmtpProbe.SetProbeRecordType(probeResult, RecordType.DeliverMail);
			SmtpProbe.SetActualDeliverTime(probeResult, (double)message.ReceivedDate.Ticks);
			probeResult.StateAttribute4 = message.Subject.TrimEnd(Environment.NewLine.ToCharArray());
			probeResult.StateAttribute5 = message.BodyText;
			StringBuilder stringBuilder = this.GetMailVerificationLog(message.Number);
			if (stringBuilder != null)
			{
				probeResult.ExecutionContext = stringBuilder.ToString();
			}
			probeResult.SetCompleted(ResultType.Succeeded);
			base.Broker.PublishResult(probeResult);
			this.TraceDebug("DeliverMail result inserted. ID:{0}", new object[]
			{
				text
			});
		}

		private StringBuilder GetMailVerificationLog(long number)
		{
			StringBuilder result = null;
			this.mailVerificationLog.TryGetValue(number, out result);
			return result;
		}

		private TimeSpan GetInboxQueryWindow()
		{
			return TimeSpan.FromSeconds((double)this.WorkDefinition.CheckMail.QueryTimeWindow) + TimeSpan.FromSeconds((double)base.Definition.RecurrenceIntervalSeconds) + TimeSpan.FromMinutes(5.0);
		}

		private bool ValidateCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors policyErrors)
		{
			if (this.WorkDefinition.SendMail.IgnoreCertificateNameMismatchPolicyError)
			{
				policyErrors &= ~SslPolicyErrors.RemoteCertificateNameMismatch;
			}
			return policyErrors == SslPolicyErrors.None;
		}

		internal static readonly string ProbeErrorTypeAttribute = "StateAttribute1";

		internal static readonly string AuthOnlyAttribute = "StateAttribute6";

		internal static readonly string IpVersionAttribute = "StateAttribute7";

		internal static readonly string PortAttribute = "StateAttribute8";

		internal static readonly string SendMailExecutionIdAttribute = "StateAttribute11";

		internal static readonly string ProbeRecordTypeAttribute = "StateAttribute12";

		internal static readonly string DeliveryExceptedAttribute = "StateAttribute16";

		internal static readonly string SentTimeAttribute = "StateAttribute17";

		internal static readonly string ExpectedDeliverTimeAttribute = "StateAttribute18";

		internal static readonly string ActualDeliverTimeAttribute = "StateAttribute19";

		internal static readonly string InternalProbeIdAttribute = "StateAttribute22";

		internal static readonly string ProbeMailInfoAttribute = "StateAttribute24";

		private static Dictionary<MailErrorType, SmtpFailureCategory> failureComponentMapping = new Dictionary<MailErrorType, SmtpFailureCategory>
		{
			{
				MailErrorType.CheckMailException,
				SmtpFailureCategory.MonitoringComponent
			},
			{
				MailErrorType.FfoAntispamFailure,
				SmtpFailureCategory.MonitoringComponent
			},
			{
				MailErrorType.MailboxLoginFailure,
				SmtpFailureCategory.MonitoringComponent
			},
			{
				MailErrorType.AzureDnsFailure,
				SmtpFailureCategory.MonitoringComponent
			},
			{
				MailErrorType.SqlQueryFailure,
				SmtpFailureCategory.MonitoringComponent
			},
			{
				MailErrorType.PopProxyFailure,
				SmtpFailureCategory.MonitoringComponent
			},
			{
				MailErrorType.FindProbeResultsTimeOut,
				SmtpFailureCategory.MonitoringComponent
			},
			{
				MailErrorType.ConnectorConfigurationError,
				SmtpFailureCategory.MonitoringComponent
			},
			{
				MailErrorType.CheckMailTimeOut,
				SmtpFailureCategory.MonitoringComponent
			},
			{
				MailErrorType.UpdateUndeliveredTimeOut,
				SmtpFailureCategory.MonitoringComponent
			},
			{
				MailErrorType.SlaExceeded,
				SmtpFailureCategory.MonitoringComponent
			},
			{
				MailErrorType.NoDelivery,
				SmtpFailureCategory.MonitoringComponent
			},
			{
				MailErrorType.SaveStatusTimeout,
				SmtpFailureCategory.MonitoringComponent
			},
			{
				MailErrorType.FfoGlsFailure,
				SmtpFailureCategory.DependentComponent
			},
			{
				MailErrorType.ServiceLocatorFailure,
				SmtpFailureCategory.DependentComponent
			},
			{
				MailErrorType.NoDestinationsFailure,
				SmtpFailureCategory.DependentComponent
			},
			{
				MailErrorType.SmtpAuthFailure,
				SmtpFailureCategory.DependentComponent
			},
			{
				MailErrorType.SmtpSendAuthTimeOut,
				SmtpFailureCategory.DependentComponent
			},
			{
				MailErrorType.NetworkingConfigurationFailure,
				SmtpFailureCategory.DependentComponent
			},
			{
				MailErrorType.ActiveDirectoryFailure,
				SmtpFailureCategory.DependentComponent
			},
			{
				MailErrorType.UnableToConnect,
				SmtpFailureCategory.DependentCoveredComponent
			},
			{
				MailErrorType.DnsFailure,
				SmtpFailureCategory.DnsFailure
			}
		};

		private ConcurrentDictionary<string, bool> mailDeliveryStatus = new ConcurrentDictionary<string, bool>();

		private ConcurrentDictionary<long, StringBuilder> mailVerificationLog = new ConcurrentDictionary<long, StringBuilder>();

		private IPop3Client popClient;

		private IMinimalSmtpClientFactory smtpClientFactory;

		private enum IpVersion
		{
			Unspecified,
			Four = 4,
			Six = 6
		}
	}
}
