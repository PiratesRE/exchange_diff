using System;

namespace Microsoft.Exchange.Transport.Sync.Worker.Health
{
	internal enum RemoteServerHealthState
	{
		Clean,
		BackedOff,
		Poisonous
	}
}
