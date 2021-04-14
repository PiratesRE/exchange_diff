using System;

namespace Microsoft.Exchange.Monitoring
{
	[Flags]
	public enum FailoverFlags
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
		Random = 4196
	}
}
