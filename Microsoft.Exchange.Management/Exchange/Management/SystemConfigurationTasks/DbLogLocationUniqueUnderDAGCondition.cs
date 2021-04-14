using System;
using System.Collections.Generic;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Serializable]
	internal sealed class DbLogLocationUniqueUnderDAGCondition : ServerCondition
	{
		public DbLogLocationUniqueUnderDAGCondition(string logLocation, ADObjectId ownerServerId, ADObjectId[] serversId, IList<Database> databases) : base(ownerServerId, null, databases, null)
		{
			this.logLocation = logLocation;
			this.serversOfMyDBCopies = serversId;
		}

		public override bool Verify()
		{
			bool flag = true;
			foreach (Database database in base.Databases)
			{
				ADObjectId[] serversToCompare;
				if (database.Servers != null && database.Servers.Length != 0)
				{
					serversToCompare = database.Servers;
				}
				else
				{
					serversToCompare = new ADObjectId[]
					{
						database.Server
					};
				}
				if (string.Equals((null == database.LogFolderPath) ? string.Empty : database.LogFolderPath.PathName, this.logLocation, StringComparison.OrdinalIgnoreCase) && EdbFileLocationUniqueUnderDAGCondition.HasCommonServer(this.serversOfMyDBCopies, serversToCompare))
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

		private ADObjectId[] serversOfMyDBCopies;
	}
}
