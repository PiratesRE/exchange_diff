using System;

namespace Microsoft.Exchange.Compliance.TaskDistributionCommon.Protocol
{
	public abstract class Payload
	{
		public string PayloadId { get; set; }

		public virtual PayloadReference ToPayloadReference()
		{
			return new PayloadReference
			{
				PayloadId = this.PayloadId
			};
		}
	}
}
