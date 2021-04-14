using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal delegate void SendClientWatsonReportAction(WatsonClientReport report, string extraData);
}
