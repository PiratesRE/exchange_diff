using System;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	public enum CertificateStatus
	{
		[LocDescription(Strings.IDs.CertificateStatusUnknown)]
		Unknown,
		[LocDescription(Strings.IDs.CertificateStatusValid)]
		Valid,
		[LocDescription(Strings.IDs.CertificateStatusRevoked)]
		Revoked,
		[LocDescription(Strings.IDs.CertificateStatusDateInvalid)]
		DateInvalid,
		[LocDescription(Strings.IDs.CertificateStatusUntrusted)]
		Untrusted,
		[LocDescription(Strings.IDs.CertificateStatusInvalid)]
		Invalid,
		[LocDescription(Strings.IDs.CertificateStatusRevocationCheckFailure)]
		RevocationCheckFailure,
		[LocDescription(Strings.IDs.CertificateStatusPendingRequest)]
		PendingRequest
	}
}
