using System;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal class RestoreJob : MergeJob
	{
		public override void Initialize(TransactionalRequestJob restoreRequest)
		{
			base.Initialize(restoreRequest);
			base.RequestJobIdentity = restoreRequest.Identity.ToString();
			Guid targetExchangeGuid = restoreRequest.TargetExchangeGuid;
			Guid sourceExchangeGuid = restoreRequest.SourceExchangeGuid;
			string orgID = (restoreRequest.OrganizationId != null && restoreRequest.OrganizationId.OrganizationalUnit != null) ? (restoreRequest.OrganizationId.OrganizationalUnit.Name + "\\") : string.Empty;
			string dbName = (restoreRequest.SourceDatabase != null) ? restoreRequest.SourceDatabase.Name : ((restoreRequest.RemoteDatabaseGuid != null) ? restoreRequest.RemoteDatabaseGuid.Value.ToString() : string.Empty);
			LocalizedString sourceTracingID = MrsStrings.RestoreMailboxTracingId(dbName, restoreRequest.SourceExchangeGuid);
			LocalizedString targetTracingID = restoreRequest.TargetIsArchive ? MrsStrings.ArchiveMailboxTracingId(orgID, targetExchangeGuid) : MrsStrings.PrimaryMailboxTracingId(orgID, targetExchangeGuid);
			MailboxCopierFlags flags = MailboxCopierFlags.Merge;
			base.MailboxMerger = new MailboxMerger(sourceExchangeGuid, targetExchangeGuid, restoreRequest, this, flags, sourceTracingID, targetTracingID);
		}
	}
}
