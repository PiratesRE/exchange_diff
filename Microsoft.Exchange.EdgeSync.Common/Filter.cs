using System;

namespace Microsoft.Exchange.EdgeSync
{
	internal delegate FilterResult Filter(ExSearchResultEntry entry, Connection sourceConnection, TargetConnection targetConnection);
}
