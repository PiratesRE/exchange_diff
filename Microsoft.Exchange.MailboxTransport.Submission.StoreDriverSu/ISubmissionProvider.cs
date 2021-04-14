using System;
using System.Net;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Transport;

namespace Microsoft.Exchange.MailboxTransport.Submission.StoreDriverSubmission
{
	internal interface ISubmissionProvider
	{
		MailSubmissionResult SubmitMessage(string serverDN, Guid mailboxGuid, Guid mdbGuid, string databaseName, long eventCounter, byte[] entryId, byte[] parentEntryId, string serverFqdn, IPAddress networkAddressBytes, DateTime originalCreateTime, bool isPublicFolder, TenantPartitionHint tenantHint, string mailboxHopLatency, QuarantineHandler quarantineHandler, SubmissionPoisonHandler submissionPoisonHandler, LatencyTracker latencyTracker);
	}
}
