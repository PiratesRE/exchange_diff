using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.MailboxReplicationService;

namespace Microsoft.Exchange.MailboxLoadBalance
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class LoadBalancerVersionInformation
	{
		static LoadBalancerVersionInformation()
		{
			LoadBalancerVersionInformation.LoadBalancerVersion[0] = true;
			LoadBalancerVersionInformation.LoadBalancerVersion[1] = true;
			LoadBalancerVersionInformation.LoadBalancerVersion[2] = true;
			LoadBalancerVersionInformation.LoadBalancerVersion[3] = true;
			LoadBalancerVersionInformation.LoadBalancerVersion[4] = true;
			LoadBalancerVersionInformation.LoadBalancerVersion[5] = true;
			LoadBalancerVersionInformation.InjectorVersion = new VersionInformation(4);
			LoadBalancerVersionInformation.InjectorVersion[0] = true;
			LoadBalancerVersionInformation.InjectorVersion[1] = true;
			LoadBalancerVersionInformation.InjectorVersion[2] = true;
			LoadBalancerVersionInformation.InjectorVersion[3] = true;
		}

		public static readonly VersionInformation LoadBalancerVersion = new VersionInformation(6);

		public static readonly VersionInformation InjectorVersion;
	}
}
