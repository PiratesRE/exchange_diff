using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Serializable]
	internal abstract class StorageGroupCondition : ConfigurationSessionCondition
	{
		protected StorageGroupCondition(ADObjectId storageGroupId, Database[] databases, IConfigurationSession session) : base(session)
		{
			this.StorageGroupId = storageGroupId;
			this.Databases = databases;
		}

		protected ADObjectId StorageGroupId
		{
			get
			{
				return this.storageGroupId;
			}
			set
			{
				this.storageGroupId = value;
			}
		}

		protected Database[] Databases
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

		private ADObjectId storageGroupId;

		private Database[] databases;
	}
}
