using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Data.Transport.Partner
{
	internal abstract class ExtendedRoutingSmtpServer : SmtpServer
	{
		public abstract void TrackAgentInfo(string agentName, string groupName, List<KeyValuePair<string, string>> data);
	}
}
