using System;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring.DNSPlusPlus
{
	public enum QueryResponseCode
	{
		Success,
		FormatError,
		ServerFailure,
		NameError,
		NotImplemented,
		Refused,
		Other
	}
}
