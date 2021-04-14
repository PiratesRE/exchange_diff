using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Cluster.Replay.DatabaseCopyLayout
{
	public struct DatabaseGroupLayoutEntry
	{
		public string DatabaseGroupId { get; private set; }

		public List<string> DatabaseNameList { get; private set; }

		public DatabaseGroupLayoutEntry(string databaseGroupId, List<string> databaseNameList, bool additionalCopyOnSpare = false)
		{
			this = default(DatabaseGroupLayoutEntry);
			this.DatabaseNameList = databaseNameList;
			this.DatabaseGroupId = databaseGroupId;
			this.AdditionalCopyOnSpare = additionalCopyOnSpare;
		}

		public bool AdditionalCopyOnSpare;
	}
}
