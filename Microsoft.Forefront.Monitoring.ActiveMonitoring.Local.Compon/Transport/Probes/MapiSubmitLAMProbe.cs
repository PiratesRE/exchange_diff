using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Xml;
using Microsoft.Exchange.Compliance.Xml;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Management.MailboxTransportSubmission.MapiProbe;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Local;
using Microsoft.Forefront.Monitoring.ActiveMonitoring.Smtp.Probes;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring.Transport.Probes
{
	public class MapiSubmitLAMProbe : ProbeWorkItem
	{
		public MapiSubmitLAMProbe()
		{
		}

		internal MapiSubmitLAMProbe(int workItemId, ProbeResult resultInstance, SendMapiMailDefinition sendMapiMailDefinitionInstance, IMapiMessageSubmitter mapiMessageSubmitterInstance, IResultsChecker resultsCheckerInstance, ITracer tracerInstance, int seqNum, bool firstRun)
		{
			this.resultsCheckerInstance = resultsCheckerInstance;
			this.tracerInstance = tracerInstance;
			this.seqNumber = (long)seqNum;
			this.firstRun = firstRun;
			this.lamNotificationId = MapiSubmitLAMProbe.GenerateLamNotificationId(workItemId, (long)seqNum);
			this.previousLamNotificationId = MapiSubmitLAMProbe.GenerateLamNotificationId(workItemId, (long)(seqNum - 1));
			this.probeResultInstance = resultInstance;
			this.sendMapiMailDefinitionInstance = sendMapiMailDefinitionInstance;
			this.mapiMessageSubmitterInstance = mapiMessageSubmitterInstance;
			this.probeId = workItemId;
			this.needsInitalization = false;
		}

		private ProbeResult ProbeResultInstance
		{
			get
			{
				if (this.probeResultInstance == null)
				{
					this.probeResultInstance = base.Result;
				}
				return this.probeResultInstance;
			}
		}

		private SendMapiMailDefinition SendMapiMailDefinitionInstance
		{
			get
			{
				if (this.sendMapiMailDefinitionInstance == null)
				{
					MailboxSelectionResult mailboxSelectionResult;
					this.sendMapiMailDefinitionInstance = SendMapiMailDefinitionFactory.CreateInstance(this.LamNotificationId, base.Definition, this.MailboxProviderInstance, out mailboxSelectionResult);
					this.mailboxSelectionResult = mailboxSelectionResult;
				}
				return this.sendMapiMailDefinitionInstance;
			}
		}

		private IMapiMessageSubmitter MapiMessageSubmitterInstance
		{
			get
			{
				if (this.mapiMessageSubmitterInstance == null)
				{
					this.mapiMessageSubmitterInstance = MapiMessageSubmitter.CreateInstance();
				}
				return this.mapiMessageSubmitterInstance;
			}
		}

		private ITracer TracerInstance
		{
			get
			{
				if (this.tracerInstance == null)
				{
					this.tracerInstance = new Tracer();
				}
				return this.tracerInstance;
			}
		}

		private IResultsChecker ResultsCheckerInstance
		{
			get
			{
				if (this.resultsCheckerInstance == null)
				{
					this.resultsCheckerInstance = new ResultsChecker();
				}
				return this.resultsCheckerInstance;
			}
		}

		private IMailboxProvider MailboxProviderInstance
		{
			get
			{
				if (this.mailboxProviderInstance == null)
				{
					this.mailboxProviderInstance = MailboxProvider.GetInstance();
				}
				return this.mailboxProviderInstance;
			}
		}

		private bool FirstRun
		{
			get
			{
				return this.firstRun;
			}
		}

		private long SequenceNumber
		{
			get
			{
				return this.seqNumber;
			}
		}

		private int ProbeId
		{
			get
			{
				return this.probeId;
			}
		}

		private string LamNotificationId
		{
			get
			{
				return this.lamNotificationId;
			}
		}

		private string FullLamNotificationId
		{
			get
			{
				if (string.IsNullOrEmpty(this.fullLamNotificationId))
				{
					this.fullLamNotificationId = string.Format("MBTSubmission/StoreDriverSubmission/{0}", this.LamNotificationId);
				}
				return this.fullLamNotificationId;
			}
		}

		private string PreviousLamNotificationId
		{
			get
			{
				return this.previousLamNotificationId;
			}
		}

		private MailboxSelectionResult MailboxSelectionResult
		{
			get
			{
				return this.mailboxSelectionResult;
			}
		}

		public override void PopulateDefinition<ProbeDefinition>(ProbeDefinition definition, Dictionary<string, string> propertyBag)
		{
			XmlDocument xmlDocument = new SafeXmlDocument();
			xmlDocument.LoadXml(definition.ExtensionAttributes);
			XmlNode node = xmlDocument.SelectSingleNode("//WorkContext/SendMapiMail/Message");
			List<KeyValuePair<string, string>> list = (from p in propertyBag
			where MapiSubmitLAMProbe.wellKnownProperties.Contains(p.Key)
			select p).ToList<KeyValuePair<string, string>>();
			list.ForEach(delegate(KeyValuePair<string, string> p)
			{
				(node.Attributes[p.Key] ?? node.Attributes.Append(node.OwnerDocument.CreateAttribute(p.Key))).Value = p.Value;
			});
			definition.ExtensionAttributes = xmlDocument.InnerXml;
		}

		internal void DoWorkInternal(CancellationToken cancellationToken)
		{
			this.TraceDebug("MapiSubmitLAMProbe started. This performs - 1. Submits a new message to Store 2. Checks results from previous Send Mail operation.");
			if (!TransportProbeCommon.IsProbeExecutionEnabled())
			{
				this.TraceDebug("MapiSubmitLAMProbe skipped as probe is disabled.");
				return;
			}
			if (this.needsInitalization)
			{
				this.Initialize();
			}
			this.TraceDebug(string.Format("Sequence # = {0}. First Run? = {1}.", this.SequenceNumber, this.FirstRun));
			this.ProbeResultInstance.StateAttribute1 = this.SendMapiMailDefinitionInstance.MessageSubject;
			this.ProbeResultInstance.StateAttribute2 = this.SendMapiMailDefinitionInstance.SenderEmailAddress;
			this.ProbeResultInstance.StateAttribute3 = this.SendMapiMailDefinitionInstance.SenderMbxGuid.ToString();
			this.ProbeResultInstance.StateAttribute4 = this.SendMapiMailDefinitionInstance.SenderMdbGuid.ToString();
			this.ProbeResultInstance.StateAttribute12 = this.SendMapiMailDefinitionInstance.RecipientEmailAddress;
			this.ProbeResultInstance.StateAttribute21 = string.Format("MessageClass:{0};MessageBody:{1}", this.SendMapiMailDefinitionInstance.MessageClass, this.SendMapiMailDefinitionInstance.MessageBody);
			this.ProbeResultInstance.StateAttribute22 = string.Format("DoNotDeliver:{0};DropMessageInHub:{1};DeleteAfterSubmit:{2}", this.SendMapiMailDefinitionInstance.DoNotDeliver.ToString(), this.SendMapiMailDefinitionInstance.DropMessageInHub.ToString(), this.SendMapiMailDefinitionInstance.DeleteAfterSubmit.ToString());
			this.ProbeResultInstance.StateAttribute25 = this.SequenceNumber.ToString();
			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
			bool potentialForAlertBasedOnCurrentRun = false;
			bool potentialForAlertBasedOnPreviousRun = false;
			Exception ex = null;
			Exception ex2 = null;
			string empty = string.Empty;
			string stateAttribute = string.Empty;
			string empty2 = string.Empty;
			Exception ex3 = null;
			string empty3 = string.Empty;
			DateTime utcNow = DateTime.UtcNow;
			double sampleValue = 0.0;
			bool flag4 = false;
			try
			{
				stateAttribute = this.GetPreviousSuccessfulMailLatencies(cancellationToken, out empty2);
			}
			catch (Exception ex4)
			{
				ex3 = ex4;
				this.LogGetLatencyException(ex4);
			}
			try
			{
				potentialForAlertBasedOnPreviousRun = this.RaiseAlertBasedOnPreviousRun(cancellationToken, empty2, out flag, out flag2);
			}
			catch (Exception ex5)
			{
				ex = ex5;
				this.LogCheckMailException(ex5);
			}
			try
			{
				flag3 = this.SendMail(cancellationToken, out empty3, out empty, out utcNow, out sampleValue);
				if (flag3)
				{
					if (!string.IsNullOrEmpty(empty) && !string.IsNullOrEmpty(empty3))
					{
						potentialForAlertBasedOnCurrentRun = false;
					}
					else
					{
						this.TraceError(string.Format("SendMail returned but either internetMessageId:{0} or entryId:{1} was null or empty", empty ?? string.Empty, empty3 ?? string.Empty));
					}
				}
			}
			catch (WrongServerException ex6)
			{
				ex2 = ex6;
				this.LogSendMailException(ex6);
				flag4 = true;
			}
			catch (MailboxOfflineException ex7)
			{
				ex2 = ex7;
				this.LogSendMailException(ex7);
				flag4 = true;
			}
			catch (StorageTransientException ex8)
			{
				ex2 = ex8;
				this.LogSendMailException(ex8);
				flag4 = true;
			}
			catch (StoragePermanentException ex9)
			{
				ex2 = ex9;
				this.LogSendMailException(ex9);
				flag4 = true;
			}
			catch (Exception ex10)
			{
				ex2 = ex10;
				this.LogSendMailException(ex10);
			}
			this.ProbeResultInstance.StateAttribute5 = empty;
			this.ProbeResultInstance.StateAttribute11 = empty3;
			this.ProbeResultInstance.StateAttribute13 = utcNow.ToString();
			this.ProbeResultInstance.StateAttribute14 = ((ex == null) ? string.Empty : ex.ToString());
			this.ProbeResultInstance.StateAttribute15 = ((ex2 == null) ? string.Empty : ex2.ToString());
			this.ProbeResultInstance.StateAttribute23 = stateAttribute;
			this.ProbeResultInstance.StateAttribute24 = ((ex3 == null) ? string.Empty : ex3.ToString());
			this.ProbeResultInstance.SampleValue = sampleValue;
			this.ProbeResultInstance.StateAttribute7 = (double)(flag ? 0 : 1);
			this.ProbeResultInstance.StateAttribute8 = (double)(flag2 ? 1 : 0);
			this.ProbeResultInstance.StateAttribute9 = (double)(flag3 ? 1 : 0);
			if (this.MailboxSelectionResult == MailboxSelectionResult.NoMonitoringMDBs)
			{
				this.ProbeResultInstance.StateAttribute9 = 2.0;
				ProbeResult probeResult = this.ProbeResultInstance;
				probeResult.StateAttribute15 += this.MailboxSelectionResult;
				potentialForAlertBasedOnCurrentRun = false;
			}
			if (this.MailboxSelectionResult == MailboxSelectionResult.NoMonitoringMDBsAreActive)
			{
				this.ProbeResultInstance.StateAttribute9 = 3.0;
				ProbeResult probeResult2 = this.ProbeResultInstance;
				probeResult2.StateAttribute15 += this.MailboxSelectionResult;
				potentialForAlertBasedOnCurrentRun = false;
			}
			if (!flag3 && flag4)
			{
				this.ProbeResultInstance.StateAttribute9 = 4.0;
				potentialForAlertBasedOnCurrentRun = false;
			}
			this.TracerInstance.TraceDebug(this.ProbeResultInstance.ExecutionContext);
			this.PerformProbeFinalAction(potentialForAlertBasedOnPreviousRun, potentialForAlertBasedOnCurrentRun, ex, ex2, utcNow);
		}

		protected override void DoWork(CancellationToken cancellationToken)
		{
			try
			{
				this.DoWorkInternal(cancellationToken);
			}
			catch (EndpointManagerEndpointUninitializedException)
			{
				ProbeResult result = base.Result;
				result.ExecutionContext += " Probe ended due to EndpointManagerEndpointUninitializedException, ignoring exception and treating as transient";
			}
		}

		protected void VerifyPreviousRunMail(CancellationToken cancellationToken, out bool noPreviousMail, out bool startedSMTPOutOperation, out bool finishedSMTPOutOperation, out bool smtpOutThrewException, out bool sendAsCheckCalledDuringPreviousRun, out bool calledDoneWithMessage)
		{
			noPreviousMail = false;
			sendAsCheckCalledDuringPreviousRun = false;
			startedSMTPOutOperation = false;
			finishedSMTPOutOperation = false;
			smtpOutThrewException = false;
			calledDoneWithMessage = false;
			if (this.FirstRun)
			{
				noPreviousMail = true;
				this.TraceDebug("No previous results to verify. ");
				return;
			}
			if (this.ResultsCheckerInstance.LastSendMailFailed(cancellationToken, Settings.DeploymentId, this.SequenceNumber - 1L, 60, this.ProbeResultInstance.ResultName, this.probeId, MapiSubmitLAMProbe.traceContext))
			{
				noPreviousMail = true;
				this.TraceDebug("Previous mail submission to store failed, no verification required.");
				return;
			}
			this.TraceDebug("Previous mail submission to store was successful. Results - ");
			List<ProbeResult> previousResults = this.ResultsCheckerInstance.GetPreviousResults(cancellationToken, Settings.DeploymentId, this.PreviousLamNotificationId, 60, MapiSubmitLAMProbe.traceContext);
			this.TraceDebug(string.Format("# of previous results: {0}. ", previousResults.Count));
			List<Stage> stagesInExtensionXml = this.GetStagesInExtensionXml(previousResults);
			if (stagesInExtensionXml == null || stagesInExtensionXml.Count == 0)
			{
				this.TraceDebug("Could Not Find stages that ran. ");
				return;
			}
			this.TraceDebug("Examining stages that ran. Found - ");
			foreach (Stage stage in stagesInExtensionXml)
			{
				this.TraceDebug(string.Format("{0}; ", stage.ToString()));
				Stage stage2 = stage;
				if (stage2 != Stage.SendAsCheck)
				{
					switch (stage2)
					{
					case Stage.StartedSMTPOutOperation:
						startedSMTPOutOperation = true;
						break;
					case Stage.FinishedSMTPOutOperation:
						finishedSMTPOutOperation = true;
						break;
					case Stage.SMTPOutThrewException:
						smtpOutThrewException = true;
						break;
					case Stage.DoneWithMessage:
						calledDoneWithMessage = true;
						break;
					}
				}
				else
				{
					sendAsCheckCalledDuringPreviousRun = true;
				}
			}
		}

		protected string GetPreviousSuccessfulMailLatencies(CancellationToken cancellationToken, out string immediatePreviousMessageIdPlusLatency)
		{
			immediatePreviousMessageIdPlusLatency = string.Empty;
			List<ProbeResult> previousNSpecificStageResults = this.ResultsCheckerInstance.GetPreviousNSpecificStageResults(cancellationToken, Settings.DeploymentId, "MBTSubmission/StoreDriverSubmission", 1440, 5, "<StateAttribute5>EventHandled</StateAttribute5>", MapiSubmitLAMProbe.traceContext);
			if (previousNSpecificStageResults == null || previousNSpecificStageResults.Count == 0)
			{
				return string.Empty;
			}
			StringBuilder stringBuilder = new StringBuilder();
			foreach (ProbeResult probeResult in previousNSpecificStageResults)
			{
				if (!string.IsNullOrWhiteSpace(probeResult.ExtensionXml))
				{
					XmlNode propertiesNode = this.GetPropertiesNode(probeResult.ExtensionXml);
					if (propertiesNode != null)
					{
						stringBuilder.Append(this.GetPropertyRaw(propertiesNode, "StateAttribute3", string.Empty));
						stringBuilder.Append(",,");
						stringBuilder.Append(this.GetPropertyRaw(propertiesNode, "StateAttribute2", string.Empty));
						stringBuilder.Append(";;");
						if (string.IsNullOrEmpty(immediatePreviousMessageIdPlusLatency))
						{
							immediatePreviousMessageIdPlusLatency = stringBuilder.ToString();
						}
					}
				}
			}
			return stringBuilder.ToString();
		}

		protected bool SendMail(CancellationToken cancellationToken, out string entryId, out string internetMessageId, out DateTime timeMessageSentToStore, out double timeToSendTheMessageToStore)
		{
			entryId = string.Empty;
			internetMessageId = string.Empty;
			timeMessageSentToStore = DateTime.UtcNow;
			timeToSendTheMessageToStore = 0.0;
			this.TraceDebug("In SendMail - ");
			cancellationToken.ThrowIfCancellationRequested();
			this.TraceDebug(string.Format("NotificationID={0}", this.LamNotificationId));
			DateTime utcNow = DateTime.UtcNow;
			switch (this.MailboxSelectionResult)
			{
			case MailboxSelectionResult.Success:
				this.TraceDebug("Sending mail.");
				break;
			case MailboxSelectionResult.NoMonitoringMDBs:
				this.TraceDebug("No MDB was found. LocalEndpointManager.Instance.MailboxDatabaseEndpoint.MailboxDatabaseInfoCollectionForBackend did not return any mdb. Sender and Recipient values are not set. SendMail Skipped.");
				return false;
			case MailboxSelectionResult.NoMonitoringMDBsAreActive:
				this.TraceDebug("No active MDB was found. Sender and Recipient values are not set");
				return false;
			}
			Guid guid;
			this.MapiMessageSubmitterInstance.SendMapiMessage(this.FullLamNotificationId, this.SendMapiMailDefinitionInstance, out entryId, out internetMessageId, out guid);
			DateTime utcNow2 = DateTime.UtcNow;
			this.TraceDebug("SendMail finished.");
			timeMessageSentToStore = utcNow2;
			timeToSendTheMessageToStore = (utcNow2 - utcNow).TotalMilliseconds;
			return true;
		}

		private static string GenerateLamNotificationId(int probeId, long sequence)
		{
			return Guid.Parse(string.Format("{0:X8}-{1:X4}-{2:X4}-{3:X4}-{4:X12}", new object[]
			{
				probeId,
				0,
				0,
				0,
				(int)sequence
			})).ToString();
		}

		private void Initialize()
		{
			this.probeId = base.Id;
			this.seqNumber = ProbeRunSequence.GetProbeRunSequenceNumber(this.probeId.ToString(), out this.firstRun);
			this.lamNotificationId = MapiSubmitLAMProbe.GenerateLamNotificationId(this.probeId, this.SequenceNumber);
			this.previousLamNotificationId = MapiSubmitLAMProbe.GenerateLamNotificationId(this.probeId, this.SequenceNumber - 1L);
		}

		private List<Stage> GetStagesInExtensionXml(List<ProbeResult> results)
		{
			List<Stage> list = new List<Stage>();
			foreach (ProbeResult probeResult in results)
			{
				if (!string.IsNullOrWhiteSpace(probeResult.ExtensionXml))
				{
					XmlNode propertiesNode = this.GetPropertiesNode(probeResult.ExtensionXml);
					if (propertiesNode != null)
					{
						list.Add(this.GetProperty(propertiesNode, "StateAttribute5", Stage.None));
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

		private Stage GetProperty(XmlNode properties, string propertyName, Stage defaultValue)
		{
			string propertyRaw = this.GetPropertyRaw(properties, propertyName, string.Empty);
			if (string.IsNullOrEmpty(propertyRaw))
			{
				return defaultValue;
			}
			return (Stage)Enum.Parse(typeof(Stage), propertyRaw);
		}

		private string GetPropertyRaw(XmlNode properties, string propertyName, string defaultValue)
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

		private void LogCheckMailException(Exception e)
		{
			this.LogException(e, "Checkmail");
		}

		private void LogSendMailException(Exception e)
		{
			this.LogException(e, "SendMail");
		}

		private void LogGetLatencyException(Exception e)
		{
			this.LogException(e, "getlatency");
		}

		private void LogException(Exception e, string context)
		{
			if (e.Message == "Cancellation requested.")
			{
				this.TraceError("Cancellation requested.");
				return;
			}
			string text = string.Format("{0} failed. Exception: {1}.", context, e.ToString());
			ProbeResult probeResult = this.ProbeResultInstance;
			probeResult.FailureContext += text;
			this.TracerInstance.TraceError(text);
		}

		private void TraceDebug(string info)
		{
			ProbeResult probeResult = this.ProbeResultInstance;
			probeResult.ExecutionContext = probeResult.ExecutionContext + info + " ";
			this.TracerInstance.TraceDebug(info);
		}

		private void TraceError(string error)
		{
			ProbeResult probeResult = this.ProbeResultInstance;
			probeResult.ExecutionContext = probeResult.ExecutionContext + error + " ";
			this.TracerInstance.TraceError(error);
		}

		private bool RaiseAlertBasedOnSALatency(string previousLatencyCompoundString)
		{
			if (string.IsNullOrEmpty(previousLatencyCompoundString))
			{
				return false;
			}
			string[] array = previousLatencyCompoundString.Split(new char[]
			{
				',',
				','
			}, StringSplitOptions.RemoveEmptyEntries);
			if (array.Length != 2)
			{
				return false;
			}
			string text = array[1].ToUpper();
			if (text.Contains("SA="))
			{
				int num = 0;
				int num2 = text.IndexOf("SA=", StringComparison.OrdinalIgnoreCase);
				int num3 = text.IndexOf("|", num2, StringComparison.OrdinalIgnoreCase);
				if (num3 < 0)
				{
					num3 = text.Length - 2;
				}
				return int.TryParse(text.Substring(num2 + 6, num3 - num2 - 6), out num) && num < 300;
			}
			return false;
		}

		private bool RaiseAlertBasedOnPreviousRun(CancellationToken cancellationToken, string immediatePreviousMessageIdPlusLatency, out bool noPreviousMail, out bool previousRunCalledDoneWithMessage)
		{
			noPreviousMail = false;
			previousRunCalledDoneWithMessage = false;
			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
			bool flag4 = false;
			this.VerifyPreviousRunMail(cancellationToken, out noPreviousMail, out flag2, out flag3, out flag4, out flag, out previousRunCalledDoneWithMessage);
			if (noPreviousMail)
			{
				return false;
			}
			this.TraceDebug("Previous SendMail failure - ");
			if (!flag)
			{
				this.TraceDebug("Mail submitted to Store during the previous run never reached SendAsCheck. \r\n                        This may indicate a latency from Store to Submission Service. Investigating. ");
				if (!this.RaiseAlertBasedOnSALatency(immediatePreviousMessageIdPlusLatency))
				{
					this.TraceDebug("Found High or no SA for previous successful run. Skip count towards alert.");
					return false;
				}
				this.TraceDebug("Found lower SA latency. Indicates an issue in Submission service. \r\n                            Investigate.");
				return true;
			}
			else
			{
				if (!flag2)
				{
					this.TraceDebug("Mail submitted during the previous run never reached SMTP Out. \r\n                            Indicates an issue or latency in the Submission service. Verify\r\n                            the stages/traces to identify root cause of the issue");
					return true;
				}
				if (flag4)
				{
					this.TraceDebug("Mail submitted during the previous run attempted to reach Hub via\r\n                            SMTPOut but failed with an unhandled exception. Submission service is not at fault. ");
					return false;
				}
				if (!flag3)
				{
					this.TraceDebug("Mail submitted during the previous run started SMTPOut but didn't finish it, indicating latency\r\n                            on the SMTPOut side. Submission service is not at fault. ");
					return false;
				}
				if (!previousRunCalledDoneWithMessage)
				{
					this.TraceDebug("Mail submitted during the previous run finished SMTPOut (or timed out) but DoneWithMessage was not called. This is \r\n                            called for Success/NoRecipients/NDRGenerated error codes. For other cases like RetrySMTP/PermananentNDRGenerationFailure\r\n                            etc, its not called. Investigate");
					return true;
				}
				this.TraceDebug("None. Previous Run Verification succeeded");
				return false;
			}
		}

		private void PerformProbeFinalAction(bool potentialForAlertBasedOnPreviousRun, bool potentialForAlertBasedOnCurrentRun, Exception previousRunVerificationException, Exception currentRunException, DateTime timeMessageSentToStore)
		{
			if (!potentialForAlertBasedOnPreviousRun && !potentialForAlertBasedOnCurrentRun)
			{
				string info = string.Format("MapiSubmitLAMProbe run finished successfully for both CheckPreviousMail and SendCurrentMail at {0}.", timeMessageSentToStore);
				this.TraceDebug(info);
				return;
			}
			if (potentialForAlertBasedOnCurrentRun && potentialForAlertBasedOnPreviousRun)
			{
				ProbeResult probeResult = this.ProbeResultInstance;
				probeResult.FailureContext += "MapiSubmitLAMProbe finished with both CheckPreviousMail and SendMail failure.";
				this.TraceError("MapiSubmitLAMProbe finished with both CheckPreviousMail and SendMail failure.");
				if (currentRunException != null)
				{
					throw currentRunException;
				}
				if (previousRunVerificationException == null)
				{
					throw new ApplicationException("MapiSubmitLAMProbe finished with both CheckPreviousMail and SendMail failure.");
				}
				throw previousRunVerificationException;
			}
			else
			{
				if (potentialForAlertBasedOnCurrentRun)
				{
					ProbeResult probeResult2 = this.ProbeResultInstance;
					probeResult2.FailureContext += "MapiSubmitLAMProbe finished with SendMail failure.";
					this.TraceError("MapiSubmitLAMProbe finished with SendMail failure.");
					if (currentRunException == null)
					{
						currentRunException = new ApplicationException("MapiSubmitLAMProbe finished with SendMail failure.");
					}
					throw currentRunException;
				}
				ProbeResult probeResult3 = this.ProbeResultInstance;
				probeResult3.FailureContext += "MapiSubmitLAMProbe finished with CheckPreviousMail failure.";
				this.TraceError("MapiSubmitLAMProbe finished with CheckPreviousMail failure.");
				if (previousRunVerificationException == null)
				{
					previousRunVerificationException = new ApplicationException("MapiSubmitLAMProbe finished with CheckPreviousMail failure.");
				}
				throw previousRunVerificationException;
			}
		}

		internal const string StoreDriverSubmissionLamNotificationIdPrefix = "MBTSubmission/StoreDriverSubmission";

		private const string CancellationMessage = "Cancellation requested.";

		private const string StoreDriverSubmissionLamNotificationIdFormat = "MBTSubmission/StoreDriverSubmission/{0}";

		private const string SearchStringInExtensionXml = "<StateAttribute5>EventHandled</StateAttribute5>";

		private const int NumofMinutesToLookBackToVerifyPreviousRun = 60;

		private const int NumofMinutesToLookBackToGetPreviousLatencies = 1440;

		private const int NumofPreviousLatencyResultsToReturn = 5;

		private const int SaLatencyLimit = 300;

		private static HashSet<string> wellKnownProperties = new HashSet<string>(new string[]
		{
			"DeleteAfterSubmit",
			"DropMessageInHub",
			"DoNotDeliver",
			"MessageClass",
			"Body",
			"Subject",
			"Message"
		});

		private static TracingContext traceContext = new TracingContext();

		private readonly bool needsInitalization = true;

		private long seqNumber;

		private string lamNotificationId;

		private string fullLamNotificationId;

		private bool firstRun;

		private int probeId;

		private string previousLamNotificationId;

		private MailboxSelectionResult mailboxSelectionResult;

		private IMapiMessageSubmitter mapiMessageSubmitterInstance;

		private ITracer tracerInstance;

		private IResultsChecker resultsCheckerInstance;

		private IMailboxProvider mailboxProviderInstance;

		private ProbeResult probeResultInstance;

		private SendMapiMailDefinition sendMapiMailDefinitionInstance;
	}
}
