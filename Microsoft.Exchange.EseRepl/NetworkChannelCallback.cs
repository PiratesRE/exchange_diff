using System;

namespace Microsoft.Exchange.EseRepl
{
	internal delegate void NetworkChannelCallback(object asyncState, int bytesAvailable, bool completedSynchronously, Exception e);
}
