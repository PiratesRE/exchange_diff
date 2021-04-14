using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.Auditing
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IAuditLogRecord
	{
		AuditLogRecordType RecordType { get; }

		DateTime CreationTime { get; }

		string Operation { get; }

		string ObjectId { get; }

		string UserId { get; }

		IEnumerable<KeyValuePair<string, string>> GetDetails();
	}
}
