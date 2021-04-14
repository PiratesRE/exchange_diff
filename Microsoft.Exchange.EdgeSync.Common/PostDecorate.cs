using System;

namespace Microsoft.Exchange.EdgeSync
{
	internal delegate void PostDecorate(ExSearchResultEntry entry, Connection sourceConnection, TargetConnection targetConnection, object state);
}
