using System;
using Microsoft.Exchange.Data.QueueViewer;

namespace Microsoft.Exchange.Transport.ShadowRedundancy
{
	internal delegate QueueStatus GetRoutedMessageQueueStatus(NextHopSolutionKey key);
}
