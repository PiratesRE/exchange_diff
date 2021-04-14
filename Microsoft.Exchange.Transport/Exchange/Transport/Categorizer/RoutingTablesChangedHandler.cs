using System;

namespace Microsoft.Exchange.Transport.Categorizer
{
	internal delegate void RoutingTablesChangedHandler(IMailRouter eventSource, DateTime newRoutingTablesTimestamp, bool routesChanged);
}
