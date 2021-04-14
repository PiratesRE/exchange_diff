using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.Auditing
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IAuditLogCollection
	{
		IEnumerable<IAuditLog> GetAuditLogs();

		bool FindLog(DateTime timestamp, bool createIfNotExists, out IAuditLog auditLog);
	}
}
