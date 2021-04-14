using System;

namespace Microsoft.Exchange.Net.ExSmtpClient
{
	internal struct SmtpChunk
	{
		internal byte[] Data;

		internal int Length;
	}
}
