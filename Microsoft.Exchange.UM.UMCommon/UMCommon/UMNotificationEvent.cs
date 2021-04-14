using System;

namespace Microsoft.Exchange.UM.UMCommon
{
	public enum UMNotificationEvent
	{
		MediaEstablishedStatus,
		MediaEdgeAuthenticationServiceCredentialsAcquisition,
		MediaEdgeResourceAllocation,
		UMPipelineFull,
		CertificateNearExpiry,
		CallRouterCertificateNearExpiry,
		ProtectedVoiceMessageEncryptDecrypt,
		UMGrammarUsage,
		UMTranscriptionThrottling
	}
}
