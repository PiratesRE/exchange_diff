using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Server.Storage.StoreIntegrityCheck
{
	public interface IJobStorage
	{
		void AddJob(IntegrityCheckJob job);

		void RemoveJob(Guid jobGuid);

		IntegrityCheckJob GetJob(Guid jobGuid);

		IEnumerable<IntegrityCheckJob> GetJobsByRequestGuid(Guid requestGuid);

		IEnumerable<IntegrityCheckJob> GetJobsByMailboxGuid(Guid mailboxGuid);

		IEnumerable<IntegrityCheckJob> GetAllJobs();
	}
}
