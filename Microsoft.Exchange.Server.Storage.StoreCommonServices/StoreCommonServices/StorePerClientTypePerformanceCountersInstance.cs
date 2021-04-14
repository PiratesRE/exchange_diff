using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Server.Storage.StoreCommonServices
{
	internal sealed class StorePerClientTypePerformanceCountersInstance : PerformanceCounterInstance
	{
		internal StorePerClientTypePerformanceCountersInstance(string instanceName, StorePerClientTypePerformanceCountersInstance autoUpdateTotalInstance) : base(instanceName, "MSExchangeIS Client Type")
		{
			bool flag = false;
			List<ExPerformanceCounter> list = new List<ExPerformanceCounter>();
			try
			{
				this.AdminRPCsInProgress = new BufferedPerformanceCounter(base.CategoryName, "Admin RPC Requests", instanceName, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.AdminRPCsInProgress, new ExPerformanceCounter[0]);
				list.Add(this.AdminRPCsInProgress);
				this.AdminRPCsRateOfExecuteTask = new BufferedPerformanceCounter(base.CategoryName, "Administrative RPC requests/sec", instanceName, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.AdminRPCsRateOfExecuteTask, new ExPerformanceCounter[0]);
				list.Add(this.AdminRPCsRateOfExecuteTask);
				this.DirectoryAccessSearchRate = new BufferedPerformanceCounter(base.CategoryName, "Directory Access: LDAP Searches/sec", instanceName, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.DirectoryAccessSearchRate, new ExPerformanceCounter[0]);
				list.Add(this.DirectoryAccessSearchRate);
				this.RPCRequests = new BufferedPerformanceCounter(base.CategoryName, "RPC Requests", instanceName, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.RPCRequests, new ExPerformanceCounter[0]);
				list.Add(this.RPCRequests);
				this.RPCBytesInRate = new BufferedPerformanceCounter(base.CategoryName, "RPC Bytes Received/sec", instanceName, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.RPCBytesInRate, new ExPerformanceCounter[0]);
				list.Add(this.RPCBytesInRate);
				this.RPCBytesOutRate = new BufferedPerformanceCounter(base.CategoryName, "RPC Bytes Sent/sec", instanceName, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.RPCBytesOutRate, new ExPerformanceCounter[0]);
				list.Add(this.RPCBytesOutRate);
				this.RPCPacketsRate = new BufferedPerformanceCounter(base.CategoryName, "RPC Packets/sec", instanceName, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.RPCPacketsRate, new ExPerformanceCounter[0]);
				list.Add(this.RPCPacketsRate);
				this.RPCOperationRate = new BufferedPerformanceCounter(base.CategoryName, "RPC Operations/sec", instanceName, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.RPCOperationRate, new ExPerformanceCounter[0]);
				list.Add(this.RPCOperationRate);
				this.RPCAverageLatency = new BufferedPerformanceCounter(base.CategoryName, "RPC Average Latency", instanceName, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.RPCAverageLatency, new ExPerformanceCounter[0]);
				list.Add(this.RPCAverageLatency);
				this.RPCAverageLatencyBase = new BufferedPerformanceCounter(base.CategoryName, "Average Time spent in an RPC Base", instanceName, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.RPCAverageLatencyBase, new ExPerformanceCounter[0]);
				list.Add(this.RPCAverageLatencyBase);
				this.LazyIndexesCreatedRate = new BufferedPerformanceCounter(base.CategoryName, "Lazy indexes created/sec", instanceName, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.LazyIndexesCreatedRate, new ExPerformanceCounter[0]);
				list.Add(this.LazyIndexesCreatedRate);
				this.LazyIndexesDeletedRate = new BufferedPerformanceCounter(base.CategoryName, "Lazy indexes deleted/sec", instanceName, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.LazyIndexesDeletedRate, new ExPerformanceCounter[0]);
				list.Add(this.LazyIndexesDeletedRate);
				this.LazyIndexesFullRefreshRate = new BufferedPerformanceCounter(base.CategoryName, "Lazy index full refresh/sec", instanceName, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.LazyIndexesFullRefreshRate, new ExPerformanceCounter[0]);
				list.Add(this.LazyIndexesFullRefreshRate);
				this.LazyIndexesIncrementalRefreshRate = new BufferedPerformanceCounter(base.CategoryName, "Lazy index incremental refresh/sec", instanceName, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.LazyIndexesIncrementalRefreshRate, new ExPerformanceCounter[0]);
				list.Add(this.LazyIndexesIncrementalRefreshRate);
				this.MessagesOpenedRate = new BufferedPerformanceCounter(base.CategoryName, "Messages opened/sec", instanceName, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.MessagesOpenedRate, new ExPerformanceCounter[0]);
				list.Add(this.MessagesOpenedRate);
				this.MessagesCreatedRate = new BufferedPerformanceCounter(base.CategoryName, "Messages created/sec", instanceName, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.MessagesCreatedRate, new ExPerformanceCounter[0]);
				list.Add(this.MessagesCreatedRate);
				this.MessagesUpdatedRate = new BufferedPerformanceCounter(base.CategoryName, "Messages updated/sec", instanceName, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.MessagesUpdatedRate, new ExPerformanceCounter[0]);
				list.Add(this.MessagesUpdatedRate);
				this.MessagesDeletedRate = new BufferedPerformanceCounter(base.CategoryName, "Messages deleted/sec", instanceName, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.MessagesDeletedRate, new ExPerformanceCounter[0]);
				list.Add(this.MessagesDeletedRate);
				this.PropertyPromotionRate = new BufferedPerformanceCounter(base.CategoryName, "Property promotions/sec", instanceName, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.PropertyPromotionRate, new ExPerformanceCounter[0]);
				list.Add(this.PropertyPromotionRate);
				this.JetPageReferencedRate = new BufferedPerformanceCounter(base.CategoryName, "Jet Pages Referenced/sec", instanceName, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.JetPageReferencedRate, new ExPerformanceCounter[0]);
				list.Add(this.JetPageReferencedRate);
				this.JetPageReadRate = new BufferedPerformanceCounter(base.CategoryName, "Jet Pages Read/sec", instanceName, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.JetPageReadRate, new ExPerformanceCounter[0]);
				list.Add(this.JetPageReadRate);
				this.JetPagePrereadRate = new BufferedPerformanceCounter(base.CategoryName, "Jet Pages Preread/sec", instanceName, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.JetPagePrereadRate, new ExPerformanceCounter[0]);
				list.Add(this.JetPagePrereadRate);
				this.JetPageDirtiedRate = new BufferedPerformanceCounter(base.CategoryName, "Jet Pages Modified/sec", instanceName, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.JetPageDirtiedRate, new ExPerformanceCounter[0]);
				list.Add(this.JetPageDirtiedRate);
				this.JetPageReDirtiedRate = new BufferedPerformanceCounter(base.CategoryName, "Jet Pages Remodified/sec", instanceName, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.JetPageReDirtiedRate, new ExPerformanceCounter[0]);
				list.Add(this.JetPageReDirtiedRate);
				this.JetLogRecordRate = new BufferedPerformanceCounter(base.CategoryName, "Jet Log Records/sec", instanceName, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.JetLogRecordRate, new ExPerformanceCounter[0]);
				list.Add(this.JetLogRecordRate);
				this.JetLogRecordBytesRate = new BufferedPerformanceCounter(base.CategoryName, "Jet Log Record Bytes/sec", instanceName, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.JetLogRecordBytesRate, new ExPerformanceCounter[0]);
				list.Add(this.JetLogRecordBytesRate);
				long num = this.AdminRPCsInProgress.RawValue;
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

		internal StorePerClientTypePerformanceCountersInstance(string instanceName) : base(instanceName, "MSExchangeIS Client Type")
		{
			bool flag = false;
			List<ExPerformanceCounter> list = new List<ExPerformanceCounter>();
			try
			{
				this.AdminRPCsInProgress = new ExPerformanceCounter(base.CategoryName, "Admin RPC Requests", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.AdminRPCsInProgress);
				this.AdminRPCsRateOfExecuteTask = new ExPerformanceCounter(base.CategoryName, "Administrative RPC requests/sec", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.AdminRPCsRateOfExecuteTask);
				this.DirectoryAccessSearchRate = new ExPerformanceCounter(base.CategoryName, "Directory Access: LDAP Searches/sec", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.DirectoryAccessSearchRate);
				this.RPCRequests = new ExPerformanceCounter(base.CategoryName, "RPC Requests", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.RPCRequests);
				this.RPCBytesInRate = new ExPerformanceCounter(base.CategoryName, "RPC Bytes Received/sec", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.RPCBytesInRate);
				this.RPCBytesOutRate = new ExPerformanceCounter(base.CategoryName, "RPC Bytes Sent/sec", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.RPCBytesOutRate);
				this.RPCPacketsRate = new ExPerformanceCounter(base.CategoryName, "RPC Packets/sec", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.RPCPacketsRate);
				this.RPCOperationRate = new ExPerformanceCounter(base.CategoryName, "RPC Operations/sec", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.RPCOperationRate);
				this.RPCAverageLatency = new ExPerformanceCounter(base.CategoryName, "RPC Average Latency", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.RPCAverageLatency);
				this.RPCAverageLatencyBase = new ExPerformanceCounter(base.CategoryName, "Average Time spent in an RPC Base", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.RPCAverageLatencyBase);
				this.LazyIndexesCreatedRate = new ExPerformanceCounter(base.CategoryName, "Lazy indexes created/sec", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.LazyIndexesCreatedRate);
				this.LazyIndexesDeletedRate = new ExPerformanceCounter(base.CategoryName, "Lazy indexes deleted/sec", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.LazyIndexesDeletedRate);
				this.LazyIndexesFullRefreshRate = new ExPerformanceCounter(base.CategoryName, "Lazy index full refresh/sec", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.LazyIndexesFullRefreshRate);
				this.LazyIndexesIncrementalRefreshRate = new ExPerformanceCounter(base.CategoryName, "Lazy index incremental refresh/sec", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.LazyIndexesIncrementalRefreshRate);
				this.MessagesOpenedRate = new ExPerformanceCounter(base.CategoryName, "Messages opened/sec", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.MessagesOpenedRate);
				this.MessagesCreatedRate = new ExPerformanceCounter(base.CategoryName, "Messages created/sec", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.MessagesCreatedRate);
				this.MessagesUpdatedRate = new ExPerformanceCounter(base.CategoryName, "Messages updated/sec", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.MessagesUpdatedRate);
				this.MessagesDeletedRate = new ExPerformanceCounter(base.CategoryName, "Messages deleted/sec", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.MessagesDeletedRate);
				this.PropertyPromotionRate = new ExPerformanceCounter(base.CategoryName, "Property promotions/sec", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.PropertyPromotionRate);
				this.JetPageReferencedRate = new ExPerformanceCounter(base.CategoryName, "Jet Pages Referenced/sec", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.JetPageReferencedRate);
				this.JetPageReadRate = new ExPerformanceCounter(base.CategoryName, "Jet Pages Read/sec", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.JetPageReadRate);
				this.JetPagePrereadRate = new ExPerformanceCounter(base.CategoryName, "Jet Pages Preread/sec", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.JetPagePrereadRate);
				this.JetPageDirtiedRate = new ExPerformanceCounter(base.CategoryName, "Jet Pages Modified/sec", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.JetPageDirtiedRate);
				this.JetPageReDirtiedRate = new ExPerformanceCounter(base.CategoryName, "Jet Pages Remodified/sec", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.JetPageReDirtiedRate);
				this.JetLogRecordRate = new ExPerformanceCounter(base.CategoryName, "Jet Log Records/sec", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.JetLogRecordRate);
				this.JetLogRecordBytesRate = new ExPerformanceCounter(base.CategoryName, "Jet Log Record Bytes/sec", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.JetLogRecordBytesRate);
				long num = this.AdminRPCsInProgress.RawValue;
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

		public readonly ExPerformanceCounter AdminRPCsInProgress;

		public readonly ExPerformanceCounter AdminRPCsRateOfExecuteTask;

		public readonly ExPerformanceCounter DirectoryAccessSearchRate;

		public readonly ExPerformanceCounter RPCRequests;

		public readonly ExPerformanceCounter RPCBytesInRate;

		public readonly ExPerformanceCounter RPCBytesOutRate;

		public readonly ExPerformanceCounter RPCPacketsRate;

		public readonly ExPerformanceCounter RPCOperationRate;

		public readonly ExPerformanceCounter RPCAverageLatency;

		public readonly ExPerformanceCounter RPCAverageLatencyBase;

		public readonly ExPerformanceCounter LazyIndexesCreatedRate;

		public readonly ExPerformanceCounter LazyIndexesDeletedRate;

		public readonly ExPerformanceCounter LazyIndexesFullRefreshRate;

		public readonly ExPerformanceCounter LazyIndexesIncrementalRefreshRate;

		public readonly ExPerformanceCounter MessagesOpenedRate;

		public readonly ExPerformanceCounter MessagesCreatedRate;

		public readonly ExPerformanceCounter MessagesUpdatedRate;

		public readonly ExPerformanceCounter MessagesDeletedRate;

		public readonly ExPerformanceCounter PropertyPromotionRate;

		public readonly ExPerformanceCounter JetPageReferencedRate;

		public readonly ExPerformanceCounter JetPageReadRate;

		public readonly ExPerformanceCounter JetPagePrereadRate;

		public readonly ExPerformanceCounter JetPageDirtiedRate;

		public readonly ExPerformanceCounter JetPageReDirtiedRate;

		public readonly ExPerformanceCounter JetLogRecordRate;

		public readonly ExPerformanceCounter JetLogRecordBytesRate;
	}
}
