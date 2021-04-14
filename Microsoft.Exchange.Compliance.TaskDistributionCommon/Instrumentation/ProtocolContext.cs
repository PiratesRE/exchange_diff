using System;
using Microsoft.Exchange.Compliance.TaskDistributionCommon.Protocol;

namespace Microsoft.Exchange.Compliance.TaskDistributionCommon.Instrumentation
{
	internal class ProtocolContext
	{
		internal ProtocolContext.MessageDirection Direction { get; set; }

		internal DateTime? QueueStartTime { get; set; }

		internal DateTime? QueueEndTime { get; set; }

		internal DateTime? ProcessStartTime { get; set; }

		internal DateTime? ProcessEndTime { get; set; }

		internal DateTime? DispatchStartTime { get; set; }

		internal DateTime? DispatchEndTime { get; set; }

		internal object DispatchData { get; set; }

		internal FaultDefinition FaultDefinition { get; set; }

		public enum MessageDirection
		{
			Unknown,
			Incoming,
			Outgoing,
			Return
		}
	}
}
