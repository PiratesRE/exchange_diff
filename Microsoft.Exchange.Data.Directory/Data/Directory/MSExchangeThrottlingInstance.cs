using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Directory
{
	internal sealed class MSExchangeThrottlingInstance : PerformanceCounterInstance
	{
		internal MSExchangeThrottlingInstance(string instanceName, MSExchangeThrottlingInstance autoUpdateTotalInstance) : base(instanceName, "MSExchange Throttling")
		{
			bool flag = false;
			List<ExPerformanceCounter> list = new List<ExPerformanceCounter>();
			try
			{
				this.AverageThreadSleepTime = new ExPerformanceCounter(base.CategoryName, "Average Thread Sleep Time", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.AverageThreadSleepTime);
				ExPerformanceCounter exPerformanceCounter = new ExPerformanceCounter(base.CategoryName, "Active PowerShell Runspaces/Sec", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter);
				this.ActivePowerShellRunspaces = new ExPerformanceCounter(base.CategoryName, "Active PowerShell Runspaces", instanceName, null, new ExPerformanceCounter[]
				{
					exPerformanceCounter
				});
				list.Add(this.ActivePowerShellRunspaces);
				ExPerformanceCounter exPerformanceCounter2 = new ExPerformanceCounter(base.CategoryName, "Exchange Executing Cmdlets/Sec", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter2);
				this.ExchangeExecutingCmdlets = new ExPerformanceCounter(base.CategoryName, "Exchange Executing Cmdlets", instanceName, null, new ExPerformanceCounter[]
				{
					exPerformanceCounter2
				});
				list.Add(this.ExchangeExecutingCmdlets);
				this.OrganizationThrottlingPolicyCacheHitCount = new ExPerformanceCounter(base.CategoryName, "Organization Throttling Policy Cache Hit Count", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.OrganizationThrottlingPolicyCacheHitCount);
				this.OrganizationThrottlingPolicyCacheMissCount = new ExPerformanceCounter(base.CategoryName, "Organization Throttling Policy Cache Miss Count", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.OrganizationThrottlingPolicyCacheMissCount);
				this.OrganizationThrottlingPolicyCacheLength = new ExPerformanceCounter(base.CategoryName, "Organization Throttling Policy Cache Length", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.OrganizationThrottlingPolicyCacheLength);
				this.OrganizationThrottlingPolicyCacheLengthPercentage = new ExPerformanceCounter(base.CategoryName, "Organization Throttling Policy Cache Length Percentage", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.OrganizationThrottlingPolicyCacheLengthPercentage);
				this.ThrottlingPolicyCacheHitCount = new ExPerformanceCounter(base.CategoryName, "Throttling Policy Cache Hit Count", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.ThrottlingPolicyCacheHitCount);
				this.ThrottlingPolicyCacheMissCount = new ExPerformanceCounter(base.CategoryName, "Throttling Policy Cache Miss Count", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.ThrottlingPolicyCacheMissCount);
				this.ThrottlingPolicyCacheLength = new ExPerformanceCounter(base.CategoryName, "Throttling Policy Cache Length", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.ThrottlingPolicyCacheLength);
				this.ThrottlingPolicyCacheLengthPercentage = new ExPerformanceCounter(base.CategoryName, "Throttling Policy Cache Length Percentage", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.ThrottlingPolicyCacheLengthPercentage);
				long num = this.AverageThreadSleepTime.RawValue;
				num += 1L;
				flag = true;
			}
			finally
			{
				if (!flag)
				{
					foreach (ExPerformanceCounter exPerformanceCounter3 in list)
					{
						exPerformanceCounter3.Close();
					}
				}
			}
			this.counters = list.ToArray();
		}

		internal MSExchangeThrottlingInstance(string instanceName) : base(instanceName, "MSExchange Throttling")
		{
			bool flag = false;
			List<ExPerformanceCounter> list = new List<ExPerformanceCounter>();
			try
			{
				this.AverageThreadSleepTime = new ExPerformanceCounter(base.CategoryName, "Average Thread Sleep Time", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.AverageThreadSleepTime);
				ExPerformanceCounter exPerformanceCounter = new ExPerformanceCounter(base.CategoryName, "Active PowerShell Runspaces/Sec", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter);
				this.ActivePowerShellRunspaces = new ExPerformanceCounter(base.CategoryName, "Active PowerShell Runspaces", instanceName, null, new ExPerformanceCounter[]
				{
					exPerformanceCounter
				});
				list.Add(this.ActivePowerShellRunspaces);
				ExPerformanceCounter exPerformanceCounter2 = new ExPerformanceCounter(base.CategoryName, "Exchange Executing Cmdlets/Sec", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter2);
				this.ExchangeExecutingCmdlets = new ExPerformanceCounter(base.CategoryName, "Exchange Executing Cmdlets", instanceName, null, new ExPerformanceCounter[]
				{
					exPerformanceCounter2
				});
				list.Add(this.ExchangeExecutingCmdlets);
				this.OrganizationThrottlingPolicyCacheHitCount = new ExPerformanceCounter(base.CategoryName, "Organization Throttling Policy Cache Hit Count", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.OrganizationThrottlingPolicyCacheHitCount);
				this.OrganizationThrottlingPolicyCacheMissCount = new ExPerformanceCounter(base.CategoryName, "Organization Throttling Policy Cache Miss Count", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.OrganizationThrottlingPolicyCacheMissCount);
				this.OrganizationThrottlingPolicyCacheLength = new ExPerformanceCounter(base.CategoryName, "Organization Throttling Policy Cache Length", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.OrganizationThrottlingPolicyCacheLength);
				this.OrganizationThrottlingPolicyCacheLengthPercentage = new ExPerformanceCounter(base.CategoryName, "Organization Throttling Policy Cache Length Percentage", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.OrganizationThrottlingPolicyCacheLengthPercentage);
				this.ThrottlingPolicyCacheHitCount = new ExPerformanceCounter(base.CategoryName, "Throttling Policy Cache Hit Count", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.ThrottlingPolicyCacheHitCount);
				this.ThrottlingPolicyCacheMissCount = new ExPerformanceCounter(base.CategoryName, "Throttling Policy Cache Miss Count", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.ThrottlingPolicyCacheMissCount);
				this.ThrottlingPolicyCacheLength = new ExPerformanceCounter(base.CategoryName, "Throttling Policy Cache Length", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.ThrottlingPolicyCacheLength);
				this.ThrottlingPolicyCacheLengthPercentage = new ExPerformanceCounter(base.CategoryName, "Throttling Policy Cache Length Percentage", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.ThrottlingPolicyCacheLengthPercentage);
				long num = this.AverageThreadSleepTime.RawValue;
				num += 1L;
				flag = true;
			}
			finally
			{
				if (!flag)
				{
					foreach (ExPerformanceCounter exPerformanceCounter3 in list)
					{
						exPerformanceCounter3.Close();
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

		public readonly ExPerformanceCounter AverageThreadSleepTime;

		public readonly ExPerformanceCounter ActivePowerShellRunspaces;

		public readonly ExPerformanceCounter ExchangeExecutingCmdlets;

		public readonly ExPerformanceCounter OrganizationThrottlingPolicyCacheHitCount;

		public readonly ExPerformanceCounter OrganizationThrottlingPolicyCacheMissCount;

		public readonly ExPerformanceCounter OrganizationThrottlingPolicyCacheLength;

		public readonly ExPerformanceCounter OrganizationThrottlingPolicyCacheLengthPercentage;

		public readonly ExPerformanceCounter ThrottlingPolicyCacheHitCount;

		public readonly ExPerformanceCounter ThrottlingPolicyCacheMissCount;

		public readonly ExPerformanceCounter ThrottlingPolicyCacheLength;

		public readonly ExPerformanceCounter ThrottlingPolicyCacheLengthPercentage;
	}
}
