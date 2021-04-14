using System;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.ActiveManager
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IDumpsterReplicationStatus
	{
		Guid? GetDatabaseGuidFromDN(string distinguishedName);

		ConstraintCheckResultType CheckReplicationHealthConstraint(Guid databaseGuid, out TimeSpan waitTime, out LocalizedString failureReason, out ConstraintCheckAgent agent);

		ConstraintCheckResultType CheckReplicationFlushed(Guid databaseGuid, DateTime utcCommitTime, out LocalizedString failureReason);
	}
}
