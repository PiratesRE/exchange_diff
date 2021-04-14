using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.MailboxLoadBalance.Data;

namespace Microsoft.Exchange.MailboxLoadBalanceClient
{
	public class LoadMetricValue
	{
		internal LoadMetricValue(LoadMetric loadMetric, long value)
		{
			this.LoadMetric = loadMetric.FriendlyName;
			this.Value = value;
			if (loadMetric.IsSize)
			{
				this.Size = new ByteQuantifiedSize?(loadMetric.ToByteQuantifiedSize(value));
			}
		}

		public LocalizedString LoadMetric { get; private set; }

		public ByteQuantifiedSize? Size { get; private set; }

		public long Value { get; private set; }

		public override string ToString()
		{
			if (this.Size != null)
			{
				return string.Format("{0}: {1} ({2})", this.LoadMetric, this.Size, this.Value);
			}
			return string.Format("{0}: {1}", this.LoadMetric, this.Value);
		}
	}
}
