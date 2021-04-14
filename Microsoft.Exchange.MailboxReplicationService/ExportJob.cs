using System;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal class ExportJob : MergeJob
	{
		public override void Initialize(TransactionalRequestJob exportRequest)
		{
			base.Initialize(exportRequest);
			base.RequestJobIdentity = exportRequest.Identity.ToString();
			Guid sourceExchangeGuid = exportRequest.SourceExchangeGuid;
			string orgID = (exportRequest.OrganizationId != null && exportRequest.OrganizationId.OrganizationalUnit != null) ? (exportRequest.OrganizationId.OrganizationalUnit.Name + "\\") : string.Empty;
			LocalizedString sourceTracingID = exportRequest.SourceIsArchive ? MrsStrings.ArchiveMailboxTracingId(orgID, sourceExchangeGuid) : MrsStrings.PrimaryMailboxTracingId(orgID, sourceExchangeGuid);
			LocalizedString targetTracingID = MrsStrings.PstTracingId(exportRequest.FilePath);
			MailboxCopierFlags flags = MailboxCopierFlags.Merge | MailboxCopierFlags.TargetIsPST;
			base.MailboxMerger = new MailboxMerger(sourceExchangeGuid, Guid.Empty, exportRequest, this, flags, sourceTracingID, targetTracingID);
		}
	}
}
