using System;
using System.Collections.Generic;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Serializable]
	internal sealed class EdbFileLocationUniqueUnderServerCondition : ServerCondition
	{
		public EdbFileLocationUniqueUnderServerCondition(string edbFileLocation, ADObjectId serverId, IList<Database> databases) : base(serverId, null, databases, null)
		{
			this.edbFileLocation = edbFileLocation;
		}

		public override bool Verify()
		{
			bool flag = true;
			foreach (Database database in base.Databases)
			{
				if (string.Equals((null == database.EdbFilePath) ? string.Empty : database.EdbFilePath.PathName, this.edbFileLocation, StringComparison.OrdinalIgnoreCase))
				{
					TaskLogger.Trace("The specifed path '{0}' equals to the product Edb file location or copy Edb file location of database '{1}'", new object[]
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

		private readonly string edbFileLocation;
	}
}
