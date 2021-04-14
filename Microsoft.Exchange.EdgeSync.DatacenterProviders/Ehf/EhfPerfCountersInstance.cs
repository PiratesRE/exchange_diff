using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.EdgeSync.Ehf
{
	internal sealed class EhfPerfCountersInstance : PerformanceCounterInstance
	{
		internal EhfPerfCountersInstance(string instanceName, EhfPerfCountersInstance autoUpdateTotalInstance) : base(instanceName, "MSExchangeEdgeSync EHF Sync Operations")
		{
			bool flag = false;
			List<ExPerformanceCounter> list = new List<ExPerformanceCounter>();
			try
			{
				this.PermanentEntryFailuresTotal = new ExPerformanceCounter(base.CategoryName, "Permanent Entry Failures Total", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.PermanentEntryFailuresTotal, new ExPerformanceCounter[0]);
				list.Add(this.PermanentEntryFailuresTotal);
				this.TransientEntryFailuresTotal = new ExPerformanceCounter(base.CategoryName, "Transient Entry Failures Total", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.TransientEntryFailuresTotal, new ExPerformanceCounter[0]);
				list.Add(this.TransientEntryFailuresTotal);
				this.EntryCountTotal = new ExPerformanceCounter(base.CategoryName, "Total Count of Entries for All Operations", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.EntryCountTotal, new ExPerformanceCounter[0]);
				list.Add(this.EntryCountTotal);
				this.LastEntryCount = new ExPerformanceCounter(base.CategoryName, "Count of Entries in the Last Operation", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.LastEntryCount, new ExPerformanceCounter[0]);
				list.Add(this.LastEntryCount);
				this.LastLatency = new ExPerformanceCounter(base.CategoryName, "Latency (msec) for the Last Operation Executed", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.LastLatency, new ExPerformanceCounter[0]);
				list.Add(this.LastLatency);
				this.AverageLatency = new ExPerformanceCounter(base.CategoryName, "Average Latency (msec) for the Operation", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.AverageLatency, new ExPerformanceCounter[0]);
				list.Add(this.AverageLatency);
				this.AverageLatencyBase = new ExPerformanceCounter(base.CategoryName, "Average Latency Base (msec) for the Operation", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.AverageLatencyBase, new ExPerformanceCounter[0]);
				list.Add(this.AverageLatencyBase);
				this.LastLatencyPerEntry = new ExPerformanceCounter(base.CategoryName, "Latency (msec) Averaged Out for the Entries in the Last Operation Executed", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.LastLatencyPerEntry, new ExPerformanceCounter[0]);
				list.Add(this.LastLatencyPerEntry);
				this.AverageLatencyPerEntry = new ExPerformanceCounter(base.CategoryName, "Average Latency Per Entry (msec) for the Operation", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.AverageLatencyPerEntry, new ExPerformanceCounter[0]);
				list.Add(this.AverageLatencyPerEntry);
				this.AverageLatencyPerEntryBase = new ExPerformanceCounter(base.CategoryName, "Average Latency Per Entry Base (msec) for the Operation", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.AverageLatencyPerEntryBase, new ExPerformanceCounter[0]);
				list.Add(this.AverageLatencyPerEntryBase);
				this.OperationsTotal = new ExPerformanceCounter(base.CategoryName, "Operations Total", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.OperationsTotal, new ExPerformanceCounter[0]);
				list.Add(this.OperationsTotal);
				this.SuccessfulOperationsTotal = new ExPerformanceCounter(base.CategoryName, "Successful Operations Total", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.SuccessfulOperationsTotal, new ExPerformanceCounter[0]);
				list.Add(this.SuccessfulOperationsTotal);
				this.TransientFailedOperationsTotal = new ExPerformanceCounter(base.CategoryName, "Transient Failed Operations Total", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.TransientFailedOperationsTotal, new ExPerformanceCounter[0]);
				list.Add(this.TransientFailedOperationsTotal);
				this.TimeoutFailedOperationsTotal = new ExPerformanceCounter(base.CategoryName, "Timeout Failed Operations Total", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.TimeoutFailedOperationsTotal, new ExPerformanceCounter[0]);
				list.Add(this.TimeoutFailedOperationsTotal);
				this.CommunicationFailedOperationsTotal = new ExPerformanceCounter(base.CategoryName, "Communication Failed Operations Total", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.CommunicationFailedOperationsTotal, new ExPerformanceCounter[0]);
				list.Add(this.CommunicationFailedOperationsTotal);
				this.ContractViolationFailedOperationsTotal = new ExPerformanceCounter(base.CategoryName, "Contract Violation Failed Operations Total", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.ContractViolationFailedOperationsTotal, new ExPerformanceCounter[0]);
				list.Add(this.ContractViolationFailedOperationsTotal);
				this.InvalidCredentialsFailedOperationsTotal = new ExPerformanceCounter(base.CategoryName, "Invalid Credentials Failed Operations Total", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.InvalidCredentialsFailedOperationsTotal, new ExPerformanceCounter[0]);
				list.Add(this.InvalidCredentialsFailedOperationsTotal);
				long num = this.PermanentEntryFailuresTotal.RawValue;
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

		internal EhfPerfCountersInstance(string instanceName) : base(instanceName, "MSExchangeEdgeSync EHF Sync Operations")
		{
			bool flag = false;
			List<ExPerformanceCounter> list = new List<ExPerformanceCounter>();
			try
			{
				this.PermanentEntryFailuresTotal = new ExPerformanceCounter(base.CategoryName, "Permanent Entry Failures Total", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.PermanentEntryFailuresTotal);
				this.TransientEntryFailuresTotal = new ExPerformanceCounter(base.CategoryName, "Transient Entry Failures Total", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.TransientEntryFailuresTotal);
				this.EntryCountTotal = new ExPerformanceCounter(base.CategoryName, "Total Count of Entries for All Operations", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.EntryCountTotal);
				this.LastEntryCount = new ExPerformanceCounter(base.CategoryName, "Count of Entries in the Last Operation", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.LastEntryCount);
				this.LastLatency = new ExPerformanceCounter(base.CategoryName, "Latency (msec) for the Last Operation Executed", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.LastLatency);
				this.AverageLatency = new ExPerformanceCounter(base.CategoryName, "Average Latency (msec) for the Operation", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.AverageLatency);
				this.AverageLatencyBase = new ExPerformanceCounter(base.CategoryName, "Average Latency Base (msec) for the Operation", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.AverageLatencyBase);
				this.LastLatencyPerEntry = new ExPerformanceCounter(base.CategoryName, "Latency (msec) Averaged Out for the Entries in the Last Operation Executed", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.LastLatencyPerEntry);
				this.AverageLatencyPerEntry = new ExPerformanceCounter(base.CategoryName, "Average Latency Per Entry (msec) for the Operation", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.AverageLatencyPerEntry);
				this.AverageLatencyPerEntryBase = new ExPerformanceCounter(base.CategoryName, "Average Latency Per Entry Base (msec) for the Operation", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.AverageLatencyPerEntryBase);
				this.OperationsTotal = new ExPerformanceCounter(base.CategoryName, "Operations Total", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.OperationsTotal);
				this.SuccessfulOperationsTotal = new ExPerformanceCounter(base.CategoryName, "Successful Operations Total", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.SuccessfulOperationsTotal);
				this.TransientFailedOperationsTotal = new ExPerformanceCounter(base.CategoryName, "Transient Failed Operations Total", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.TransientFailedOperationsTotal);
				this.TimeoutFailedOperationsTotal = new ExPerformanceCounter(base.CategoryName, "Timeout Failed Operations Total", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.TimeoutFailedOperationsTotal);
				this.CommunicationFailedOperationsTotal = new ExPerformanceCounter(base.CategoryName, "Communication Failed Operations Total", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.CommunicationFailedOperationsTotal);
				this.ContractViolationFailedOperationsTotal = new ExPerformanceCounter(base.CategoryName, "Contract Violation Failed Operations Total", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.ContractViolationFailedOperationsTotal);
				this.InvalidCredentialsFailedOperationsTotal = new ExPerformanceCounter(base.CategoryName, "Invalid Credentials Failed Operations Total", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.InvalidCredentialsFailedOperationsTotal);
				long num = this.PermanentEntryFailuresTotal.RawValue;
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

		public readonly ExPerformanceCounter PermanentEntryFailuresTotal;

		public readonly ExPerformanceCounter TransientEntryFailuresTotal;

		public readonly ExPerformanceCounter EntryCountTotal;

		public readonly ExPerformanceCounter LastEntryCount;

		public readonly ExPerformanceCounter LastLatency;

		public readonly ExPerformanceCounter AverageLatency;

		public readonly ExPerformanceCounter AverageLatencyBase;

		public readonly ExPerformanceCounter LastLatencyPerEntry;

		public readonly ExPerformanceCounter AverageLatencyPerEntry;

		public readonly ExPerformanceCounter AverageLatencyPerEntryBase;

		public readonly ExPerformanceCounter OperationsTotal;

		public readonly ExPerformanceCounter SuccessfulOperationsTotal;

		public readonly ExPerformanceCounter TransientFailedOperationsTotal;

		public readonly ExPerformanceCounter TimeoutFailedOperationsTotal;

		public readonly ExPerformanceCounter CommunicationFailedOperationsTotal;

		public readonly ExPerformanceCounter ContractViolationFailedOperationsTotal;

		public readonly ExPerformanceCounter InvalidCredentialsFailedOperationsTotal;
	}
}
