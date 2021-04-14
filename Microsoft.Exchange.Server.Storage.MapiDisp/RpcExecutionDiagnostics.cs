using System;
using System.Diagnostics;
using System.Threading;
using Microsoft.Exchange.RpcClientAccess.Parser;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.DirectoryServices;
using Microsoft.Exchange.Server.Storage.HA;
using Microsoft.Exchange.Server.Storage.PhysicalAccess;
using Microsoft.Exchange.Server.Storage.StoreCommonServices;

namespace Microsoft.Exchange.Server.Storage.MapiDisp
{
	public class RpcExecutionDiagnostics : ExecutionDiagnostics
	{
		public static int AverageRpcLatencyInMilliseconds
		{
			get
			{
				return RpcExecutionDiagnostics.rpcLatencyAverage.Average;
			}
		}

		public TimeSpan RpcLatency
		{
			get
			{
				return this.rpcLatency;
			}
		}

		protected int RpcExecutionCookie
		{
			get
			{
				return this.rpcExecutionCookie;
			}
		}

		public override uint TypeIdentifier
		{
			get
			{
				return 2U;
			}
		}

		public ThrottlingData ReplicationThrottlingData
		{
			get
			{
				return this.replicationThrottlingData;
			}
		}

		public static void ResetLatencyStatistics()
		{
			RpcExecutionDiagnostics.rpcLatencyAverage.Reset();
		}

		internal PerRpcStatisticsAuxiliaryBlock CreateRpcStatisticsAuxiliaryBlock(StorePerDatabasePerformanceCountersInstance perfInstancePerDB)
		{
			DatabaseConnectionStatistics databaseCollector = base.RpcStatistics.DatabaseCollector;
			return new PerRpcStatisticsAuxiliaryBlock((uint)base.RpcStatistics.ElapsedTime.TotalMilliseconds, (uint)(base.RpcStatistics.CpuKernelTime.TotalMilliseconds + base.RpcStatistics.CpuUserTime.TotalMilliseconds), (uint)databaseCollector.ThreadStats.cPageRead, (uint)databaseCollector.ThreadStats.cPagePreread, (uint)databaseCollector.ThreadStats.cLogRecord, (uint)databaseCollector.ThreadStats.cbLogRecord, 0UL, 0UL, (uint)RpcExecutionDiagnostics.AverageRpcLatencyInMilliseconds, (uint)RpcExecutionDiagnostics.AverageRpcLatencyInMilliseconds, 0U, (perfInstancePerDB != null) ? ((uint)perfInstancePerDB.RateOfROPs.RawValue) : 1U, 0U, 0U, 0U, (uint)RpcExecutionDiagnostics.currentProcessId, this.GetDataProtectionHealth(), this.GetDataAvailabilityHealth(), Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.GetCurrentServerCPUUsagePercentage());
		}

		internal override void EnablePerClientTypePerfCounterUpdate()
		{
			base.EnablePerClientTypePerfCounterUpdate();
		}

		internal override void DisablePerClientTypePerfCounterUpdate()
		{
			if (base.PerClientPerfInstance != null)
			{
				base.PerClientPerfInstance.DirectoryAccessSearchRate.IncrementBy((long)base.RpcStatistics.DirectoryCount);
			}
			base.DisablePerClientTypePerfCounterUpdate();
		}

		protected void OnRpcBegin()
		{
			this.rpcStartTimeStamp = StopwatchStamp.GetStamp();
			this.rpcExecutionCookie = Interlocked.Increment(ref RpcExecutionDiagnostics.lastRpcExecutionCookieCookie);
			base.OnBeginRpc();
		}

		protected void OnRpcEnd()
		{
			ResourceDigestStats activity = new ResourceDigestStats
			{
				TimeInCPU = base.RpcStatistics.CpuKernelTime + base.RpcStatistics.CpuUserTime
			};
			base.ActivityCollector.LogActivity(activity);
			this.rpcLatency = this.rpcStartTimeStamp.ElapsedTime;
			RpcExecutionDiagnostics.rpcLatencyAverage.AddValue((int)base.RpcStatistics.ElapsedTime.TotalMilliseconds);
		}

		private uint GetDataProtectionHealth()
		{
			return (uint)this.ReplicationThrottlingData.DataProtectionHealth;
		}

		private uint GetDataAvailabilityHealth()
		{
			return (uint)this.ReplicationThrottlingData.DataAvailabilityHealth;
		}

		private static int currentProcessId = Process.GetCurrentProcess().Id;

		private static int lastRpcExecutionCookieCookie = 0;

		private static MovingAverage rpcLatencyAverage = new MovingAverage();

		private StopwatchStamp rpcStartTimeStamp;

		private TimeSpan rpcLatency;

		private int rpcExecutionCookie;

		private ThrottlingData replicationThrottlingData = new ThrottlingData();
	}
}
