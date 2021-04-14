using System;

namespace Microsoft.Exchange.Net
{
	internal enum DnsRecordType : ushort
	{
		Unknown,
		A,
		NS,
		CNAME = 5,
		SOA,
		PTR = 12,
		MX = 15,
		TXT,
		AAAA = 28,
		SRV = 33
	}
}
