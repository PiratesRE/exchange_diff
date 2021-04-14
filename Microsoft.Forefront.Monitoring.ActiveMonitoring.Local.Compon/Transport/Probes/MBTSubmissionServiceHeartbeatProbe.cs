using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Xml;
using Microsoft.Exchange.Compliance.Xml;
using Microsoft.Exchange.Management.MailboxTransportSubmission.MapiProbe;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Common;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Local;
using Microsoft.Forefront.Monitoring.ActiveMonitoring.Smtp.Probes;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring.Transport.Probes
{
	public class MBTSubmissionServiceHeartbeatProbe : ProbeWorkItem
	{
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

		private long SequenceNumber
		{
			get
			{
				return this.seqNumber;
			}
		}

		private bool FirstRun
		{
			get
			{
				return this.firstRun;
			}
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

		private MailboxDatabaseSelectionResult MailboxDatabaseSelectionResult
		{
			get
			{
				return this.mailboxDatabaseSelectionResult;
			}
		}

		private string LamNotificationId
		{
			get
			{
				return this.lamNotificationId;
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

		private SendMapiMailDefinition SendMapiMailDefinitionInstance
		{
			get
			{
				if (this.sendMapiMailDefinitionInstance == null)
				{
					this.sendMapiMailDefinitionInstance = SendMapiMailDefinitionFactory.CreateMapiMailInstance(this.LamNotificationId, base.Definition);
				}
				return this.sendMapiMailDefinitionInstance;
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

		internal void DoWorkInternal(CancellationToken cancellationToken)
		{
			this.TraceDebug("MBTSubmissionServiceHeartbeatProbe started. This performs - 1. Submits a new message to Store 2. checks the crimson channel for MBTSubmissionServiceNotifyMapiLogger Event.");
			if (!TransportProbeCommon.IsProbeExecutionEnabled())
			{
				this.TraceDebug("MBTSubmissionServiceHeartbeatProbe skipped as probe is disabled.");
				return;
			}
			this.Initialize();
			this.TraceDebug(string.Format("Sequence # = {0}. First Run? = {1}.", this.SequenceNumber, this.FirstRun));
			bool potentialForAlertBasedOnCurrentRun = false;
			bool potentialForAlertBasedOneventlog = false;
			Exception ex = null;
			Exception ex2 = null;
			DateTime utcNow = DateTime.UtcNow;
			bool flag = this.SendMapiMessage(ref potentialForAlertBasedOnCurrentRun, ref ex);
			if (flag)
			{
				this.ProbeResultInstance.StateAttribute1 = this.SendMapiMailDefinitionInstance.MessageSubject;
				this.ProbeResultInstance.StateAttribute2 = this.SendMapiMailDefinitionInstance.SenderEmailAddress;
				this.ProbeResultInstance.StateAttribute3 = this.SendMapiMailDefinitionInstance.SenderMbxGuid.ToString();
				this.ProbeResultInstance.StateAttribute4 = this.SendMapiMailDefinitionInstance.SenderMdbGuid.ToString();
				this.ProbeResultInstance.StateAttribute5 = this.SendMapiMailDefinitionInstance.RecipientEmailAddress;
				try
				{
					potentialForAlertBasedOneventlog = this.RaiseAlertBasedOnEventLog(cancellationToken);
				}
				catch (Exception ex3)
				{
					ex2 = ex3;
					potentialForAlertBasedOneventlog = true;
				}
				this.ProbeResultInstance.StateAttribute14 = ((ex2 == null) ? string.Empty : ex2.ToString());
				this.ProbeResultInstance.StateAttribute15 = ((ex == null) ? string.Empty : ex.ToString());
				this.PerformProbeFinalAction(potentialForAlertBasedOneventlog, potentialForAlertBasedOnCurrentRun, ex2, ex, utcNow);
				return;
			}
			this.ProbeResultInstance.StateAttribute13 = "Unable to Send Mapi Message";
			this.TraceDebug(string.Format("Send Mapi Message exception {0}", ex));
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

		private bool SendMapiMessage(ref bool potentialForAlertBasedOnCurrentRun, ref Exception currentRunException)
		{
			string empty = string.Empty;
			string empty2 = string.Empty;
			ICollection<MailboxDatabaseInfo> collection;
			this.mailboxDatabaseSelectionResult = this.MailboxProviderInstance.GetAllMailboxDatabaseInfo(out collection);
			this.ProbeResultInstance.StateAttribute11 = this.MailboxDatabaseSelectionResult.ToString();
			if (this.mailboxDatabaseSelectionResult == MailboxDatabaseSelectionResult.Success)
			{
				Random rnd = new Random((int)DateTime.UtcNow.Ticks & 65535);
				collection = (from MailboxDatabaseInfo in collection
				orderby rnd.Next()
				select MailboxDatabaseInfo).ToList<MailboxDatabaseInfo>();
				foreach (MailboxDatabaseInfo mailboxDatabaseInfo in collection)
				{
					if (DirectoryAccessor.Instance.IsDatabaseCopyActiveOnLocalServer(mailboxDatabaseInfo.MailboxDatabaseGuid))
					{
						this.SendMapiMailDefinitionInstance.SenderMbxGuid = mailboxDatabaseInfo.MonitoringAccountMailboxGuid;
						this.SendMapiMailDefinitionInstance.SenderMdbGuid = mailboxDatabaseInfo.MailboxDatabaseGuid;
						this.SendMapiMailDefinitionInstance.SenderEmailAddress = string.Format("{0}@{1}", mailboxDatabaseInfo.MonitoringAccount, mailboxDatabaseInfo.MonitoringAccountDomain);
						this.SendMapiMailDefinitionInstance.RecipientEmailAddress = string.Format("{0}@{1}", mailboxDatabaseInfo.MonitoringAccount, mailboxDatabaseInfo.MonitoringAccountDomain);
						try
						{
							Guid guid;
							this.MapiMessageSubmitterInstance.SendMapiMessage(this.FullLamNotificationId, this.SendMapiMailDefinitionInstance, out empty2, out empty, out guid);
							if (!string.IsNullOrEmpty(empty) && !string.IsNullOrEmpty(empty2))
							{
								potentialForAlertBasedOnCurrentRun = false;
							}
							else
							{
								this.TraceError(string.Format("SendMail returned but either internetMessageId:{0} or entryId:{1} was null or empty", empty ?? string.Empty, empty2 ?? string.Empty));
							}
							this.ProbeResultInstance.StateAttribute21 = "Mapi Message Sent Successfully";
							return true;
						}
						catch (Exception ex)
						{
							currentRunException = ex;
							this.LogSendMailException(ex);
							ProbeResult probeResult = this.ProbeResultInstance;
							probeResult.StateAttribute12 = probeResult.StateAttribute12 + ex.GetType().FullName + "\n";
						}
					}
				}
				return false;
			}
			return false;
		}

		private bool RaiseAlertBasedOnEventLog(CancellationToken cancellationToken)
		{
			if (this.FirstRun)
			{
				this.TraceDebug("No events to verify ");
				return false;
			}
			XmlDocument xmlDocument = new SafeXmlDocument();
			xmlDocument.LoadXml(base.Definition.ExtensionAttributes);
			XmlElement xmlElement = Utils.CheckXmlElement(xmlDocument.SelectSingleNode("//MBTSubmissionServiceHeartbeatProbeParam"), "//MBTSubmissionServiceHeartbeatProbeParam");
			string attribute = xmlElement.GetAttribute("NumofMinutesToLookBack");
			int numofMinutesToLookBack;
			if (!int.TryParse(attribute, out numofMinutesToLookBack))
			{
				numofMinutesToLookBack = 15;
			}
			List<ProbeResult> previousProbeResults = this.ResultsCheckerInstance.GetPreviousProbeResults(cancellationToken, numofMinutesToLookBack, "MailboxTransport/MBTSubmissionServiceNotifyMapiLogger", MBTSubmissionServiceHeartbeatProbe.traceContext);
			this.TraceDebug(string.Format("# of previous results: {0}. ", previousProbeResults.Count));
			if (previousProbeResults == null || previousProbeResults.Count == 0)
			{
				this.TraceDebug("Could Not Find any Submission service heartbeat event");
				return true;
			}
			return false;
		}

		private string GenerateLamNotificationId(int probeId, long sequence)
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
			this.lamNotificationId = this.GenerateLamNotificationId(this.probeId, this.SequenceNumber);
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

		private void LogSendMailException(Exception e)
		{
			this.LogException(e, "SendMail");
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

		private void PerformProbeFinalAction(bool potentialForAlertBasedOneventlog, bool potentialForAlertBasedOnCurrentRun, Exception eventlogException, Exception currentRunException, DateTime timeMessageSentToStore)
		{
			if (!potentialForAlertBasedOneventlog && !potentialForAlertBasedOnCurrentRun)
			{
				string info = string.Format("MBTSubmissionServiceHeartbeatProbe run finished successfully for both checkeventlog and SendCurrentMail at {0}.", timeMessageSentToStore);
				this.TraceDebug(info);
				return;
			}
			if (potentialForAlertBasedOnCurrentRun && potentialForAlertBasedOneventlog)
			{
				ProbeResult probeResult = this.ProbeResultInstance;
				probeResult.FailureContext += "MBTSubmissionServiceHeartbeatProbe finished with both checkeventlog and SendMail failure.";
				this.TraceError("MBTSubmissionServiceHeartbeatProbe finished with both checkeventlog and SendMail failure.");
				if (eventlogException == null)
				{
					throw new ApplicationException("MBTSubmissionServiceHeartbeatProbe finished with both checkeventlog and SendMail failure.");
				}
				throw eventlogException;
			}
			else
			{
				if (potentialForAlertBasedOneventlog)
				{
					ProbeResult probeResult2 = this.ProbeResultInstance;
					probeResult2.FailureContext += "MBTSubmissionServiceHeartbeatProbe finished with checkeventlog failure.";
					this.TraceError("MBTSubmissionServiceHeartbeatProbe finished with checkeventlog failure.");
					if (eventlogException == null)
					{
						eventlogException = new ApplicationException("MBTSubmissionServiceHeartbeatProbe finished with checkeventlog failure.");
					}
					throw eventlogException;
				}
				return;
			}
		}

		internal const string StoreDriverSubmissionNotificationIdPrefix = "MBTSubmission/StoreDriverSubmission";

		private const string StoreDriverSubmissionNotificationIdFormat = "MBTSubmission/StoreDriverSubmission/{0}";

		private const int DefaultNumberOfMinutesToLookBackVerifyEventLog = 15;

		private const string CancellationMessage = "Cancellation requested.";

		private const string ResultName = "MailboxTransport/MBTSubmissionServiceNotifyMapiLogger";

		private const string NumberOfMinutesToLookBackXmlAttribute = "NumofMinutesToLookBack";

		private const bool NeedsInitalization = true;

		private const string ProbeParamXmlNodeString = "//MBTSubmissionServiceHeartbeatProbeParam";

		private static TracingContext traceContext = new TracingContext();

		private long seqNumber;

		private int probeId;

		private bool firstRun;

		private ProbeResult probeResultInstance;

		private ITracer tracerInstance;

		private MailboxDatabaseSelectionResult mailboxDatabaseSelectionResult;

		private string lamNotificationId;

		private string fullLamNotificationId;

		private IMapiMessageSubmitter mapiMessageSubmitterInstance;

		private SendMapiMailDefinition sendMapiMailDefinitionInstance;

		private IMailboxProvider mailboxProviderInstance;

		private IResultsChecker resultsCheckerInstance;
	}
}
