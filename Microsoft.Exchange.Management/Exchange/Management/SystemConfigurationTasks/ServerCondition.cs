using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Serializable]
	internal abstract class ServerCondition : ConfigurationSessionCondition
	{
		protected ServerCondition(ADObjectId serverId, IList<StorageGroup> storageGroups, IList<Database> databases, IConfigurationSession session) : base(session)
		{
			this.ServerId = serverId;
			this.StorageGroups = storageGroups;
			this.Databases = databases;
		}

		protected ADObjectId ServerId
		{
			get
			{
				return this.serverId;
			}
			set
			{
				this.serverId = value;
			}
		}

		protected IList<StorageGroup> StorageGroups
		{
			get
			{
				return this.storageGroups;
			}
			set
			{
				this.storageGroups = value;
			}
		}

		protected IList<Database> Databases
		{
			get
			{
				return this.databases;
			}
			set
			{
				this.databases = value;
			}
		}

		private ADObjectId serverId;

		private IList<StorageGroup> storageGroups;

		private IList<Database> databases;
	}
}
