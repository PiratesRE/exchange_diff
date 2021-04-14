using System;
using System.Diagnostics;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Extensibility.Internal;

namespace Microsoft.Exchange.Data.Transport.Internal.MExRuntime
{
	internal sealed class MExPerfCounters
	{
		public MExPerfCounters(ProcessTransportRole processRole, AgentRecord[] agentRecords)
		{
			this.agentRecords = agentRecords;
			MExCounters.SetCategoryName(MExPerfCounters.GetPerformanceCounterCategory(processRole));
			this.synchronousAgentProcessorUsageSlidingCounters = new SlidingAverageCounter[this.agentRecords.Length];
			this.asynchronousAgentProcessorUsageSlidingCounters = new SlidingAverageCounter[this.agentRecords.Length];
			for (int i = 0; i < this.agentRecords.Length; i++)
			{
				MExCounters.GetInstance(this.agentRecords[i].Name);
				this.synchronousAgentProcessorUsageSlidingCounters[this.agentRecords[i].SequenceNumber] = new SlidingAverageCounter(MExPerfCounters.slidingWindowLengthForAgentProcessorUsageCounters, MExPerfCounters.bucketLengthForAgentProcessorUsageCounters);
				this.asynchronousAgentProcessorUsageSlidingCounters[this.agentRecords[i].SequenceNumber] = new SlidingAverageCounter(MExPerfCounters.slidingWindowLengthForAgentProcessorUsageCounters, MExPerfCounters.bucketLengthForAgentProcessorUsageCounters);
			}
		}

		public void Shutdown()
		{
			for (int i = 0; i < this.agentRecords.Length; i++)
			{
				MExCounters.RemoveInstance(this.agentRecords[i].Name);
			}
		}

		internal void Subscribe(IDispatcher dispatcher)
		{
			dispatcher.OnAgentInvokeStart += this.AgentInvokeStartHandler;
			dispatcher.OnAgentInvokeEnd += this.AgentInvokeEndHandler;
			dispatcher.OnAgentInvokeReturns += this.AgentInvokeReturnsHandler;
		}

		internal void AgentInvokeStartHandler(object source, MExSession context)
		{
			context.BeginInvokeTicks = Stopwatch.GetTimestamp();
		}

		internal void AgentInvokeEndHandler(object dispatcher, MExSession context)
		{
			MExCountersInstance instance = MExCounters.GetInstance(context.CurrentAgent.Name);
			long timestamp = Stopwatch.GetTimestamp();
			instance.AverageAgentDelay.IncrementBy(timestamp - context.BeginInvokeTicks);
			instance.AverageAgentDelayBase.Increment();
			instance.TotalAgentInvocations.Increment();
		}

		internal void AgentInvokeReturnsHandler(object dispatcher, MExSession context)
		{
			MExCountersInstance instance = MExCounters.GetInstance(context.CurrentAgent.Name);
			if (context.IsAsyncAgent)
			{
				SlidingAverageCounter slidingAverageCounter = this.asynchronousAgentProcessorUsageSlidingCounters[context.CurrentAgent.SequenceNumber];
				slidingAverageCounter.AddValue(context.TotalProcessorTime);
				long rawValue2;
				long rawValue = slidingAverageCounter.CalculateAverageAcrossAllSamples(out rawValue2);
				instance.AverageAgentProcessorUsageAsynchronousInvocations.RawValue = rawValue;
				instance.AsynchronousAgentInvocationSamples.RawValue = rawValue2;
			}
			else
			{
				SlidingAverageCounter slidingAverageCounter2 = this.synchronousAgentProcessorUsageSlidingCounters[context.CurrentAgent.SequenceNumber];
				slidingAverageCounter2.AddValue(context.TotalProcessorTime);
				long rawValue4;
				long rawValue3 = slidingAverageCounter2.CalculateAverageAcrossAllSamples(out rawValue4);
				instance.AverageAgentProcessorUsageSynchronousInvocations.RawValue = rawValue3;
				instance.SynchronousAgentInvocationSamples.RawValue = rawValue4;
			}
			context.CleanupCpuTracker();
		}

		private static string GetPerformanceCounterCategory(ProcessTransportRole processRole)
		{
			switch (processRole)
			{
			case ProcessTransportRole.Hub:
			case ProcessTransportRole.Edge:
				return "MSExchangeTransport Extensibility Agents";
			case ProcessTransportRole.FrontEnd:
				return "MSExchangeFrontEndTransport Extensibility Agents";
			case ProcessTransportRole.MailboxSubmission:
				return "MSExchange Submission Extensibility Agents";
			case ProcessTransportRole.MailboxDelivery:
				return "MSExchange Delivery Extensibility Agents";
			default:
				throw new InvalidOperationException(string.Format("Performance counter category not defined for process role {0}", processRole));
			}
		}

		private static readonly TimeSpan slidingWindowLengthForAgentProcessorUsageCounters = TimeSpan.FromMinutes(10.0);

		private static readonly TimeSpan bucketLengthForAgentProcessorUsageCounters = TimeSpan.FromSeconds(10.0);

		private readonly AgentRecord[] agentRecords;

		private SlidingAverageCounter[] synchronousAgentProcessorUsageSlidingCounters;

		private SlidingAverageCounter[] asynchronousAgentProcessorUsageSlidingCounters;
	}
}
