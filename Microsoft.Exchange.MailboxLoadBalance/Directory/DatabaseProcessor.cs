using System;
using Microsoft.Exchange.AnchorService;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.MailboxLoadBalance.Config;
using Microsoft.Exchange.MailboxLoadBalance.Drain;

namespace Microsoft.Exchange.MailboxLoadBalance.Directory
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class DatabaseProcessor
	{
		public DatabaseProcessor(ILoadBalanceSettings settings, DatabaseDrainControl drainControl, ILogger logger, DirectoryDatabase database)
		{
			this.settings = settings;
			this.drainControl = drainControl;
			this.logger = logger;
			this.database = database;
		}

		public void ProcessDatabase()
		{
			if (!this.settings.AutomaticDatabaseDrainEnabled)
			{
				return;
			}
			if (this.database == null)
			{
				return;
			}
			ByteQuantifiedSize currentPhysicalSize = this.database.GetSize().CurrentPhysicalSize;
			double num = 1.0 + (double)this.settings.AutomaticDrainStartFileSizePercent / 100.0;
			ulong bytesValue = (ulong)(this.database.MaximumSize.ToBytes() * num);
			if (currentPhysicalSize >= ByteQuantifiedSize.FromBytes(bytesValue))
			{
				this.logger.LogInformation("Database {0} has {1} EDB file size, and the maximum allowed size is {2} with a {3}% tolerance. Starting drain process.", new object[]
				{
					this.database.Identity,
					currentPhysicalSize,
					this.database.MaximumSize,
					this.settings.AutomaticDrainStartFileSizePercent
				});
				this.drainControl.BeginDrainDatabase(this.database);
			}
		}

		private readonly DirectoryDatabase database;

		private readonly DatabaseDrainControl drainControl;

		private readonly ILogger logger;

		private readonly ILoadBalanceSettings settings;
	}
}
