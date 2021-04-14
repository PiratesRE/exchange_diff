using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.Auditing
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IAuditLog
	{
		DateTime EstimatedLogStartTime { get; }

		DateTime EstimatedLogEndTime { get; }

		bool IsAsynchronous { get; }

		IAuditQueryContext<TFilter> CreateAuditQueryContext<TFilter>();

		int WriteAuditRecord(IAuditLogRecord auditRecord);
	}
}
