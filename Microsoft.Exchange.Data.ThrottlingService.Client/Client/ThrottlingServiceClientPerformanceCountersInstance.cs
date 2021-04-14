using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.ThrottlingService.Client
{
	internal sealed class ThrottlingServiceClientPerformanceCountersInstance : PerformanceCounterInstance
	{
		internal ThrottlingServiceClientPerformanceCountersInstance(string instanceName, ThrottlingServiceClientPerformanceCountersInstance autoUpdateTotalInstance) : base(instanceName, "MSExchange Throttling Service Client")
		{
			bool flag = false;
			List<ExPerformanceCounter> list = new List<ExPerformanceCounter>();
			try
			{
				this.SuccessfulSubmissionRequests = new ExPerformanceCounter(base.CategoryName, "Percentage of Successful Submission Requests.", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.SuccessfulSubmissionRequests);
				this.DeniedSubmissionRequest = new ExPerformanceCounter(base.CategoryName, "Percentage of Denied Submission Request.", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.DeniedSubmissionRequest);
				this.BypassedSubmissionRequests = new ExPerformanceCounter(base.CategoryName, "Percentage of Bypassed Submission Requests.", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.BypassedSubmissionRequests);
				this.AverageSubmissionRequestTime = new ExPerformanceCounter(base.CategoryName, "Average request processing time.", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.AverageSubmissionRequestTime);
				long num = this.SuccessfulSubmissionRequests.RawValue;
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

		internal ThrottlingServiceClientPerformanceCountersInstance(string instanceName) : base(instanceName, "MSExchange Throttling Service Client")
		{
			bool flag = false;
			List<ExPerformanceCounter> list = new List<ExPerformanceCounter>();
			try
			{
				this.SuccessfulSubmissionRequests = new ExPerformanceCounter(base.CategoryName, "Percentage of Successful Submission Requests.", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.SuccessfulSubmissionRequests);
				this.DeniedSubmissionRequest = new ExPerformanceCounter(base.CategoryName, "Percentage of Denied Submission Request.", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.DeniedSubmissionRequest);
				this.BypassedSubmissionRequests = new ExPerformanceCounter(base.CategoryName, "Percentage of Bypassed Submission Requests.", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.BypassedSubmissionRequests);
				this.AverageSubmissionRequestTime = new ExPerformanceCounter(base.CategoryName, "Average request processing time.", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.AverageSubmissionRequestTime);
				long num = this.SuccessfulSubmissionRequests.RawValue;
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

		public readonly ExPerformanceCounter SuccessfulSubmissionRequests;

		public readonly ExPerformanceCounter DeniedSubmissionRequest;

		public readonly ExPerformanceCounter BypassedSubmissionRequests;

		public readonly ExPerformanceCounter AverageSubmissionRequestTime;
	}
}
