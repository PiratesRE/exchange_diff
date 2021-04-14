using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Cluster.Replay.DatabaseCopyLayout
{
	public class DatabaseCopyLayoutGenerator
	{
		public DatabaseCopyLayoutGenerator(int serversPerDag, int databasesPerDag, int databaseDrivesPerServer, int databasesPerDrive, int copiesPerDatabase, int numberOfExtraCopiesOnSpares = 0, string dbNamePrefix = "", string dbNumberFormatSpecifier = "D")
		{
			this.m_serversPerDag = serversPerDag;
			this.m_databasesPerDag = databasesPerDag;
			this.m_databaseDrivesPerServer = databaseDrivesPerServer;
			this.m_databasesPerDrive = databasesPerDrive;
			this.m_copiesPerDatabase = copiesPerDatabase;
			this.m_numberOfExtraCopiesOnSpares = numberOfExtraCopiesOnSpares;
			this.m_copyLayoutEntry = new DatabaseCopyLayoutEntry(dbNamePrefix, dbNumberFormatSpecifier);
		}

		public Dictionary<int, List<DatabaseGroupLayoutEntry>> GenerateDatabaseCopyLayoutForDag()
		{
			return CopyLayoutGenerator.GenerateCopyLayoutTable(this.m_copyLayoutEntry, this.m_serversPerDag, this.m_databasesPerDag, this.m_databaseDrivesPerServer, this.m_databasesPerDrive, this.m_copiesPerDatabase, this.m_numberOfExtraCopiesOnSpares);
		}

		public List<DatabaseGroupLayoutEntry> GenerateDatabaseCopyLayoutForServer(int serverIndex)
		{
			Dictionary<int, List<DatabaseGroupLayoutEntry>> dictionary = this.GenerateDatabaseCopyLayoutForDag();
			if (dictionary != null)
			{
				return dictionary[serverIndex];
			}
			return null;
		}

		private readonly int m_serversPerDag;

		private readonly int m_databasesPerDag;

		private readonly int m_databaseDrivesPerServer;

		private readonly int m_databasesPerDrive;

		private readonly int m_copiesPerDatabase;

		private readonly int m_numberOfExtraCopiesOnSpares;

		private DatabaseCopyLayoutEntry m_copyLayoutEntry;
	}
}
