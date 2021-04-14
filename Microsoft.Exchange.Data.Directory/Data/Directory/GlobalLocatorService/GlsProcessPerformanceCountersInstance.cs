using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Directory.GlobalLocatorService
{
	internal sealed class GlsProcessPerformanceCountersInstance : PerformanceCounterInstance
	{
		internal GlsProcessPerformanceCountersInstance(string instanceName, GlsProcessPerformanceCountersInstance autoUpdateTotalInstance) : base(instanceName, "MSExchange Global Locator Processes")
		{
			bool flag = false;
			List<ExPerformanceCounter> list = new List<ExPerformanceCounter>();
			try
			{
				this.NinetyFifthPercentileLatency = new ExPerformanceCounter(base.CategoryName, "95th Percentile Overall Latency", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.NinetyFifthPercentileLatency, new ExPerformanceCounter[0]);
				list.Add(this.NinetyFifthPercentileLatency);
				this.NinetyFifthPercentileLatencyBase = new ExPerformanceCounter(base.CategoryName, "Base for 95th Percentile Overall Latency", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.NinetyFifthPercentileLatencyBase, new ExPerformanceCounter[0]);
				list.Add(this.NinetyFifthPercentileLatencyBase);
				this.NinetyNinthPercentileLatency = new ExPerformanceCounter(base.CategoryName, "99th Percentile Overall Latency", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.NinetyNinthPercentileLatency, new ExPerformanceCounter[0]);
				list.Add(this.NinetyNinthPercentileLatency);
				this.NinetyNinthPercentileLatencyBase = new ExPerformanceCounter(base.CategoryName, "Base for 99th Percentile Overall Latency", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.NinetyNinthPercentileLatencyBase, new ExPerformanceCounter[0]);
				list.Add(this.NinetyNinthPercentileLatencyBase);
				this.AverageOverallLatency = new ExPerformanceCounter(base.CategoryName, "Average Overall Latency", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.AverageOverallLatency, new ExPerformanceCounter[0]);
				list.Add(this.AverageOverallLatency);
				this.AverageOverallLatencyBase = new ExPerformanceCounter(base.CategoryName, "Base for Average Overall Latency", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.AverageOverallLatencyBase, new ExPerformanceCounter[0]);
				list.Add(this.AverageOverallLatencyBase);
				this.AverageReadLatency = new ExPerformanceCounter(base.CategoryName, "Average Read Latency", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.AverageReadLatency, new ExPerformanceCounter[0]);
				list.Add(this.AverageReadLatency);
				this.AverageReadLatencyBase = new ExPerformanceCounter(base.CategoryName, "Base for Average Read Latency", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.AverageReadLatencyBase, new ExPerformanceCounter[0]);
				list.Add(this.AverageReadLatencyBase);
				this.AverageWriteLatency = new ExPerformanceCounter(base.CategoryName, "Average Write Latency", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.AverageWriteLatency, new ExPerformanceCounter[0]);
				list.Add(this.AverageWriteLatency);
				this.AverageWriteLatencyBase = new ExPerformanceCounter(base.CategoryName, "Base for Average Write Latency", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.AverageWriteLatencyBase, new ExPerformanceCounter[0]);
				list.Add(this.AverageWriteLatencyBase);
				this.SuccessfulCallsPerMinute = new ExPerformanceCounter(base.CategoryName, "Successful calls over last minute", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.SuccessfulCallsPerMinute, new ExPerformanceCounter[0]);
				list.Add(this.SuccessfulCallsPerMinute);
				this.FailedCallsPerMinute = new ExPerformanceCounter(base.CategoryName, "Failed calls over last minute", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.FailedCallsPerMinute, new ExPerformanceCounter[0]);
				list.Add(this.FailedCallsPerMinute);
				this.NotFoundCallsPerMinute = new ExPerformanceCounter(base.CategoryName, "Not Found calls over last minute", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.NotFoundCallsPerMinute, new ExPerformanceCounter[0]);
				list.Add(this.NotFoundCallsPerMinute);
				this.CacheHitsRatioPerMinute = new ExPerformanceCounter(base.CategoryName, "Percentage of GLS cache hits for last minute", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.CacheHitsRatioPerMinute, new ExPerformanceCounter[0]);
				list.Add(this.CacheHitsRatioPerMinute);
				this.AcceptedDomainLookupCacheHitsRatioPerMinute = new ExPerformanceCounter(base.CategoryName, "Percentage of domain Cache hits for last minute", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.AcceptedDomainLookupCacheHitsRatioPerMinute, new ExPerformanceCounter[0]);
				list.Add(this.AcceptedDomainLookupCacheHitsRatioPerMinute);
				this.ExternalDirectoryOrganizationIdCacheHitsRatioPerMinute = new ExPerformanceCounter(base.CategoryName, "Percentage of ExternalDirectoryOrganizationId Cache hits for last minute", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.ExternalDirectoryOrganizationIdCacheHitsRatioPerMinute, new ExPerformanceCounter[0]);
				list.Add(this.ExternalDirectoryOrganizationIdCacheHitsRatioPerMinute);
				this.MSAUserNetIdCacheHitsRatioPerMinute = new ExPerformanceCounter(base.CategoryName, "Percentage of MSAUserNetID Cache hits for last minute", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.MSAUserNetIdCacheHitsRatioPerMinute, new ExPerformanceCounter[0]);
				list.Add(this.MSAUserNetIdCacheHitsRatioPerMinute);
				long num = this.NinetyFifthPercentileLatency.RawValue;
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

		internal GlsProcessPerformanceCountersInstance(string instanceName) : base(instanceName, "MSExchange Global Locator Processes")
		{
			bool flag = false;
			List<ExPerformanceCounter> list = new List<ExPerformanceCounter>();
			try
			{
				this.NinetyFifthPercentileLatency = new ExPerformanceCounter(base.CategoryName, "95th Percentile Overall Latency", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.NinetyFifthPercentileLatency);
				this.NinetyFifthPercentileLatencyBase = new ExPerformanceCounter(base.CategoryName, "Base for 95th Percentile Overall Latency", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.NinetyFifthPercentileLatencyBase);
				this.NinetyNinthPercentileLatency = new ExPerformanceCounter(base.CategoryName, "99th Percentile Overall Latency", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.NinetyNinthPercentileLatency);
				this.NinetyNinthPercentileLatencyBase = new ExPerformanceCounter(base.CategoryName, "Base for 99th Percentile Overall Latency", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.NinetyNinthPercentileLatencyBase);
				this.AverageOverallLatency = new ExPerformanceCounter(base.CategoryName, "Average Overall Latency", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.AverageOverallLatency);
				this.AverageOverallLatencyBase = new ExPerformanceCounter(base.CategoryName, "Base for Average Overall Latency", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.AverageOverallLatencyBase);
				this.AverageReadLatency = new ExPerformanceCounter(base.CategoryName, "Average Read Latency", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.AverageReadLatency);
				this.AverageReadLatencyBase = new ExPerformanceCounter(base.CategoryName, "Base for Average Read Latency", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.AverageReadLatencyBase);
				this.AverageWriteLatency = new ExPerformanceCounter(base.CategoryName, "Average Write Latency", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.AverageWriteLatency);
				this.AverageWriteLatencyBase = new ExPerformanceCounter(base.CategoryName, "Base for Average Write Latency", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.AverageWriteLatencyBase);
				this.SuccessfulCallsPerMinute = new ExPerformanceCounter(base.CategoryName, "Successful calls over last minute", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.SuccessfulCallsPerMinute);
				this.FailedCallsPerMinute = new ExPerformanceCounter(base.CategoryName, "Failed calls over last minute", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.FailedCallsPerMinute);
				this.NotFoundCallsPerMinute = new ExPerformanceCounter(base.CategoryName, "Not Found calls over last minute", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.NotFoundCallsPerMinute);
				this.CacheHitsRatioPerMinute = new ExPerformanceCounter(base.CategoryName, "Percentage of GLS cache hits for last minute", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.CacheHitsRatioPerMinute);
				this.AcceptedDomainLookupCacheHitsRatioPerMinute = new ExPerformanceCounter(base.CategoryName, "Percentage of domain Cache hits for last minute", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.AcceptedDomainLookupCacheHitsRatioPerMinute);
				this.ExternalDirectoryOrganizationIdCacheHitsRatioPerMinute = new ExPerformanceCounter(base.CategoryName, "Percentage of ExternalDirectoryOrganizationId Cache hits for last minute", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.ExternalDirectoryOrganizationIdCacheHitsRatioPerMinute);
				this.MSAUserNetIdCacheHitsRatioPerMinute = new ExPerformanceCounter(base.CategoryName, "Percentage of MSAUserNetID Cache hits for last minute", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.MSAUserNetIdCacheHitsRatioPerMinute);
				long num = this.NinetyFifthPercentileLatency.RawValue;
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

		public readonly ExPerformanceCounter NinetyFifthPercentileLatency;

		public readonly ExPerformanceCounter NinetyFifthPercentileLatencyBase;

		public readonly ExPerformanceCounter NinetyNinthPercentileLatency;

		public readonly ExPerformanceCounter NinetyNinthPercentileLatencyBase;

		public readonly ExPerformanceCounter AverageOverallLatency;

		public readonly ExPerformanceCounter AverageOverallLatencyBase;

		public readonly ExPerformanceCounter AverageReadLatency;

		public readonly ExPerformanceCounter AverageReadLatencyBase;

		public readonly ExPerformanceCounter AverageWriteLatency;

		public readonly ExPerformanceCounter AverageWriteLatencyBase;

		public readonly ExPerformanceCounter SuccessfulCallsPerMinute;

		public readonly ExPerformanceCounter FailedCallsPerMinute;

		public readonly ExPerformanceCounter NotFoundCallsPerMinute;

		public readonly ExPerformanceCounter CacheHitsRatioPerMinute;

		public readonly ExPerformanceCounter AcceptedDomainLookupCacheHitsRatioPerMinute;

		public readonly ExPerformanceCounter ExternalDirectoryOrganizationIdCacheHitsRatioPerMinute;

		public readonly ExPerformanceCounter MSAUserNetIdCacheHitsRatioPerMinute;
	}
}
