using System;
using System.Collections.Generic;
using Microsoft.Exchange.Cluster.ReplayEventLog;
using Microsoft.Exchange.Cluster.Shared;
using Microsoft.Exchange.Data.HA.DirectoryServices;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Cluster.Replay;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal class IOBufferPool
	{
		public static IOBuffer Allocate()
		{
			return IOBufferPool.Instance.DispenseBuffer();
		}

		public static void Free(IOBuffer buf)
		{
			IOBufferPool.Instance.ReturnBuffer(buf);
		}

		public static void ConfigureCopyCount(int copyCount)
		{
			IOBufferPool.Instance.ConfigureCopyCountInternal(copyCount);
		}

		private IOBuffer DispenseBuffer()
		{
			IOBuffer result;
			lock (this)
			{
				IOBuffer iobuffer;
				if (this.freeList.Count > 0)
				{
					int index = this.freeList.Count - 1;
					iobuffer = this.freeList[index];
					this.freeList.RemoveAt(index);
				}
				else
				{
					if (this.knownList.Count >= this.maxBufferCount)
					{
						throw new IOBufferPoolLimitException(this.maxBufferCount, 1048576);
					}
					iobuffer = new IOBuffer(1048576, false);
					this.knownList.Add(iobuffer);
				}
				result = iobuffer;
			}
			return result;
		}

		private void ReturnBuffer(IOBuffer buf)
		{
			lock (this)
			{
				if (!buf.PreAllocated)
				{
					this.knownList.Remove(buf);
				}
				else
				{
					this.freeList.Add(buf);
				}
			}
		}

		private void ConfigureCopyCountInternal(int copyCount)
		{
			lock (this)
			{
				if (!this.hasPreallocationOccurred)
				{
					IOBufferPool.Tracer.TraceDebug(0L, "IOBufferPool is handling preallocation");
					this.HandlePreallocation(copyCount);
					this.hasPreallocationOccurred = true;
				}
				else
				{
					int num = copyCount * this.buffersPerCopy;
					if (num > this.maxBufferCount)
					{
						IOBufferPool.Tracer.TraceDebug<int>(0L, "IOBufferPool.ConfigureCopyCount increases max buffers to {0}", num);
						this.maxBufferCount = num;
					}
				}
			}
		}

		private void HandlePreallocation(int copyCount)
		{
			int num = 50;
			string text = null;
			int iobufferPoolPreallocationOverride = RegistryParameters.IOBufferPoolPreallocationOverride;
			if (iobufferPoolPreallocationOverride >= 0)
			{
				num = iobufferPoolPreallocationOverride;
				text = string.Format("IOBufferPoolPreallocationOverride was used. BufCount={0}", iobufferPoolPreallocationOverride);
			}
			else
			{
				IMonitoringADConfig monitoringADConfig = null;
				try
				{
					monitoringADConfig = Dependencies.MonitoringADConfigProvider.GetConfig(true);
				}
				catch (MonitoringADConfigException arg)
				{
					IOBufferPool.Tracer.TraceError<MonitoringADConfigException>(0L, "IOBufferPool.HandlePreallocation failed to get adConfig: {0}", arg);
					text = string.Format("Failed to get adConfig: {0}", arg);
				}
				if (monitoringADConfig == null)
				{
					if (text == null)
					{
						text = "Failed to get adConfig but no exception was thrown";
					}
				}
				else if (monitoringADConfig.ServerRole != MonitoringServerRole.DagMember)
				{
					num = 0;
					text = string.Format("Non-DAG role: {0}", monitoringADConfig.ServerRole);
				}
				else
				{
					IADServer srv = monitoringADConfig.LookupMiniServerByName(AmServerName.LocalComputerName);
					int maxMemoryPerDatabase = PassiveBlockMode.GetMaxMemoryPerDatabase(srv);
					int maxBuffersPerDatabase = PassiveBlockMode.GetMaxBuffersPerDatabase(maxMemoryPerDatabase);
					this.buffersPerCopy = maxBuffersPerDatabase;
					IADDatabaseAvailabilityGroup dag = monitoringADConfig.Dag;
					int num2 = 0;
					if (dag.AutoDagTotalNumberOfServers > 0)
					{
						num2 = dag.AutoDagTotalNumberOfDatabases * dag.AutoDagDatabaseCopiesPerDatabase / dag.AutoDagTotalNumberOfServers;
					}
					text = string.Format("BuffersPerCopy={0}. ExpectedCopiesPerServer={1} RIMgr reports {2} copies.", this.buffersPerCopy, num2, copyCount);
					copyCount = Math.Max(copyCount, num2);
					num = copyCount * this.buffersPerCopy;
				}
			}
			if (num > this.maxBufferCount)
			{
				this.maxBufferCount = num;
			}
			this.Preallocate(num);
			ReplayCrimsonEvents.IoBufferPoolInit.Log<int, string>(num, text);
		}

		private void Preallocate(int bufsToPrealloc)
		{
			int num = bufsToPrealloc;
			while (num-- > 0)
			{
				IOBuffer item = new IOBuffer(1048576, true);
				this.knownList.Add(item);
				this.freeList.Add(item);
			}
			IOBufferPool.Tracer.TraceDebug<int>(0L, "IOBufferPool.Preallocated {0} bufs", bufsToPrealloc);
		}

		public const int BufferSize = 1048576;

		public const int DefaultInitialAllocationOfBuffers = 50;

		private static readonly Trace Tracer = ExTraceGlobals.PassiveBlockModeTracer;

		private static IOBufferPool Instance = new IOBufferPool();

		private List<IOBuffer> freeList = new List<IOBuffer>();

		private List<IOBuffer> knownList = new List<IOBuffer>();

		private int maxBufferCount = 50;

		private int buffersPerCopy = 10;

		private bool hasPreallocationOccurred;
	}
}
