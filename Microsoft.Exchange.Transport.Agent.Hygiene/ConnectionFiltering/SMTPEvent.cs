using System;

namespace Microsoft.Exchange.Transport.Agent.ConnectionFiltering
{
	internal enum SMTPEvent
	{
		Unknown,
		RcptTo,
		EOH
	}
}
