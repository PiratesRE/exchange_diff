using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Net.Protocols
{
	internal abstract class ProtocolClient : DisposeTrackableBase
	{
		internal ProtocolClient()
		{
		}

		internal abstract bool TryCancel();
	}
}
