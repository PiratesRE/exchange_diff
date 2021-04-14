using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Transport.Scheduler.Contracts;

namespace Microsoft.Exchange.Transport.Scheduler.Processing
{
	internal class PriorityDistributedQueue : ISchedulerQueue
	{
		public PriorityDistributedQueue(IDictionary<DeliveryPriority, int> distributions)
		{
			this.priorityDistributions = PriorityDistributedQueue.ValidateAndGetDistributionArray(distributions);
			this.perPrioritySubQueues = new ISchedulerQueue[this.priorityDistributions.Length];
			for (int i = 0; i < this.perPrioritySubQueues.Length; i++)
			{
				this.perPrioritySubQueues[i] = new ConcurrentQueueWrapper();
			}
			this.selectedPriorityIndex = 0;
			this.selectedPriorityItemCount = this.priorityDistributions[this.selectedPriorityIndex];
		}

		public bool IsEmpty
		{
			get
			{
				foreach (ISchedulerQueue schedulerQueue in this.perPrioritySubQueues)
				{
					if (!schedulerQueue.IsEmpty)
					{
						return false;
					}
				}
				return true;
			}
		}

		public long Count
		{
			get
			{
				long num = 0L;
				foreach (ISchedulerQueue schedulerQueue in this.perPrioritySubQueues)
				{
					num += schedulerQueue.Count;
				}
				return num;
			}
		}

		public void Enqueue(ISchedulableMessage message)
		{
			ArgumentValidator.ThrowIfNull("message", message);
			IMessageScope messageScope = null;
			if (message.Scopes != null)
			{
				messageScope = message.Scopes.FirstOrDefault((IMessageScope x) => x.Type == MessageScopeType.Priority);
			}
			if (messageScope == null)
			{
				throw new ArgumentException(string.Format("Message does not have a priority scope associated with it:{0}", message));
			}
			int num = (int)((DeliveryPriority)messageScope.Value);
			this.perPrioritySubQueues[num].Enqueue(message);
		}

		public bool TryDequeue(out ISchedulableMessage message)
		{
			int num = this.selectedPriorityIndex;
			while (!this.perPrioritySubQueues[this.selectedPriorityIndex].TryDequeue(out message))
			{
				this.RolloverToNextPriority();
				if (num == this.selectedPriorityIndex)
				{
					message = null;
					return false;
				}
			}
			this.selectedPriorityItemCount--;
			if (this.selectedPriorityItemCount == 0)
			{
				this.RolloverToNextPriority();
			}
			return true;
		}

		public bool TryPeek(out ISchedulableMessage message)
		{
			int num = this.selectedPriorityIndex;
			while (!this.perPrioritySubQueues[this.selectedPriorityIndex].TryPeek(out message))
			{
				this.RolloverToNextPriority();
				if (num == this.selectedPriorityIndex)
				{
					message = null;
					return false;
				}
			}
			return true;
		}

		private static int[] ValidateAndGetDistributionArray(IDictionary<DeliveryPriority, int> distributions)
		{
			ArgumentValidator.ThrowIfNull("distributions", distributions);
			int priorityTypesLength = Enum.GetNames(typeof(DeliveryPriority)).Count<string>();
			ArgumentValidator.ThrowIfInvalidValue<IDictionary<DeliveryPriority, int>>("distributions", distributions, (IDictionary<DeliveryPriority, int> input) => input.Count == priorityTypesLength);
			foreach (KeyValuePair<DeliveryPriority, int> keyValuePair in distributions)
			{
				if (keyValuePair.Value <= 0)
				{
					throw new ArgumentException(string.Format("Delivery Priority {0} has invalid value of {1} specified. Only positive value allowed", keyValuePair.Key, keyValuePair.Value));
				}
			}
			int[] array = new int[priorityTypesLength];
			foreach (object obj in Enum.GetValues(typeof(DeliveryPriority)))
			{
				DeliveryPriority deliveryPriority = (DeliveryPriority)obj;
				int num;
				if (!distributions.TryGetValue(deliveryPriority, out num))
				{
					throw new ArgumentException(string.Format("Could not find expected entry for Priority {0}. Input Set {1} has unknown values", deliveryPriority, distributions));
				}
				array[(int)deliveryPriority] = num;
			}
			return array;
		}

		private void RolloverToNextPriority()
		{
			this.selectedPriorityIndex = (this.selectedPriorityIndex + 1) % this.perPrioritySubQueues.Length;
			this.selectedPriorityItemCount = this.priorityDistributions[this.selectedPriorityIndex];
		}

		private readonly int[] priorityDistributions;

		private readonly ISchedulerQueue[] perPrioritySubQueues;

		private int selectedPriorityIndex;

		private int selectedPriorityItemCount;
	}
}
