using System;

namespace Microsoft.Exchange.UM.UMCore
{
	internal delegate void UMCallSessionHandler<TEventArgs>(BaseUMCallSession umCallSession, TEventArgs e);
}
