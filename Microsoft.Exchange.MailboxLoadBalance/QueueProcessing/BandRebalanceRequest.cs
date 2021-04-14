using System;
using Microsoft.Exchange.AnchorService;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.MailboxLoadBalance.Band;
using Microsoft.Exchange.MailboxLoadBalance.Data.LoadMetrics;
using Microsoft.Exchange.MailboxLoadBalance.Directory;
using Microsoft.Exchange.MailboxLoadBalance.LoadBalance;
using Microsoft.Exchange.MailboxLoadBalance.Logging;
using Microsoft.Exchange.MailboxLoadBalance.ServiceSupport;

namespace Microsoft.Exchange.MailboxLoadBalance.QueueProcessing
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class BandRebalanceRequest : BaseRequest
	{
		public BandRebalanceRequest(BandMailboxRebalanceData rebalanceRequest, IClientFactory clientFactory, ILogger logger)
		{
			this.rebalanceRequest = rebalanceRequest;
			this.clientFactory = clientFactory;
			this.logger = logger;
		}

		protected override void ProcessRequest()
		{
			this.logger.LogInformation("Starting load balancing moves for request {0}", new object[]
			{
				this.rebalanceRequest
			});
			try
			{
				BandRebalanceLog.Write(this.rebalanceRequest);
				IOperationRetryManager operationRetryManager = LoadBalanceOperationRetryManager.Create(this.logger);
				operationRetryManager.Run(new Action(this.RequestRebalancingOnRemoteServer));
			}
			finally
			{
				this.logger.LogInformation("Done creating rebalancing request.", new object[0]);
			}
		}

		private void RequestRebalancingOnRemoteServer()
		{
			DirectoryDatabase database = (DirectoryDatabase)this.rebalanceRequest.SourceDatabase.DirectoryObject;
			using (ILoadBalanceService loadBalanceClientForDatabase = this.clientFactory.GetLoadBalanceClientForDatabase(database))
			{
				loadBalanceClientForDatabase.BeginMailboxMove(this.rebalanceRequest, PhysicalSize.Instance);
			}
		}

		private readonly BandMailboxRebalanceData rebalanceRequest;

		private readonly IClientFactory clientFactory;

		private readonly ILogger logger;
	}
}
