using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Configuration.Core
{
	internal sealed class RemotePowershellPerformanceCountersInstance : PerformanceCounterInstance
	{
		internal RemotePowershellPerformanceCountersInstance(string instanceName, RemotePowershellPerformanceCountersInstance autoUpdateTotalInstance) : base(instanceName, "MSExchangeRemotePowershell")
		{
			bool flag = false;
			List<ExPerformanceCounter> list = new List<ExPerformanceCounter>();
			try
			{
				this.PID = new ExPerformanceCounter(base.CategoryName, "Process ID", instanceName, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.PID, new ExPerformanceCounter[0]);
				list.Add(this.PID);
				this.CurrentSessions = new ExPerformanceCounter(base.CategoryName, "Current Connected Sessions", instanceName, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.CurrentSessions, new ExPerformanceCounter[0]);
				list.Add(this.CurrentSessions);
				this.CurrentUniqueUsers = new ExPerformanceCounter(base.CategoryName, "Current Connected Unique Users", instanceName, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.CurrentUniqueUsers, new ExPerformanceCounter[0]);
				list.Add(this.CurrentUniqueUsers);
				this.FailFastUserCacheSize = new ExPerformanceCounter(base.CategoryName, "FailFast User Cache Size", instanceName, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.FailFastUserCacheSize, new ExPerformanceCounter[0]);
				list.Add(this.FailFastUserCacheSize);
				this.ConnectedUserCacheSize = new ExPerformanceCounter(base.CategoryName, "Connected User Cache Size", instanceName, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.ConnectedUserCacheSize, new ExPerformanceCounter[0]);
				list.Add(this.ConnectedUserCacheSize);
				this.AuthenticatedUserCacheSize = new ExPerformanceCounter(base.CategoryName, "Authenticated User Cache Size", instanceName, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.AuthenticatedUserCacheSize, new ExPerformanceCounter[0]);
				list.Add(this.AuthenticatedUserCacheSize);
				this.PerUserBudgetsDicSize = new ExPerformanceCounter(base.CategoryName, "Per-User Budgets Dictionary Size", instanceName, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.PerUserBudgetsDicSize, new ExPerformanceCounter[0]);
				list.Add(this.PerUserBudgetsDicSize);
				this.PerTenantBudgetsDicSize = new ExPerformanceCounter(base.CategoryName, "Per-Tenant Budgets Dictionary Size", instanceName, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.PerTenantBudgetsDicSize, new ExPerformanceCounter[0]);
				list.Add(this.PerTenantBudgetsDicSize);
				this.PerUserKeyToRemoveBudgetsCacheSize = new ExPerformanceCounter(base.CategoryName, "Per-User KeyToRemoveBudgets Cache Size", instanceName, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.PerUserKeyToRemoveBudgetsCacheSize, new ExPerformanceCounter[0]);
				list.Add(this.PerUserKeyToRemoveBudgetsCacheSize);
				this.PerTenantKeyToRemoveBudgetsCacheSize = new ExPerformanceCounter(base.CategoryName, "Per-Tenant KeyToRemoveBudgets Cache Size", instanceName, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.PerTenantKeyToRemoveBudgetsCacheSize, new ExPerformanceCounter[0]);
				list.Add(this.PerTenantKeyToRemoveBudgetsCacheSize);
				this.FailureThrottlingUserCacheSize = new ExPerformanceCounter(base.CategoryName, "Failure Throttling User Cache Size", instanceName, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.FailureThrottlingUserCacheSize, new ExPerformanceCounter[0]);
				list.Add(this.FailureThrottlingUserCacheSize);
				this.RequestsBeFailFasted = new ExPerformanceCounter(base.CategoryName, "Requests be Fail-fasted", instanceName, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.RequestsBeFailFasted, new ExPerformanceCounter[0]);
				list.Add(this.RequestsBeFailFasted);
				this.ConnectionLeak = new ExPerformanceCounter(base.CategoryName, "Connection Leak", instanceName, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.ConnectionLeak, new ExPerformanceCounter[0]);
				list.Add(this.ConnectionLeak);
				this.AverageResponseTime = new ExPerformanceCounter(base.CategoryName, "PowerShell Average Response Time", instanceName, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.AverageResponseTime, new ExPerformanceCounter[0]);
				list.Add(this.AverageResponseTime);
				long num = this.PID.RawValue;
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

		internal RemotePowershellPerformanceCountersInstance(string instanceName) : base(instanceName, "MSExchangeRemotePowershell")
		{
			bool flag = false;
			List<ExPerformanceCounter> list = new List<ExPerformanceCounter>();
			try
			{
				this.PID = new ExPerformanceCounter(base.CategoryName, "Process ID", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.PID);
				this.CurrentSessions = new ExPerformanceCounter(base.CategoryName, "Current Connected Sessions", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.CurrentSessions);
				this.CurrentUniqueUsers = new ExPerformanceCounter(base.CategoryName, "Current Connected Unique Users", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.CurrentUniqueUsers);
				this.FailFastUserCacheSize = new ExPerformanceCounter(base.CategoryName, "FailFast User Cache Size", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.FailFastUserCacheSize);
				this.ConnectedUserCacheSize = new ExPerformanceCounter(base.CategoryName, "Connected User Cache Size", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.ConnectedUserCacheSize);
				this.AuthenticatedUserCacheSize = new ExPerformanceCounter(base.CategoryName, "Authenticated User Cache Size", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.AuthenticatedUserCacheSize);
				this.PerUserBudgetsDicSize = new ExPerformanceCounter(base.CategoryName, "Per-User Budgets Dictionary Size", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.PerUserBudgetsDicSize);
				this.PerTenantBudgetsDicSize = new ExPerformanceCounter(base.CategoryName, "Per-Tenant Budgets Dictionary Size", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.PerTenantBudgetsDicSize);
				this.PerUserKeyToRemoveBudgetsCacheSize = new ExPerformanceCounter(base.CategoryName, "Per-User KeyToRemoveBudgets Cache Size", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.PerUserKeyToRemoveBudgetsCacheSize);
				this.PerTenantKeyToRemoveBudgetsCacheSize = new ExPerformanceCounter(base.CategoryName, "Per-Tenant KeyToRemoveBudgets Cache Size", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.PerTenantKeyToRemoveBudgetsCacheSize);
				this.FailureThrottlingUserCacheSize = new ExPerformanceCounter(base.CategoryName, "Failure Throttling User Cache Size", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.FailureThrottlingUserCacheSize);
				this.RequestsBeFailFasted = new ExPerformanceCounter(base.CategoryName, "Requests be Fail-fasted", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.RequestsBeFailFasted);
				this.ConnectionLeak = new ExPerformanceCounter(base.CategoryName, "Connection Leak", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.ConnectionLeak);
				this.AverageResponseTime = new ExPerformanceCounter(base.CategoryName, "PowerShell Average Response Time", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.AverageResponseTime);
				long num = this.PID.RawValue;
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

		public readonly ExPerformanceCounter PID;

		public readonly ExPerformanceCounter CurrentSessions;

		public readonly ExPerformanceCounter CurrentUniqueUsers;

		public readonly ExPerformanceCounter FailFastUserCacheSize;

		public readonly ExPerformanceCounter ConnectedUserCacheSize;

		public readonly ExPerformanceCounter AuthenticatedUserCacheSize;

		public readonly ExPerformanceCounter PerUserBudgetsDicSize;

		public readonly ExPerformanceCounter PerTenantBudgetsDicSize;

		public readonly ExPerformanceCounter PerUserKeyToRemoveBudgetsCacheSize;

		public readonly ExPerformanceCounter PerTenantKeyToRemoveBudgetsCacheSize;

		public readonly ExPerformanceCounter FailureThrottlingUserCacheSize;

		public readonly ExPerformanceCounter RequestsBeFailFasted;

		public readonly ExPerformanceCounter ConnectionLeak;

		public readonly ExPerformanceCounter AverageResponseTime;
	}
}
