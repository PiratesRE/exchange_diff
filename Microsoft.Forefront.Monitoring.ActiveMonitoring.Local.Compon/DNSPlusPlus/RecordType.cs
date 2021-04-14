using System;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring.DNSPlusPlus
{
	public enum RecordType
	{
		A = 1,
		NS,
		CNAME = 5,
		SOA,
		WKS = 11,
		PTR,
		HINFO,
		MX = 15,
		TXT,
		AAAA = 28,
		SRV = 33,
		ANY = 255
	}
}
