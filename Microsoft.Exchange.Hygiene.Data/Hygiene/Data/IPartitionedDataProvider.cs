using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Hygiene.Data
{
	internal interface IPartitionedDataProvider : IConfigDataProvider
	{
		int GetNumberOfPersistentCopiesPerPartition(int physicalInstanceId);

		int GetNumberOfPhysicalPartitions();

		object[] GetAllPhysicalPartitions();

		Dictionary<int, bool[]> GetStatusOfAllPhysicalPartitionCopies();

		string GetPartitionedDatabaseCopyName(int physicalPartition, int fssCopyId);

		void UpdateLatency(int physicalPartition, int fssCopyId, LatencyBucket bucket);
	}
}
