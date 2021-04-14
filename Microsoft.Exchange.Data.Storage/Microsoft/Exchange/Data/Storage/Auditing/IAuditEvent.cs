using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.Auditing
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IAuditEvent
	{
		Guid RecordId { get; }

		string OrganizationId { get; }

		Guid MailboxGuid { get; }

		string OperationName { get; }

		string LogonTypeName { get; }

		OperationResult OperationSucceeded { get; }

		bool ExternalAccess { get; }

		IAuditLogRecord GetLogRecord();
	}
}
