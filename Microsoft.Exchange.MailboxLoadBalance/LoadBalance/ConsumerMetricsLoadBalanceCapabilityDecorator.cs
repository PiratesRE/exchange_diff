using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.MailboxLoadBalance.Band;
using Microsoft.Exchange.MailboxLoadBalance.Data;
using Microsoft.Exchange.MailboxLoadBalance.Directory;

namespace Microsoft.Exchange.MailboxLoadBalance.LoadBalance
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class ConsumerMetricsLoadBalanceCapabilityDecorator : MissingCapabilityLoadBalanceClientDecorator
	{
		public ConsumerMetricsLoadBalanceCapabilityDecorator(ILoadBalanceService service, DirectoryServer targetServer) : base(service, targetServer)
		{
		}

		public override void BeginMailboxMove(BandMailboxRebalanceData rebalanceData, LoadMetric metric)
		{
			base.BeginMailboxMove(rebalanceData.ToSerializationFormat(true), new LoadMetric(metric.Name, metric.IsSize));
		}

		public override DatabaseSizeInfo GetDatabaseSizeInformation(DirectoryIdentity database)
		{
			DirectoryDatabase database2 = (DirectoryDatabase)base.TargetServer.Directory.GetDirectoryObject(database);
			return base.GetDatabaseSizeInformation(database2);
		}

		public override LoadContainer GetLocalServerData(Band[] bands)
		{
			LoadContainer localServerData = base.GetLocalServerData(bands);
			localServerData.ConvertFromSerializationFormat();
			return localServerData;
		}
	}
}
