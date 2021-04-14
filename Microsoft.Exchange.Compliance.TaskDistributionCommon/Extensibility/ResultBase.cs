using System;
using System.Collections.Concurrent;
using Microsoft.Exchange.Compliance.TaskDistributionCommon.Protocol;

namespace Microsoft.Exchange.Compliance.TaskDistributionCommon.Extensibility
{
	public abstract class ResultBase : WorkDefinition
	{
		public IProducerConsumerCollection<FaultRecord> Faults
		{
			get
			{
				return this.faults;
			}
		}

		public abstract int SerializationVersion { get; }

		public abstract byte[] GetSerializedResult();

		public virtual void MergeFaults(ResultBase source)
		{
			if (source != null)
			{
				FaultRecord item;
				while (source.Faults.TryTake(out item))
				{
					this.Faults.TryAdd(item);
				}
			}
		}

		private ConcurrentBag<FaultRecord> faults = new ConcurrentBag<FaultRecord>();
	}
}
