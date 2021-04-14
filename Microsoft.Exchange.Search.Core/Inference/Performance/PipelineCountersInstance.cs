using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Inference.Performance
{
	internal sealed class PipelineCountersInstance : PerformanceCounterInstance
	{
		internal PipelineCountersInstance(string instanceName, PipelineCountersInstance autoUpdateTotalInstance) : base(instanceName, "MSExchangeInference Pipeline")
		{
			bool flag = false;
			List<ExPerformanceCounter> list = new List<ExPerformanceCounter>();
			try
			{
				this.NumberOfOutstandingDocuments = new ExPerformanceCounter(base.CategoryName, "Number of Outstanding Documents", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.NumberOfOutstandingDocuments);
				ExPerformanceCounter exPerformanceCounter = new ExPerformanceCounter(base.CategoryName, "Document Incoming Rate Per Second", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter);
				this.NumberOfIncomingDocuments = new ExPerformanceCounter(base.CategoryName, "Number Of Incoming Documents", instanceName, null, new ExPerformanceCounter[]
				{
					exPerformanceCounter
				});
				list.Add(this.NumberOfIncomingDocuments);
				ExPerformanceCounter exPerformanceCounter2 = new ExPerformanceCounter(base.CategoryName, "Document Reject Rate Per Second", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter2);
				this.NumberOfRejectedDocuments = new ExPerformanceCounter(base.CategoryName, "Number Of Rejected Documents", instanceName, null, new ExPerformanceCounter[]
				{
					exPerformanceCounter2
				});
				list.Add(this.NumberOfRejectedDocuments);
				ExPerformanceCounter exPerformanceCounter3 = new ExPerformanceCounter(base.CategoryName, "Document Processing Rate Per Second", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter3);
				this.NumberOfProcessedDocuments = new ExPerformanceCounter(base.CategoryName, "Number Of Processed Documents", instanceName, null, new ExPerformanceCounter[]
				{
					exPerformanceCounter3
				});
				list.Add(this.NumberOfProcessedDocuments);
				ExPerformanceCounter exPerformanceCounter4 = new ExPerformanceCounter(base.CategoryName, "Document Success Rate Per Second", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter4);
				this.NumberOfSucceededDocuments = new ExPerformanceCounter(base.CategoryName, "Number Of Succeeded Documents", instanceName, null, new ExPerformanceCounter[]
				{
					exPerformanceCounter4
				});
				list.Add(this.NumberOfSucceededDocuments);
				ExPerformanceCounter exPerformanceCounter5 = new ExPerformanceCounter(base.CategoryName, "Document Failure Rate Per Second", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter5);
				this.NumberOfFailedDocuments = new ExPerformanceCounter(base.CategoryName, "Number Of Failed Documents", instanceName, null, new ExPerformanceCounter[]
				{
					exPerformanceCounter5
				});
				list.Add(this.NumberOfFailedDocuments);
				this.AverageDocumentProcessingTime = new ExPerformanceCounter(base.CategoryName, "Average Document Processing Time In Seconds", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.AverageDocumentProcessingTime);
				this.AverageDocumentProcessingTimeBase = new ExPerformanceCounter(base.CategoryName, "Average Document Processing Time Base", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.AverageDocumentProcessingTimeBase);
				this.NumberOfComponentsPoisoned = new ExPerformanceCounter(base.CategoryName, "Number Of Components Poisoned", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.NumberOfComponentsPoisoned);
				long num = this.NumberOfOutstandingDocuments.RawValue;
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

		internal PipelineCountersInstance(string instanceName) : base(instanceName, "MSExchangeInference Pipeline")
		{
			bool flag = false;
			List<ExPerformanceCounter> list = new List<ExPerformanceCounter>();
			try
			{
				this.NumberOfOutstandingDocuments = new ExPerformanceCounter(base.CategoryName, "Number of Outstanding Documents", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.NumberOfOutstandingDocuments);
				ExPerformanceCounter exPerformanceCounter = new ExPerformanceCounter(base.CategoryName, "Document Incoming Rate Per Second", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter);
				this.NumberOfIncomingDocuments = new ExPerformanceCounter(base.CategoryName, "Number Of Incoming Documents", instanceName, null, new ExPerformanceCounter[]
				{
					exPerformanceCounter
				});
				list.Add(this.NumberOfIncomingDocuments);
				ExPerformanceCounter exPerformanceCounter2 = new ExPerformanceCounter(base.CategoryName, "Document Reject Rate Per Second", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter2);
				this.NumberOfRejectedDocuments = new ExPerformanceCounter(base.CategoryName, "Number Of Rejected Documents", instanceName, null, new ExPerformanceCounter[]
				{
					exPerformanceCounter2
				});
				list.Add(this.NumberOfRejectedDocuments);
				ExPerformanceCounter exPerformanceCounter3 = new ExPerformanceCounter(base.CategoryName, "Document Processing Rate Per Second", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter3);
				this.NumberOfProcessedDocuments = new ExPerformanceCounter(base.CategoryName, "Number Of Processed Documents", instanceName, null, new ExPerformanceCounter[]
				{
					exPerformanceCounter3
				});
				list.Add(this.NumberOfProcessedDocuments);
				ExPerformanceCounter exPerformanceCounter4 = new ExPerformanceCounter(base.CategoryName, "Document Success Rate Per Second", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter4);
				this.NumberOfSucceededDocuments = new ExPerformanceCounter(base.CategoryName, "Number Of Succeeded Documents", instanceName, null, new ExPerformanceCounter[]
				{
					exPerformanceCounter4
				});
				list.Add(this.NumberOfSucceededDocuments);
				ExPerformanceCounter exPerformanceCounter5 = new ExPerformanceCounter(base.CategoryName, "Document Failure Rate Per Second", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter5);
				this.NumberOfFailedDocuments = new ExPerformanceCounter(base.CategoryName, "Number Of Failed Documents", instanceName, null, new ExPerformanceCounter[]
				{
					exPerformanceCounter5
				});
				list.Add(this.NumberOfFailedDocuments);
				this.AverageDocumentProcessingTime = new ExPerformanceCounter(base.CategoryName, "Average Document Processing Time In Seconds", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.AverageDocumentProcessingTime);
				this.AverageDocumentProcessingTimeBase = new ExPerformanceCounter(base.CategoryName, "Average Document Processing Time Base", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.AverageDocumentProcessingTimeBase);
				this.NumberOfComponentsPoisoned = new ExPerformanceCounter(base.CategoryName, "Number Of Components Poisoned", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.NumberOfComponentsPoisoned);
				long num = this.NumberOfOutstandingDocuments.RawValue;
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

		public readonly ExPerformanceCounter NumberOfOutstandingDocuments;

		public readonly ExPerformanceCounter NumberOfIncomingDocuments;

		public readonly ExPerformanceCounter NumberOfRejectedDocuments;

		public readonly ExPerformanceCounter NumberOfProcessedDocuments;

		public readonly ExPerformanceCounter NumberOfSucceededDocuments;

		public readonly ExPerformanceCounter NumberOfFailedDocuments;

		public readonly ExPerformanceCounter AverageDocumentProcessingTime;

		public readonly ExPerformanceCounter AverageDocumentProcessingTimeBase;

		public readonly ExPerformanceCounter NumberOfComponentsPoisoned;
	}
}
