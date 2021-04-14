using System;
using Microsoft.Exchange.Data.Transport.Email;

namespace Microsoft.Exchange.Transport.Agent.Common
{
	internal class ScanTimeout
	{
		internal virtual TimeSpan GetTimeout(EmailMessage message = null)
		{
			return TimeSpan.FromMinutes(2.0);
		}
	}
}
