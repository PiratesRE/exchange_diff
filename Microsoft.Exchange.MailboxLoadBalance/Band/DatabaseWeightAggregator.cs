using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.MailboxLoadBalance.Data;

namespace Microsoft.Exchange.MailboxLoadBalance.Band
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class DatabaseWeightAggregator : ILoadEntityVisitor
	{
		public int TotalWeight { get; private set; }

		public bool Visit(LoadContainer container)
		{
			if (container.ContainerType != ContainerType.Database)
			{
				return container.CanAcceptRegularLoad;
			}
			if (!container.CanAcceptRegularLoad)
			{
				return false;
			}
			this.TotalWeight += container.RelativeLoadWeight;
			return false;
		}

		public bool Visit(LoadEntity entity)
		{
			return false;
		}
	}
}
