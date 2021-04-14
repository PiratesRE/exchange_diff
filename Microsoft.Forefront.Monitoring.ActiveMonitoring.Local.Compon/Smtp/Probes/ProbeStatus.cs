using System;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring.Smtp.Probes
{
	public class ProbeStatus
	{
		public ProbeStatus(StatusEntry entry)
		{
			if (entry == null)
			{
				throw new ArgumentException("Collection cannot be null");
			}
			this.ExecutionContext = entry["executionContext"];
			this.ResultType = (ResultType)Enum.Parse(typeof(ResultType), entry["resultType"]);
			this.InternalProbeId = entry["internalProbeId"];
			this.ProbeMailInfo = entry["probeMailInfo"];
			this.SendMailExecutionId = entry["sendMailExecutionId"];
			this.DeliveryExpected = bool.Parse(entry["deliveryExpected"]);
			this.SentTime = DateTime.Parse(entry["sentTime"]);
			this.RecordType = (RecordType)Enum.Parse(typeof(RecordType), entry["recordType"]);
			this.ProbeErrorType = (MailErrorType)Enum.Parse(typeof(MailErrorType), entry["probeErrorType"]);
			this.EhloIssued = entry["ehloIssued"];
			this.ExchangeMessageId = entry["exchangeMessageId"];
			this.FDServerEncountered = entry["fdServerEncountered"];
			this.SmtpResponseReceived = entry["smtpResponseReceived"];
			this.TargetVIP = entry["targetVip"];
			this.HubServer = entry["hubServer"];
		}

		public ProbeStatus(ProbeResult result)
		{
			if (result == null)
			{
				throw new ArgumentException("Collection cannot be null");
			}
			this.ExecutionContext = result.ExecutionContext;
			this.ResultType = result.ResultType;
			this.InternalProbeId = SmtpProbe.GetInternalProbeId(result);
			this.ProbeMailInfo = SmtpProbe.GetProbeMailInfo(result);
			this.SendMailExecutionId = SmtpProbe.GetSendMailExecutionId(result);
			this.DeliveryExpected = SmtpProbe.GetDeliveryExpected(result);
			this.SentTime = new DateTime((long)SmtpProbe.GetSentTime(result));
			this.RecordType = (RecordType)Enum.Parse(typeof(RecordType), SmtpProbe.GetProbeRecordType(result));
			this.ProbeErrorType = (MailErrorType)Enum.Parse(typeof(MailErrorType), SmtpProbe.GetProbeErrorType(result));
			this.EhloIssued = BucketedSmtpProbe.GetEhloIssued(result);
			this.ExchangeMessageId = BucketedSmtpProbe.GetExchangeMessageID(result);
			this.FDServerEncountered = BucketedSmtpProbe.GetFDServerEncountered(result);
			this.SmtpResponseReceived = BucketedSmtpProbe.GetSmtpResponseReceived(result);
			this.TargetVIP = BucketedSmtpProbe.GetTargetVIP(result);
			this.HubServer = BucketedSmtpProbe.GetHubServer(result);
		}

		public string InternalProbeId { get; private set; }

		public string ProbeMailInfo { get; private set; }

		public string SendMailExecutionId { get; private set; }

		public string EhloIssued { get; private set; }

		public string ExchangeMessageId { get; private set; }

		public string FDServerEncountered { get; private set; }

		public string SmtpResponseReceived { get; private set; }

		public string TargetVIP { get; private set; }

		public string HubServer { get; private set; }

		public string ExecutionContext { get; private set; }

		public bool DeliveryExpected { get; private set; }

		public DateTime SentTime { get; private set; }

		public RecordType RecordType { get; private set; }

		public MailErrorType ProbeErrorType { get; private set; }

		public ResultType ResultType { get; private set; }

		public void CreateStatusEntry(StatusEntryCollection collection)
		{
			if (collection == null)
			{
				throw new ArgumentException("Collection cannot be null");
			}
			StatusEntry statusEntry = collection.CreateStatusEntry();
			statusEntry["executionContext"] = this.ExecutionContext;
			statusEntry["resultType"] = this.ResultType.ToString();
			statusEntry["internalProbeId"] = this.InternalProbeId;
			statusEntry["probeMailInfo"] = this.ProbeMailInfo;
			statusEntry["sendMailExecutionId"] = this.SendMailExecutionId;
			statusEntry["deliveryExpected"] = this.DeliveryExpected.ToString();
			statusEntry["sentTime"] = this.SentTime.ToString();
			statusEntry["recordType"] = this.RecordType.ToString();
			statusEntry["probeErrorType"] = this.ProbeErrorType.ToString();
			statusEntry["ehloIssued"] = this.EhloIssued;
			statusEntry["exchangeMessageId"] = this.ExchangeMessageId;
			statusEntry["fdServerEncountered"] = this.FDServerEncountered;
			statusEntry["smtpResponseReceived"] = this.SmtpResponseReceived;
			statusEntry["targetVip"] = this.TargetVIP;
			statusEntry["hubServer"] = this.HubServer;
		}

		private const string InternalProbeIdKey = "internalProbeId";

		private const string ProbeMailInfoKey = "probeMailInfo";

		private const string SendMailExecutionIdKey = "sendMailExecutionId";

		private const string ExecutionContextKey = "executionContext";

		private const string RecordTypeKey = "recordType";

		private const string ProbeErrorTypeKey = "probeErrorType";

		private const string EhloIssuedKey = "ehloIssued";

		private const string ExchangeMessageIdKey = "exchangeMessageId";

		private const string FdServerEncounteredKey = "fdServerEncountered";

		private const string SmtpResponseReceivedKey = "smtpResponseReceived";

		private const string TargetVipKey = "targetVip";

		private const string HubServerKey = "hubServer";

		private const string DeliveryExpectedKey = "deliveryExpected";

		private const string SentTimeKey = "sentTime";

		private const string ResultTypeKey = "resultType";
	}
}
