using System;
using System.Text;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.WorkloadManagement
{
	internal sealed class SystemWorkloadManagerEvent
	{
		public SystemWorkloadManagerEvent(ResourceLoad load, int slots, bool delayed)
		{
			this.DateTime = TimeProvider.UtcNow;
			this.Load = load;
			this.Slots = slots;
			this.Delayed = delayed;
		}

		public DateTime DateTime { get; private set; }

		public ResourceLoad Load { get; private set; }

		public int Slots { get; private set; }

		public bool Delayed { get; private set; }

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat("DateTime={0},Load={1}", this.DateTime, this.Load);
			if (this.Slots >= 0)
			{
				stringBuilder.AppendFormat(",Slots={0}", this.Slots);
			}
			if (this.Delayed)
			{
				stringBuilder.AppendFormat(",Delayed={0}", this.Delayed);
			}
			return stringBuilder.ToString();
		}
	}
}
