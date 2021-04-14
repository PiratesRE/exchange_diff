using System;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.PropTags;
using Microsoft.Exchange.Server.Storage.StoreCommonServices;

namespace Microsoft.Exchange.Server.Storage.StoreIntegrityCheck
{
	public interface IIntegrityCheckJob
	{
		Guid JobGuid { get; }

		Guid RequestGuid { get; }

		Guid MailboxGuid { get; }

		int MailboxNumber { get; }

		TaskId TaskId { get; }

		bool DetectOnly { get; }

		DateTime CreationTime { get; }

		JobSource Source { get; }

		JobPriority Priority { get; }

		JobState State { get; }

		short Progress { get; }

		TimeSpan TimeInServer { get; }

		DateTime? CompletedTime { get; }

		DateTime? LastExecutionTime { get; }

		int CorruptionsDetected { get; }

		int CorruptionsFixed { get; }

		ErrorCode Error { get; }

		Properties GetProperties(StorePropTag[] propTags);
	}
}
