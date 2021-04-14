using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.MailboxLoadBalance.Data;

namespace Microsoft.Exchange.MailboxLoadBalance.Provisioning
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class DatabaseCollector : ILoadEntityVisitor
	{
		public IEnumerable<LoadContainer> Databases
		{
			get
			{
				return this.databases;
			}
		}

		public bool Visit(LoadContainer container)
		{
			if (container.ContainerType == ContainerType.Database)
			{
				this.databases.Add(container);
			}
			return true;
		}

		public bool Visit(LoadEntity entity)
		{
			return false;
		}

		private readonly List<LoadContainer> databases = new List<LoadContainer>(100);
	}
}
