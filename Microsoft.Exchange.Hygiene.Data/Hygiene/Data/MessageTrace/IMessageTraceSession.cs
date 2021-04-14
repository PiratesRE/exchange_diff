using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Hygiene.Data.MessageTrace
{
	internal interface IMessageTraceSession
	{
		int GetNumberOfPersistentCopiesPerPartition(int physicalInstanceId);

		int GetNumberOfPhysicalPartitions();

		void Save(MessageTraceBatch messageTraceBatch);

		void Save(IEnumerable<MessageTrafficTypeMapping> messageTrafficTypeMappingBatch, int? persistentStoreCopyId);

		Dictionary<int, bool[]> GetStatusOfAllPhysicalPartitionCopies();

		object GetPartitionId(string hashKey);
	}
}
