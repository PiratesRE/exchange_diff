using System;

namespace Microsoft.Exchange.EdgeSync
{
	internal delegate bool PreDecorate(ExSearchResultEntry entry, Connection sourceConnection, TargetConnection targetConnection, object state);
}
