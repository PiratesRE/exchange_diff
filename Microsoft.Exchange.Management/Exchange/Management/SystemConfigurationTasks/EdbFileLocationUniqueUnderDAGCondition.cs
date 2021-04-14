using System;
using System.Collections.Generic;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Serializable]
	internal sealed class EdbFileLocationUniqueUnderDAGCondition : ServerCondition
	{
		public EdbFileLocationUniqueUnderDAGCondition(string edbFileLocation, ADObjectId ownerServerId, ADObjectId[] serversId, IList<Database> databases) : base(ownerServerId, null, databases, null)
		{
			this.edbFileLocation = edbFileLocation;
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
				if (string.Equals((null == database.EdbFilePath) ? string.Empty : database.EdbFilePath.PathName, this.edbFileLocation, StringComparison.OrdinalIgnoreCase) && EdbFileLocationUniqueUnderDAGCondition.HasCommonServer(this.serversOfMyDBCopies, serversToCompare))
				{
					TaskLogger.Trace("The specifed path '{0}' equals to the product Edb file location or copy Edb file location of database '{1}' and the database is in the same DAG", new object[]
					{
						this.edbFileLocation,
						(ADObjectId)database.Identity
					});
					flag = false;
					break;
				}
			}
			TaskLogger.Trace("EdbFileLocationUniqueUnderServerCondition.Verify() returns {0}: <Server '{1}', PathName '{2}'>", new object[]
			{
				flag,
				base.ServerId.ToString(),
				this.edbFileLocation
			});
			return flag;
		}

		internal static bool HasCommonServer(ADObjectId[] servers, ADObjectId[] serversToCompare)
		{
			foreach (ADObjectId adobjectId in servers)
			{
				foreach (ADObjectId id in serversToCompare)
				{
					if (adobjectId.Equals(id))
					{
						return true;
					}
				}
			}
			return false;
		}

		private readonly string edbFileLocation;

		private ADObjectId[] serversOfMyDBCopies;
	}
}
