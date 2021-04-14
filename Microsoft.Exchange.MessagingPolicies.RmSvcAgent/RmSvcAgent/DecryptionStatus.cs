using System;

namespace Microsoft.Exchange.MessagingPolicies.RmSvcAgent
{
	internal enum DecryptionStatus
	{
		StartAsync,
		Success,
		PermanentFailure,
		TransientFailure,
		ConfigurationLoadFailure
	}
}
