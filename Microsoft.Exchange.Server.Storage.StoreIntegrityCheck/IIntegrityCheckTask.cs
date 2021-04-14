using System;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.StoreCommonServices;

namespace Microsoft.Exchange.Server.Storage.StoreIntegrityCheck
{
	public interface IIntegrityCheckTask
	{
		string TaskName { get; }

		IJobExecutionTracker JobExecutionTracker { get; }

		ErrorCode Execute(Context context, Guid mailboxGuid, bool detectOnly, bool isScheduled, Func<bool> shouldContinue);
	}
}
