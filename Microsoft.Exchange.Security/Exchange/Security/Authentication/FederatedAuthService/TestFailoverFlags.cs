using System;

namespace Microsoft.Exchange.Security.Authentication.FederatedAuthService
{
	[Flags]
	internal enum TestFailoverFlags
	{
		None = 0,
		HRDRequest = 1,
		HRDResponse = 2,
		LiveIdRequest = 4,
		LiveIdResponse = 8,
		OrgIdRequest = 16,
		OrgIdResponse = 32,
		OfflineHRD = 64,
		OfflineAuthentication = 128,
		HRDRequestTimeout = 256,
		LiveIdRequestTimeout = 512,
		LowPasswordConfidence = 1024,
		OrgIdRequestTimeout = 2048,
		FederatedRequestTimeout = 4096,
		FederatedRequest = 8192
	}
}
