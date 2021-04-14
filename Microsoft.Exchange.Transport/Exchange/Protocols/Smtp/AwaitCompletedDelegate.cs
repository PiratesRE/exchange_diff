using System;
using System.Threading;

namespace Microsoft.Exchange.Protocols.Smtp
{
	internal delegate void AwaitCompletedDelegate(CancellationToken cancellationToken);
}
