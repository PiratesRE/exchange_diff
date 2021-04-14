using System;
using System.Net;
using Microsoft.Exchange.Data.Directory.Recipient;

namespace Microsoft.Exchange.Monitoring
{
	internal struct UserWithCredential
	{
		public ADUser User { get; set; }

		public NetworkCredential Credential { get; set; }
	}
}
