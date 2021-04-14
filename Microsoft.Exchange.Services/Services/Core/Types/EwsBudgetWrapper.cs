using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Mapi.Unmanaged;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal class EwsBudgetWrapper : StandardBudgetWrapper, IEwsBudget, IStandardBudget, IBudget, IDisposable
	{
		public EwsBudgetWrapper(EwsBudget innerBudget) : base(innerBudget)
		{
			this.initGen0Collections = GC.CollectionCount(0);
			this.initGen1Collections = GC.CollectionCount(1);
			this.initGen2Collections = GC.CollectionCount(2);
			this.connectionCostType = EwsBudget.GetConnectionCostType();
			this.LogBudgetToIIS(true);
		}

		public bool SleepIfNecessary()
		{
			int num;
			float num2;
			return this.SleepIfNecessary(out num, out num2);
		}

		public bool SleepIfNecessary(out int sleepTime, out float cpuPercent)
		{
			if (CPUBasedSleeper.SleepIfNecessary(out sleepTime, out cpuPercent))
			{
				ThrottlingLogInfo.AddDataPoint(sleepTime, cpuPercent);
				return true;
			}
			return false;
		}

		public void LogEndStateToIIS()
		{
			this.SetContextItemIfNotAlreadySet("TotalLdapRequestCount", this.TotalLdapRequestCount);
			this.SetContextItemIfNotAlreadySet("TotalLdapRequestLatency", this.TotalLdapRequestLatency);
			this.SetContextItemIfNotAlreadySet("TotalRpcRequestCount", this.TotalRpcRequestCount);
			this.SetContextItemIfNotAlreadySet("TotalRpcRequestLatency", this.TotalRpcRequestLatency);
			this.LogBudgetToIIS(false);
		}

		public uint TotalRpcRequestCount
		{
			get
			{
				return this.totalRpcRequestCount;
			}
		}

		public ulong TotalRpcRequestLatency
		{
			get
			{
				return this.totalRpcRequestLatencyInTicks / 10000UL;
			}
		}

		public uint TotalLdapRequestCount
		{
			get
			{
				return this.totalLdapRequestCount;
			}
		}

		public long TotalLdapRequestLatency
		{
			get
			{
				return this.totalLdapRequestLatencyInMsec;
			}
		}

		public bool TryIncrementFoundObjectCount(uint foundCount, out int maxPossible)
		{
			if (this.CanAllocateFoundObjects(foundCount, out maxPossible))
			{
				this.foundObjects += foundCount;
				return true;
			}
			return false;
		}

		public bool CanAllocateFoundObjects(uint foundCount, out int maxPossible)
		{
			int findCountLimit = Global.FindCountLimit;
			ExTraceGlobals.FaultInjectionTracer.TraceTest<int>(3913690429U, ref findCountLimit);
			int num = findCountLimit - (int)this.foundObjects;
			if ((ulong)foundCount <= (ulong)((long)num))
			{
				maxPossible = (int)foundCount;
				return true;
			}
			maxPossible = Math.Max(0, num);
			return false;
		}

		public void StartPerformanceContext()
		{
			NativeMethods.GetTLSPerformanceContext(out this.rpcPerformanceContext);
			this.ldapPerformanceContext = new PerformanceContext(PerformanceContext.Current);
			this.perfDataThreadId = Environment.CurrentManagedThreadId;
		}

		public void StopPerformanceContext()
		{
			if (Environment.CurrentManagedThreadId == this.perfDataThreadId)
			{
				PerformanceContext performanceContext;
				if (NativeMethods.GetTLSPerformanceContext(out performanceContext))
				{
					this.totalRpcRequestCount += performanceContext.rpcCount - this.rpcPerformanceContext.rpcCount;
					this.totalRpcRequestLatencyInTicks += performanceContext.rpcLatency - this.rpcPerformanceContext.rpcLatency;
				}
				if (this.ldapPerformanceContext != null)
				{
					this.totalLdapRequestCount += PerformanceContext.Current.RequestCount - this.ldapPerformanceContext.RequestCount;
					this.totalLdapRequestLatencyInMsec += (long)(PerformanceContext.Current.RequestLatency - this.ldapPerformanceContext.RequestLatency);
				}
				this.perfDataThreadId = -1;
			}
		}

		protected override CostHandle InternalStartConnection(string callerInfo)
		{
			EwsBudget ewsBudget = (EwsBudget)base.GetInnerBudget();
			if (this.connectionCostType != CostType.Connection)
			{
				return ewsBudget.StartHangingConnection(new Action<CostHandle>(base.HandleCostHandleRelease));
			}
			return ewsBudget.StartConnection(new Action<CostHandle>(base.HandleCostHandleRelease), callerInfo);
		}

		private string GetGCInfo()
		{
			int num = GC.CollectionCount(0) - this.initGen0Collections;
			int num2 = GC.CollectionCount(1) - this.initGen1Collections;
			int num3 = GC.CollectionCount(2) - this.initGen2Collections;
			if (num == 0 && num2 == 0 && num3 == 0)
			{
				return string.Empty;
			}
			return string.Format(";GC:{0}/{1}/{2};", num, num2, num3);
		}

		private void LogBudgetToIIS(bool isInit)
		{
			if (Global.WriteRequestDetailsToLog && CallContext.Current != null && CallContext.Current.HttpContext != null && CallContext.Current.HttpContext.Items != null)
			{
				string key = isInit ? "StartBudget" : "EndBudget";
				string text = this.ToString();
				if (!isInit)
				{
					text += this.GetGCInfo();
				}
				CallContext.Current.HttpContext.Items[key] = text;
			}
		}

		private void SetContextItemIfNotAlreadySet(string key, object value)
		{
			if (CallContext.Current != null && CallContext.Current.HttpContext != null && CallContext.Current.HttpContext.Items != null && CallContext.Current.HttpContext.Items[key] == null)
			{
				CallContext.Current.HttpContext.Items[key] = value;
			}
		}

		protected override StandardBudget ReacquireBudget()
		{
			return EwsBudgetCache.Singleton.Get(base.Owner);
		}

		private readonly int initGen0Collections;

		private readonly int initGen1Collections;

		private readonly int initGen2Collections;

		private CostType connectionCostType;

		private uint foundObjects;

		private int perfDataThreadId;

		private PerformanceContext rpcPerformanceContext;

		private PerformanceContext ldapPerformanceContext;

		private uint totalRpcRequestCount;

		private ulong totalRpcRequestLatencyInTicks;

		private uint totalLdapRequestCount;

		private long totalLdapRequestLatencyInMsec;
	}
}
