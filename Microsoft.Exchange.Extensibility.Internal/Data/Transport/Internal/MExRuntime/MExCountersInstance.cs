using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Transport.Internal.MExRuntime
{
	internal sealed class MExCountersInstance : PerformanceCounterInstance
	{
		internal MExCountersInstance(string instanceName, MExCountersInstance autoUpdateTotalInstance) : base(instanceName, MExCounters.CategoryName)
		{
			bool flag = false;
			List<ExPerformanceCounter> list = new List<ExPerformanceCounter>();
			try
			{
				this.AverageAgentDelay = new ExPerformanceCounter(base.CategoryName, "Average Agent Processing Time (sec)", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.AverageAgentDelay);
				this.AverageAgentDelayBase = new ExPerformanceCounter(base.CategoryName, "Average Agent Processing Time Base (sec)", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.AverageAgentDelayBase);
				this.AverageAgentProcessorUsageSynchronousInvocations = new ExPerformanceCounter(base.CategoryName, "Average CPU usage of synchronous invocations of agent (milliseconds)", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.AverageAgentProcessorUsageSynchronousInvocations);
				this.SynchronousAgentInvocationSamples = new ExPerformanceCounter(base.CategoryName, "Samples used to calculate Average CPU usage of synchronous invocations", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.SynchronousAgentInvocationSamples);
				this.AverageAgentProcessorUsageAsynchronousInvocations = new ExPerformanceCounter(base.CategoryName, "Average CPU usage of asynchronous invocations of agent (milliseconds)", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.AverageAgentProcessorUsageAsynchronousInvocations);
				this.AsynchronousAgentInvocationSamples = new ExPerformanceCounter(base.CategoryName, "Samples used to calculate Average CPU usage of asynchronous invocations", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.AsynchronousAgentInvocationSamples);
				this.TotalAgentInvocations = new ExPerformanceCounter(base.CategoryName, "Total Agent Invocations", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.TotalAgentInvocations);
				this.TotalAgentErrorHandlingOverrides = new ExPerformanceCounter(base.CategoryName, "Total Agent Error Handling Overrides", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.TotalAgentErrorHandlingOverrides);
				long num = this.AverageAgentDelay.RawValue;
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

		internal MExCountersInstance(string instanceName) : base(instanceName, MExCounters.CategoryName)
		{
			bool flag = false;
			List<ExPerformanceCounter> list = new List<ExPerformanceCounter>();
			try
			{
				this.AverageAgentDelay = new ExPerformanceCounter(base.CategoryName, "Average Agent Processing Time (sec)", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.AverageAgentDelay);
				this.AverageAgentDelayBase = new ExPerformanceCounter(base.CategoryName, "Average Agent Processing Time Base (sec)", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.AverageAgentDelayBase);
				this.AverageAgentProcessorUsageSynchronousInvocations = new ExPerformanceCounter(base.CategoryName, "Average CPU usage of synchronous invocations of agent (milliseconds)", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.AverageAgentProcessorUsageSynchronousInvocations);
				this.SynchronousAgentInvocationSamples = new ExPerformanceCounter(base.CategoryName, "Samples used to calculate Average CPU usage of synchronous invocations", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.SynchronousAgentInvocationSamples);
				this.AverageAgentProcessorUsageAsynchronousInvocations = new ExPerformanceCounter(base.CategoryName, "Average CPU usage of asynchronous invocations of agent (milliseconds)", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.AverageAgentProcessorUsageAsynchronousInvocations);
				this.AsynchronousAgentInvocationSamples = new ExPerformanceCounter(base.CategoryName, "Samples used to calculate Average CPU usage of asynchronous invocations", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.AsynchronousAgentInvocationSamples);
				this.TotalAgentInvocations = new ExPerformanceCounter(base.CategoryName, "Total Agent Invocations", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.TotalAgentInvocations);
				this.TotalAgentErrorHandlingOverrides = new ExPerformanceCounter(base.CategoryName, "Total Agent Error Handling Overrides", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.TotalAgentErrorHandlingOverrides);
				long num = this.AverageAgentDelay.RawValue;
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

		public readonly ExPerformanceCounter AverageAgentDelay;

		public readonly ExPerformanceCounter AverageAgentDelayBase;

		public readonly ExPerformanceCounter AverageAgentProcessorUsageSynchronousInvocations;

		public readonly ExPerformanceCounter SynchronousAgentInvocationSamples;

		public readonly ExPerformanceCounter AverageAgentProcessorUsageAsynchronousInvocations;

		public readonly ExPerformanceCounter AsynchronousAgentInvocationSamples;

		public readonly ExPerformanceCounter TotalAgentInvocations;

		public readonly ExPerformanceCounter TotalAgentErrorHandlingOverrides;
	}
}
