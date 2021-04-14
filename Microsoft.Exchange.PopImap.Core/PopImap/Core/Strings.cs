using System;
using System.Text;

namespace Microsoft.Exchange.PopImap.Core
{
	internal static class Strings
	{
		internal static readonly string CRLF = "\r\n";

		internal static readonly byte[] CRLFByteArray = Encoding.ASCII.GetBytes(Strings.CRLF);

		internal static readonly string CAS = "cas";

		internal static readonly string MBX = "mbx";
	}
}
