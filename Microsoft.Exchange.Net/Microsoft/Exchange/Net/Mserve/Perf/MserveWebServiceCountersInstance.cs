using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Net.Mserve.Perf
{
	internal sealed class MserveWebServiceCountersInstance : PerformanceCounterInstance
	{
		internal MserveWebServiceCountersInstance(string instanceName, MserveWebServiceCountersInstance autoUpdateTotalInstance) : base(instanceName, "MSExchange MserveWebService")
		{
			bool flag = false;
			List<ExPerformanceCounter> list = new List<ExPerformanceCounter>();
			try
			{
				this.ReadRequestsInMserveCacheService = new ExPerformanceCounter(base.CategoryName, "Read requests by Mserve Cache", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.ReadRequestsInMserveCacheService);
				this.TotalRequestsInMserveCacheService = new ExPerformanceCounter(base.CategoryName, "Total requests by Mserve Cache Service", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.TotalRequestsInMserveCacheService);
				this.TotalFailuresInMserveCacheService = new ExPerformanceCounter(base.CategoryName, "Total failures by Mserve Cache Service", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.TotalFailuresInMserveCacheService);
				this.PercentageFailuresInMserveCacheService = new ExPerformanceCounter(base.CategoryName, "Percentage of failures by Mserve Cache Service", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.PercentageFailuresInMserveCacheService);
				this.PercentageRequestsInMserveCacheService = new ExPerformanceCounter(base.CategoryName, "Percentage of requests by Mserve Cache Service", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.PercentageRequestsInMserveCacheService);
				this.ReadRequestsInMserveWebService = new ExPerformanceCounter(base.CategoryName, "Read requests by Real Mserve Web Service", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.ReadRequestsInMserveWebService);
				this.AddRequestsInMserveWebService = new ExPerformanceCounter(base.CategoryName, "Add requests by Real Mserve Web Service", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.AddRequestsInMserveWebService);
				this.DeleteRequestsInMserveWebService = new ExPerformanceCounter(base.CategoryName, "Delete requests by Real Mserve Web Service", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.DeleteRequestsInMserveWebService);
				this.TotalRequestsInMserveWebService = new ExPerformanceCounter(base.CategoryName, "Total requests by Real Mserve Web Service", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.TotalRequestsInMserveWebService);
				this.TotalFailuresInMserveWebService = new ExPerformanceCounter(base.CategoryName, "Total failures by Real Mserve Web Service", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.TotalFailuresInMserveWebService);
				this.PercentageFailuresInMserveWebService = new ExPerformanceCounter(base.CategoryName, "Percentage of failures by Real Mserve Web Service", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.PercentageFailuresInMserveWebService);
				this.PercentageRequestsInMserveWebService = new ExPerformanceCounter(base.CategoryName, "Percentage of requests by Real Mserve Web Service", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.PercentageRequestsInMserveWebService);
				this.TotalRequestsInMserveService = new ExPerformanceCounter(base.CategoryName, "Total requests by Mserve", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.TotalRequestsInMserveService);
				this.TotalFailuresInMserveService = new ExPerformanceCounter(base.CategoryName, "Total failures by Mserve", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.TotalFailuresInMserveService);
				this.PercentageTotalFailuresInMserveService = new ExPerformanceCounter(base.CategoryName, "Percentage of failures in Mserve", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.PercentageTotalFailuresInMserveService);
				long num = this.ReadRequestsInMserveCacheService.RawValue;
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

		internal MserveWebServiceCountersInstance(string instanceName) : base(instanceName, "MSExchange MserveWebService")
		{
			bool flag = false;
			List<ExPerformanceCounter> list = new List<ExPerformanceCounter>();
			try
			{
				this.ReadRequestsInMserveCacheService = new ExPerformanceCounter(base.CategoryName, "Read requests by Mserve Cache", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.ReadRequestsInMserveCacheService);
				this.TotalRequestsInMserveCacheService = new ExPerformanceCounter(base.CategoryName, "Total requests by Mserve Cache Service", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.TotalRequestsInMserveCacheService);
				this.TotalFailuresInMserveCacheService = new ExPerformanceCounter(base.CategoryName, "Total failures by Mserve Cache Service", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.TotalFailuresInMserveCacheService);
				this.PercentageFailuresInMserveCacheService = new ExPerformanceCounter(base.CategoryName, "Percentage of failures by Mserve Cache Service", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.PercentageFailuresInMserveCacheService);
				this.PercentageRequestsInMserveCacheService = new ExPerformanceCounter(base.CategoryName, "Percentage of requests by Mserve Cache Service", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.PercentageRequestsInMserveCacheService);
				this.ReadRequestsInMserveWebService = new ExPerformanceCounter(base.CategoryName, "Read requests by Real Mserve Web Service", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.ReadRequestsInMserveWebService);
				this.AddRequestsInMserveWebService = new ExPerformanceCounter(base.CategoryName, "Add requests by Real Mserve Web Service", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.AddRequestsInMserveWebService);
				this.DeleteRequestsInMserveWebService = new ExPerformanceCounter(base.CategoryName, "Delete requests by Real Mserve Web Service", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.DeleteRequestsInMserveWebService);
				this.TotalRequestsInMserveWebService = new ExPerformanceCounter(base.CategoryName, "Total requests by Real Mserve Web Service", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.TotalRequestsInMserveWebService);
				this.TotalFailuresInMserveWebService = new ExPerformanceCounter(base.CategoryName, "Total failures by Real Mserve Web Service", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.TotalFailuresInMserveWebService);
				this.PercentageFailuresInMserveWebService = new ExPerformanceCounter(base.CategoryName, "Percentage of failures by Real Mserve Web Service", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.PercentageFailuresInMserveWebService);
				this.PercentageRequestsInMserveWebService = new ExPerformanceCounter(base.CategoryName, "Percentage of requests by Real Mserve Web Service", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.PercentageRequestsInMserveWebService);
				this.TotalRequestsInMserveService = new ExPerformanceCounter(base.CategoryName, "Total requests by Mserve", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.TotalRequestsInMserveService);
				this.TotalFailuresInMserveService = new ExPerformanceCounter(base.CategoryName, "Total failures by Mserve", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.TotalFailuresInMserveService);
				this.PercentageTotalFailuresInMserveService = new ExPerformanceCounter(base.CategoryName, "Percentage of failures in Mserve", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.PercentageTotalFailuresInMserveService);
				long num = this.ReadRequestsInMserveCacheService.RawValue;
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

		public readonly ExPerformanceCounter ReadRequestsInMserveCacheService;

		public readonly ExPerformanceCounter TotalRequestsInMserveCacheService;

		public readonly ExPerformanceCounter TotalFailuresInMserveCacheService;

		public readonly ExPerformanceCounter PercentageFailuresInMserveCacheService;

		public readonly ExPerformanceCounter PercentageRequestsInMserveCacheService;

		public readonly ExPerformanceCounter ReadRequestsInMserveWebService;

		public readonly ExPerformanceCounter AddRequestsInMserveWebService;

		public readonly ExPerformanceCounter DeleteRequestsInMserveWebService;

		public readonly ExPerformanceCounter TotalRequestsInMserveWebService;

		public readonly ExPerformanceCounter TotalFailuresInMserveWebService;

		public readonly ExPerformanceCounter PercentageFailuresInMserveWebService;

		public readonly ExPerformanceCounter PercentageRequestsInMserveWebService;

		public readonly ExPerformanceCounter TotalRequestsInMserveService;

		public readonly ExPerformanceCounter TotalFailuresInMserveService;

		public readonly ExPerformanceCounter PercentageTotalFailuresInMserveService;
	}
}
