using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.MailboxTransport.Submission.StoreDriverSubmission
{
	internal sealed class MSExchangeStoreDriverSubmissionAgentInstance : PerformanceCounterInstance
	{
		internal MSExchangeStoreDriverSubmissionAgentInstance(string instanceName, MSExchangeStoreDriverSubmissionAgentInstance autoUpdateTotalInstance) : base(instanceName, "MSExchange Submission Store Driver Agents")
		{
			bool flag = false;
			List<ExPerformanceCounter> list = new List<ExPerformanceCounter>();
			try
			{
				this.SubmissionAgentFailures = new ExPerformanceCounter(base.CategoryName, "StoreDriverSubmission Agent Failure", instanceName, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.SubmissionAgentFailures, new ExPerformanceCounter[0]);
				list.Add(this.SubmissionAgentFailures);
				long num = this.SubmissionAgentFailures.RawValue;
				num += 1L;
				flag = true;
			}
			finally
			{
				if (!flag)
				{
					foreach (ExPerformanceCounter exPerformanceCounter in list)
					{
						exPerformanceCounter.Close();
					}
				}
			}
			this.counters = list.ToArray();
		}

		internal MSExchangeStoreDriverSubmissionAgentInstance(string instanceName) : base(instanceName, "MSExchange Submission Store Driver Agents")
		{
			bool flag = false;
			List<ExPerformanceCounter> list = new List<ExPerformanceCounter>();
			try
			{
				this.SubmissionAgentFailures = new ExPerformanceCounter(base.CategoryName, "StoreDriverSubmission Agent Failure", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.SubmissionAgentFailures);
				long num = this.SubmissionAgentFailures.RawValue;
				num += 1L;
				flag = true;
			}
			finally
			{
				if (!flag)
				{
					foreach (ExPerformanceCounter exPerformanceCounter in list)
					{
						exPerformanceCounter.Close();
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

		public readonly ExPerformanceCounter SubmissionAgentFailures;
	}
}
