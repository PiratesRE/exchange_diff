using System;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal class ImportJob : MergeJob
	{
		public override void Initialize(TransactionalRequestJob importRequest)
		{
			base.Initialize(importRequest);
			base.RequestJobIdentity = importRequest.Identity.ToString();
			Guid targetExchangeGuid = importRequest.TargetExchangeGuid;
			string orgID = (importRequest.OrganizationId != null && importRequest.OrganizationId.OrganizationalUnit != null) ? (importRequest.OrganizationId.OrganizationalUnit.Name + "\\") : string.Empty;
			LocalizedString sourceTracingID = MrsStrings.PstTracingId(importRequest.FilePath);
			LocalizedString targetTracingID = importRequest.TargetIsArchive ? MrsStrings.ArchiveMailboxTracingId(orgID, targetExchangeGuid) : MrsStrings.PrimaryMailboxTracingId(orgID, targetExchangeGuid);
			MailboxCopierFlags flags = MailboxCopierFlags.Merge | MailboxCopierFlags.SourceIsPST;
			base.MailboxMerger = new MailboxMerger(Guid.Empty, targetExchangeGuid, importRequest, this, flags, sourceTracingID, targetTracingID);
		}
	}
}
