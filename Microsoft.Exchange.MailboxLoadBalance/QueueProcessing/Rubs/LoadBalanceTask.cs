using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.MailboxLoadBalance.Logging;
using Microsoft.Exchange.WorkloadManagement;

namespace Microsoft.Exchange.MailboxLoadBalance.QueueProcessing.Rubs
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class LoadBalanceTask : SystemTaskBase
	{
		public LoadBalanceTask(SystemWorkloadBase workload, ResourceReservation reservation, IRequest request) : base(workload, reservation)
		{
			this.request = request;
		}

		protected override TaskStepResult Execute()
		{
			this.request.Process();
			DatabaseRequestLog.Write(this.request);
			return TaskStepResult.Complete;
		}

		private readonly IRequest request;
	}
}
