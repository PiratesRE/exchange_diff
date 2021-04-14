using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.MailboxReplicationService
{
	public interface IJobQuarantineProvider
	{
		void QuarantineJob(Guid requestGuid, Exception ex);

		void UnquarantineJob(Guid requestGuid);

		IDictionary<Guid, FailureRec> GetQuarantinedJobs();
	}
}
