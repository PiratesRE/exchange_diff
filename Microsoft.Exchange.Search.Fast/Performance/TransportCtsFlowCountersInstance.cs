using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Search.Performance
{
	internal sealed class TransportCtsFlowCountersInstance : PerformanceCounterInstance
	{
		internal TransportCtsFlowCountersInstance(string instanceName, TransportCtsFlowCountersInstance autoUpdateTotalInstance) : base(instanceName, "MSExchangeSearch Transport CTS Flow")
		{
			bool flag = false;
			List<ExPerformanceCounter> list = new List<ExPerformanceCounter>();
			try
			{
				ExPerformanceCounter exPerformanceCounter = new ExPerformanceCounter(base.CategoryName, "Total Bytes Sent Per Second", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter);
				this.BytesSent = new ExPerformanceCounter(base.CategoryName, "Bytes Sent", instanceName, null, new ExPerformanceCounter[]
				{
					exPerformanceCounter
				});
				list.Add(this.BytesSent);
				ExPerformanceCounter exPerformanceCounter2 = new ExPerformanceCounter(base.CategoryName, "Total Bytes Received Per Second", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter2);
				this.BytesReceived = new ExPerformanceCounter(base.CategoryName, "Bytes Received", instanceName, null, new ExPerformanceCounter[]
				{
					exPerformanceCounter2
				});
				list.Add(this.BytesReceived);
				ExPerformanceCounter exPerformanceCounter3 = new ExPerformanceCounter(base.CategoryName, "Document Processing Rate Per Second", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter3);
				ExPerformanceCounter exPerformanceCounter4 = new ExPerformanceCounter(base.CategoryName, "Document Failure Rate Per Second", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter4);
				ExPerformanceCounter exPerformanceCounter5 = new ExPerformanceCounter(base.CategoryName, "Document Skip Rate Per Second", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter5);
				this.LanguageDetectionFailures = new ExPerformanceCounter(base.CategoryName, "Language Detection Failures", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.LanguageDetectionFailures);
				this.NumberOfFailedDocuments = new ExPerformanceCounter(base.CategoryName, "Number Of Failed Documents", instanceName, null, new ExPerformanceCounter[]
				{
					exPerformanceCounter4
				});
				list.Add(this.NumberOfFailedDocuments);
				this.NumberOfProcessedDocuments = new ExPerformanceCounter(base.CategoryName, "Number Of Processed Documents", instanceName, null, new ExPerformanceCounter[]
				{
					exPerformanceCounter3
				});
				list.Add(this.NumberOfProcessedDocuments);
				this.NumberOfSkippedNlg = new ExPerformanceCounter(base.CategoryName, "Nlg Documents Skipped", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.NumberOfSkippedNlg);
				this.TimeInGetConnectionInMsec = new ExPerformanceCounter(base.CategoryName, "Time Spent Msec: GetConnection", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.TimeInGetConnectionInMsec);
				this.TimeInPropertyBagLoadInMsec = new ExPerformanceCounter(base.CategoryName, "Time Spent Msec: PropertyBagLoad", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.TimeInPropertyBagLoadInMsec);
				this.TimeInMessageItemConversionInMsec = new ExPerformanceCounter(base.CategoryName, "Time Spent Msec: MessageItem Conversion", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.TimeInMessageItemConversionInMsec);
				this.TimeDeterminingAgeOfItemInMsec = new ExPerformanceCounter(base.CategoryName, "Time Spent Msec: Determining age of Item", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.TimeDeterminingAgeOfItemInMsec);
				this.TimeInMimeConversionInMsec = new ExPerformanceCounter(base.CategoryName, "Time Spent Msec: Mime Conversion", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.TimeInMimeConversionInMsec);
				this.TimeInShouldAnnotateMessageInMsec = new ExPerformanceCounter(base.CategoryName, "Time Spent Msec: ShouldAnnotateMessage", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.TimeInShouldAnnotateMessageInMsec);
				this.TimeInTransportRetrieverInMsec = new ExPerformanceCounter(base.CategoryName, "Time Spent Msec: TransportRetriever", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.TimeInTransportRetrieverInMsec);
				this.TimeInDocParserInMsec = new ExPerformanceCounter(base.CategoryName, "Time Spent Msec: DocParser", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.TimeInDocParserInMsec);
				this.TimeInNLGSubflowInMsec = new ExPerformanceCounter(base.CategoryName, "Time Spent Msec: NlgSubflow", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.TimeInNLGSubflowInMsec);
				this.TimeInQueueInMsec = new ExPerformanceCounter(base.CategoryName, "Time Spent Msec: Waiting In Queue", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.TimeInQueueInMsec);
				this.TimeInWordbreakerInMsec = new ExPerformanceCounter(base.CategoryName, "Time Spent Msec: WordBreaker", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.TimeInWordbreakerInMsec);
				this.TimeSpentWaitingForConnectInMsec = new ExPerformanceCounter(base.CategoryName, "Time Spent Msec: Waiting For Connect", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.TimeSpentWaitingForConnectInMsec);
				this.TotalDocuments = new ExPerformanceCounter(base.CategoryName, "Number Of Documents", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.TotalDocuments);
				this.TotalSkippedDocuments = new ExPerformanceCounter(base.CategoryName, "Number Of Skipped Documents", instanceName, null, new ExPerformanceCounter[]
				{
					exPerformanceCounter5
				});
				list.Add(this.TotalSkippedDocuments);
				this.TotalTimeProcessingMessageInMsec = new ExPerformanceCounter(base.CategoryName, "Time Spent Msec: Processing Messages", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.TotalTimeProcessingMessageInMsec);
				this.TotalTimeProcessingFailedMessageInMsec = new ExPerformanceCounter(base.CategoryName, "Time Spent Msec: Processing Messages That Failed", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.TotalTimeProcessingFailedMessageInMsec);
				this.ProcessedUnder250ms = new ExPerformanceCounter(base.CategoryName, "Documents Processed Under 250ms", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.ProcessedUnder250ms);
				this.ProcessedUnder500ms = new ExPerformanceCounter(base.CategoryName, "Documents Processed Under 500ms", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.ProcessedUnder500ms);
				this.ProcessedUnder1000ms = new ExPerformanceCounter(base.CategoryName, "Documents Processed Under 1000ms", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.ProcessedUnder1000ms);
				this.ProcessedUnder2000ms = new ExPerformanceCounter(base.CategoryName, "Documents Processed Under 2000ms", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.ProcessedUnder2000ms);
				this.ProcessedUnder5000ms = new ExPerformanceCounter(base.CategoryName, "Documents Processed Under 5000ms", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.ProcessedUnder5000ms);
				this.ProcessedOver5000ms = new ExPerformanceCounter(base.CategoryName, "Documents processed over 5000ms", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.ProcessedOver5000ms);
				long num = this.BytesSent.RawValue;
				num += 1L;
				flag = true;
			}
			finally
			{
				if (!flag)
				{
					foreach (ExPerformanceCounter exPerformanceCounter6 in list)
					{
						exPerformanceCounter6.Close();
					}
				}
			}
			this.counters = list.ToArray();
		}

		internal TransportCtsFlowCountersInstance(string instanceName) : base(instanceName, "MSExchangeSearch Transport CTS Flow")
		{
			bool flag = false;
			List<ExPerformanceCounter> list = new List<ExPerformanceCounter>();
			try
			{
				ExPerformanceCounter exPerformanceCounter = new ExPerformanceCounter(base.CategoryName, "Total Bytes Sent Per Second", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter);
				this.BytesSent = new ExPerformanceCounter(base.CategoryName, "Bytes Sent", instanceName, null, new ExPerformanceCounter[]
				{
					exPerformanceCounter
				});
				list.Add(this.BytesSent);
				ExPerformanceCounter exPerformanceCounter2 = new ExPerformanceCounter(base.CategoryName, "Total Bytes Received Per Second", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter2);
				this.BytesReceived = new ExPerformanceCounter(base.CategoryName, "Bytes Received", instanceName, null, new ExPerformanceCounter[]
				{
					exPerformanceCounter2
				});
				list.Add(this.BytesReceived);
				ExPerformanceCounter exPerformanceCounter3 = new ExPerformanceCounter(base.CategoryName, "Document Processing Rate Per Second", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter3);
				ExPerformanceCounter exPerformanceCounter4 = new ExPerformanceCounter(base.CategoryName, "Document Failure Rate Per Second", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter4);
				ExPerformanceCounter exPerformanceCounter5 = new ExPerformanceCounter(base.CategoryName, "Document Skip Rate Per Second", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter5);
				this.LanguageDetectionFailures = new ExPerformanceCounter(base.CategoryName, "Language Detection Failures", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.LanguageDetectionFailures);
				this.NumberOfFailedDocuments = new ExPerformanceCounter(base.CategoryName, "Number Of Failed Documents", instanceName, null, new ExPerformanceCounter[]
				{
					exPerformanceCounter4
				});
				list.Add(this.NumberOfFailedDocuments);
				this.NumberOfProcessedDocuments = new ExPerformanceCounter(base.CategoryName, "Number Of Processed Documents", instanceName, null, new ExPerformanceCounter[]
				{
					exPerformanceCounter3
				});
				list.Add(this.NumberOfProcessedDocuments);
				this.NumberOfSkippedNlg = new ExPerformanceCounter(base.CategoryName, "Nlg Documents Skipped", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.NumberOfSkippedNlg);
				this.TimeInGetConnectionInMsec = new ExPerformanceCounter(base.CategoryName, "Time Spent Msec: GetConnection", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.TimeInGetConnectionInMsec);
				this.TimeInPropertyBagLoadInMsec = new ExPerformanceCounter(base.CategoryName, "Time Spent Msec: PropertyBagLoad", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.TimeInPropertyBagLoadInMsec);
				this.TimeInMessageItemConversionInMsec = new ExPerformanceCounter(base.CategoryName, "Time Spent Msec: MessageItem Conversion", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.TimeInMessageItemConversionInMsec);
				this.TimeDeterminingAgeOfItemInMsec = new ExPerformanceCounter(base.CategoryName, "Time Spent Msec: Determining age of Item", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.TimeDeterminingAgeOfItemInMsec);
				this.TimeInMimeConversionInMsec = new ExPerformanceCounter(base.CategoryName, "Time Spent Msec: Mime Conversion", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.TimeInMimeConversionInMsec);
				this.TimeInShouldAnnotateMessageInMsec = new ExPerformanceCounter(base.CategoryName, "Time Spent Msec: ShouldAnnotateMessage", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.TimeInShouldAnnotateMessageInMsec);
				this.TimeInTransportRetrieverInMsec = new ExPerformanceCounter(base.CategoryName, "Time Spent Msec: TransportRetriever", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.TimeInTransportRetrieverInMsec);
				this.TimeInDocParserInMsec = new ExPerformanceCounter(base.CategoryName, "Time Spent Msec: DocParser", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.TimeInDocParserInMsec);
				this.TimeInNLGSubflowInMsec = new ExPerformanceCounter(base.CategoryName, "Time Spent Msec: NlgSubflow", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.TimeInNLGSubflowInMsec);
				this.TimeInQueueInMsec = new ExPerformanceCounter(base.CategoryName, "Time Spent Msec: Waiting In Queue", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.TimeInQueueInMsec);
				this.TimeInWordbreakerInMsec = new ExPerformanceCounter(base.CategoryName, "Time Spent Msec: WordBreaker", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.TimeInWordbreakerInMsec);
				this.TimeSpentWaitingForConnectInMsec = new ExPerformanceCounter(base.CategoryName, "Time Spent Msec: Waiting For Connect", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.TimeSpentWaitingForConnectInMsec);
				this.TotalDocuments = new ExPerformanceCounter(base.CategoryName, "Number Of Documents", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.TotalDocuments);
				this.TotalSkippedDocuments = new ExPerformanceCounter(base.CategoryName, "Number Of Skipped Documents", instanceName, null, new ExPerformanceCounter[]
				{
					exPerformanceCounter5
				});
				list.Add(this.TotalSkippedDocuments);
				this.TotalTimeProcessingMessageInMsec = new ExPerformanceCounter(base.CategoryName, "Time Spent Msec: Processing Messages", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.TotalTimeProcessingMessageInMsec);
				this.TotalTimeProcessingFailedMessageInMsec = new ExPerformanceCounter(base.CategoryName, "Time Spent Msec: Processing Messages That Failed", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.TotalTimeProcessingFailedMessageInMsec);
				this.ProcessedUnder250ms = new ExPerformanceCounter(base.CategoryName, "Documents Processed Under 250ms", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.ProcessedUnder250ms);
				this.ProcessedUnder500ms = new ExPerformanceCounter(base.CategoryName, "Documents Processed Under 500ms", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.ProcessedUnder500ms);
				this.ProcessedUnder1000ms = new ExPerformanceCounter(base.CategoryName, "Documents Processed Under 1000ms", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.ProcessedUnder1000ms);
				this.ProcessedUnder2000ms = new ExPerformanceCounter(base.CategoryName, "Documents Processed Under 2000ms", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.ProcessedUnder2000ms);
				this.ProcessedUnder5000ms = new ExPerformanceCounter(base.CategoryName, "Documents Processed Under 5000ms", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.ProcessedUnder5000ms);
				this.ProcessedOver5000ms = new ExPerformanceCounter(base.CategoryName, "Documents processed over 5000ms", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.ProcessedOver5000ms);
				long num = this.BytesSent.RawValue;
				num += 1L;
				flag = true;
			}
			finally
			{
				if (!flag)
				{
					foreach (ExPerformanceCounter exPerformanceCounter6 in list)
					{
						exPerformanceCounter6.Close();
					}
				}
			}
			this.counters = list.ToArray();
		}

		public override void GetPerfCounterDiagnosticsInfo(XElement topElement)
		{
			XElement xelement = null;
			foreach (ExPerformanceCounter exPerformanceCounter in this.counters)
			{
				try
				{
					if (xelement == null)
					{
						xelement = new XElement(ExPerformanceCounter.GetEncodedName(exPerformanceCounter.InstanceName));
						topElement.Add(xelement);
					}
					xelement.Add(new XElement(ExPerformanceCounter.GetEncodedName(exPerformanceCounter.CounterName), exPerformanceCounter.NextValue()));
				}
				catch (XmlException ex)
				{
					XElement content = new XElement("Error", ex.Message);
					topElement.Add(content);
				}
			}
		}

		public readonly ExPerformanceCounter BytesSent;

		public readonly ExPerformanceCounter BytesReceived;

		public readonly ExPerformanceCounter LanguageDetectionFailures;

		public readonly ExPerformanceCounter NumberOfFailedDocuments;

		public readonly ExPerformanceCounter NumberOfProcessedDocuments;

		public readonly ExPerformanceCounter NumberOfSkippedNlg;

		public readonly ExPerformanceCounter TimeInGetConnectionInMsec;

		public readonly ExPerformanceCounter TimeInPropertyBagLoadInMsec;

		public readonly ExPerformanceCounter TimeInMessageItemConversionInMsec;

		public readonly ExPerformanceCounter TimeDeterminingAgeOfItemInMsec;

		public readonly ExPerformanceCounter TimeInMimeConversionInMsec;

		public readonly ExPerformanceCounter TimeInShouldAnnotateMessageInMsec;

		public readonly ExPerformanceCounter TimeInTransportRetrieverInMsec;

		public readonly ExPerformanceCounter TimeInDocParserInMsec;

		public readonly ExPerformanceCounter TimeInNLGSubflowInMsec;

		public readonly ExPerformanceCounter TimeInQueueInMsec;

		public readonly ExPerformanceCounter TimeInWordbreakerInMsec;

		public readonly ExPerformanceCounter TimeSpentWaitingForConnectInMsec;

		public readonly ExPerformanceCounter TotalDocuments;

		public readonly ExPerformanceCounter TotalSkippedDocuments;

		public readonly ExPerformanceCounter TotalTimeProcessingMessageInMsec;

		public readonly ExPerformanceCounter TotalTimeProcessingFailedMessageInMsec;

		public readonly ExPerformanceCounter ProcessedUnder250ms;

		public readonly ExPerformanceCounter ProcessedUnder500ms;

		public readonly ExPerformanceCounter ProcessedUnder1000ms;

		public readonly ExPerformanceCounter ProcessedUnder2000ms;

		public readonly ExPerformanceCounter ProcessedUnder5000ms;

		public readonly ExPerformanceCounter ProcessedOver5000ms;
	}
}
