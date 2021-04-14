using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.ResourceHealth
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class DatabaseInformation : IDatabaseInformation
	{
		public DatabaseInformation(Guid databaseGuid, string databaseName, string databaseVolumeName)
		{
			ArgumentValidator.ThrowIfNullOrEmpty("databaseName", databaseName);
			ArgumentValidator.ThrowIfNullOrEmpty("databaseVolumeName", databaseVolumeName);
			this.DatabaseGuid = databaseGuid;
			this.DatabaseName = databaseName;
			this.DatabaseVolumeName = databaseVolumeName;
		}

		public Guid DatabaseGuid { get; private set; }

		public string DatabaseName { get; private set; }

		public string DatabaseVolumeName { get; private set; }
	}
}
