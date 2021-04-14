using System;
using System.Collections.Generic;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Serializable]
	internal sealed class DbLogLocationUniqueUnderServerCondition : ServerCondition
	{
		public DbLogLocationUniqueUnderServerCondition(string logLocation, ADObjectId serverId, IList<Database> databases) : base(serverId, null, databases, null)
		{
			this.logLocation = logLocation;
		}

		public override bool Verify()
		{
			bool flag = true;
			foreach (Database database in base.Databases)
			{
				if (string.Equals((null == database.LogFolderPath) ? string.Empty : database.LogFolderPath.PathName, this.logLocation, StringComparison.OrdinalIgnoreCase))
				{
					flag = false;
					break;
				}
			}
			TaskLogger.Trace("DbLogLocationUniqueUnderServerCondition.Verify() returns {0}: <Server '{1}', LogLocation '{2}'>", new object[]
			{
				flag,
				base.ServerId.ToString(),
				this.logLocation
			});
			return flag;
		}

		private readonly string logLocation;
	}
}
