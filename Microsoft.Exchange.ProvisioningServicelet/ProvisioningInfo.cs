using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Migration;

namespace Microsoft.Exchange.Servicelets.Provisioning
{
	internal struct ProvisioningInfo
	{
		public ProvisioningInfo(ObjectId itemId, Guid jobId, IProvisioningData data)
		{
			this.ItemId = itemId;
			this.JobId = jobId;
			this.Data = data;
			this.Worker = null;
			this.Canceled = false;
			this.TimesAttempted = 0;
			this.LastAttempted = ExDateTime.Now;
		}

		public readonly ObjectId ItemId;

		public readonly Guid JobId;

		public readonly IProvisioningData Data;

		public ProvisioningWorker Worker;

		public bool Canceled;

		public int TimesAttempted;

		public ExDateTime LastAttempted;
	}
}
