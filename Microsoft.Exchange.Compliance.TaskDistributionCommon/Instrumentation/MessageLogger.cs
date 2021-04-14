using System;
using System.Collections.Generic;
using Microsoft.Exchange.Compliance.TaskDistributionCommon.Diagnostics;
using Microsoft.Exchange.Compliance.TaskDistributionCommon.Extensibility;
using Microsoft.Exchange.Compliance.TaskDistributionCommon.Protocol;
using Microsoft.Office.Datacenter.ActiveMonitoring;

namespace Microsoft.Exchange.Compliance.TaskDistributionCommon.Instrumentation
{
	internal class MessageLogger
	{
		protected MessageLogger()
		{
		}

		public static MessageLogger Instance
		{
			get
			{
				return MessageLogger.instance;
			}
		}

		public void LogMessageReceived(ComplianceMessage message)
		{
			message.ProtocolContext.QueueStartTime = new DateTime?(DateTime.UtcNow);
			IPerformanceCounterAccessor counter = this.GetCounter(message.ComplianceMessageType.ToString());
			if (counter != null)
			{
				counter.AddQueueEvent(QueueEvent.Enqueue);
			}
		}

		public void LogMessageBlockProcessing(ComplianceMessage message, string block)
		{
			if (message.ProtocolContext.QueueEndTime == null)
			{
				this.LogMessageProcessing(message);
			}
			IPerformanceCounterAccessor counter = this.GetCounter(block);
			if (counter != null)
			{
				counter.AddProcessorEvent(ProcessorEvent.StartProcessing);
			}
		}

		public void LogMessageBlockProcessed(ComplianceMessage message, string block, long time)
		{
			IPerformanceCounterAccessor counter = this.GetCounter(block);
			if (counter != null)
			{
				counter.AddProcessorEvent(ProcessorEvent.EndProcessing);
				counter.AddProcessingCompletionEvent(ExceptionHandler.IsFaulted(message) ? ProcessingCompletionEvent.Failure : ProcessingCompletionEvent.Success, time);
			}
		}

		public void LogMessageProcessing(ComplianceMessage message)
		{
			message.ProtocolContext.QueueEndTime = new DateTime?(DateTime.UtcNow);
			message.ProtocolContext.ProcessStartTime = new DateTime?(DateTime.UtcNow);
			IPerformanceCounterAccessor counter = this.GetCounter(message.ComplianceMessageType.ToString());
			if (counter != null)
			{
				long latency = 0L;
				if (message.ProtocolContext.QueueStartTime != null && message.ProtocolContext.QueueEndTime != null)
				{
					latency = (long)(message.ProtocolContext.QueueEndTime.Value - message.ProtocolContext.QueueStartTime.Value).TotalMilliseconds;
				}
				counter.AddDequeueLatency(latency);
				counter.AddQueueEvent(QueueEvent.Dequeue);
				counter.AddProcessorEvent(ProcessorEvent.StartProcessing);
			}
		}

		public void LogMessageDispatching(ComplianceMessage message)
		{
			message.ProtocolContext.DispatchStartTime = new DateTime?(DateTime.UtcNow);
		}

		public void LogMessageProcessed(ComplianceMessage message)
		{
			message.ProtocolContext.ProcessEndTime = new DateTime?(DateTime.UtcNow);
			IPerformanceCounterAccessor counter = this.GetCounter(message.ComplianceMessageType.ToString());
			if (counter != null)
			{
				long latency = 0L;
				if (message.ProtocolContext.ProcessStartTime != null && message.ProtocolContext.ProcessEndTime != null)
				{
					latency = (long)(message.ProtocolContext.ProcessEndTime.Value - message.ProtocolContext.ProcessStartTime.Value).TotalMilliseconds;
				}
				counter.AddProcessorEvent(ProcessorEvent.EndProcessing);
				counter.AddProcessingCompletionEvent(ExceptionHandler.IsFaulted(message) ? ProcessingCompletionEvent.Failure : ProcessingCompletionEvent.Success, latency);
			}
			if (message.ProtocolContext.Direction == ProtocolContext.MessageDirection.Return)
			{
				ProtocolContext.MessageDirection direction = message.ProtocolContext.Direction;
			}
		}

		public void LogMessageDispatched(ComplianceMessage message)
		{
			message.ProtocolContext.DispatchEndTime = new DateTime?(DateTime.UtcNow);
			ProtocolContext.MessageDirection direction = message.ProtocolContext.Direction;
		}

		public void LogMessageFaulted(ComplianceMessage message, FaultDefinition fault)
		{
			foreach (FaultRecord faultRecord in fault.Faults)
			{
				EventNotificationItem eventNotificationItem = new EventNotificationItem("TaskDistributionFabric", "TaskDistributionFabric", "Exception", ResultSeverityLevel.Error);
				eventNotificationItem.CustomProperties = new Dictionary<string, string>();
				foreach (KeyValuePair<string, string> keyValuePair in faultRecord.Data)
				{
					if (!string.IsNullOrEmpty(keyValuePair.Key) && !string.IsNullOrEmpty(keyValuePair.Value))
					{
						eventNotificationItem.CustomProperties[keyValuePair.Key] = keyValuePair.Value;
					}
				}
				eventNotificationItem.Publish(false);
			}
		}

		private IPerformanceCounterAccessor GetCounter(string type)
		{
			IPerformanceCounterAccessorRegistry performanceCounterAccessorRegistry;
			FaultDefinition faultDefinition;
			if (Registry.Instance.TryGetInstance<IPerformanceCounterAccessorRegistry>(RegistryComponent.Common, CommonComponent.PerformanceCounterRegistry, out performanceCounterAccessorRegistry, out faultDefinition, "GetCounter", "f:\\15.00.1497\\sources\\dev\\EDiscovery\\src\\TaskDistributionSystem\\TaskDistributionCommon\\Instrumentation\\MessageLogger.cs", 207))
			{
				return performanceCounterAccessorRegistry.GetOrAddPerformanceCounterAccessor(type);
			}
			return null;
		}

		private static MessageLogger instance = new MessageLogger();
	}
}
