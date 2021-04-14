using System;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Serializable]
	internal abstract class DatabaseCondition : ConfigurationSessionCondition
	{
		protected DatabaseCondition(Database database, IConfigurationSession session) : base(session)
		{
			this.Database = database;
		}

		protected Database Database
		{
			get
			{
				return this.database;
			}
			set
			{
				this.database = value;
			}
		}

		private Database database;
	}
}
