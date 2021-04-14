using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Exchange.Compliance.TaskDistributionCommon.Protocol;
using Microsoft.Exchange.Compliance.TaskDistributionCommon.Serialization;

namespace Microsoft.Exchange.Compliance.TaskDistributionCommon.Client
{
	public abstract class WorkloadClientBase : IMessageSender
	{
		public virtual async Task<bool[]> SendMessageAsync(IEnumerable<ComplianceMessage> messages)
		{
			IEnumerable<ComplianceMessage> responses = await this.SendMessageAsyncInternal(messages);
			foreach (ComplianceMessage complianceMessage in responses)
			{
				FaultDefinition faultDefinition;
				StatusPayload status;
				if (ComplianceSerializer.TryDeserialize<StatusPayload>(StatusPayload.Description, complianceMessage.Payload, out status, out faultDefinition, "SendMessageAsync", "f:\\15.00.1497\\sources\\dev\\EDiscovery\\src\\TaskDistributionSystem\\TaskDistributionCommon\\Client\\WorkloadClientBase.cs", 45))
				{
					return (from t in messages
					select status.QueuedMessages.Contains(t.MessageId)).ToArray<bool>();
				}
			}
			return new bool[1];
		}

		protected abstract Task<IEnumerable<ComplianceMessage>> SendMessageAsyncInternal(IEnumerable<ComplianceMessage> messages);
	}
}
