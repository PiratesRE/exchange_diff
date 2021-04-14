using System;
using Microsoft.Exchange.AnchorService;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.MailboxLoadBalance.Providers;

namespace Microsoft.Exchange.MailboxLoadBalance.Data
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class DirectoryReconnectionVisitor : ILoadEntityVisitor
	{
		public DirectoryReconnectionVisitor(IDirectoryProvider directory, ILogger logger)
		{
			this.directory = directory;
			this.logger = logger;
		}

		public bool Visit(LoadContainer container)
		{
			this.PopulateDirectoryObjectFromIdentity(container);
			return true;
		}

		public bool Visit(LoadEntity entity)
		{
			this.PopulateDirectoryObjectFromIdentity(entity);
			return true;
		}

		private void PopulateDirectoryObjectFromIdentity(LoadEntity entity)
		{
			using (OperationTracker.Create(this.logger, "Re-hydrating directory object of type {0} on load entity.", new object[]
			{
				entity.DirectoryObjectIdentity.ObjectType
			}))
			{
				if (entity.DirectoryObjectIdentity != null)
				{
					try
					{
						entity.DirectoryObject = this.directory.GetDirectoryObject(entity.DirectoryObjectIdentity);
					}
					catch (LocalizedException exception)
					{
						this.logger.LogError(exception, "Failed to rehydrate object with identity '{0}'.", new object[]
						{
							entity.DirectoryObjectIdentity
						});
					}
				}
			}
		}

		private readonly IDirectoryProvider directory;

		private readonly ILogger logger;
	}
}
