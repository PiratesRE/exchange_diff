using System;

namespace Microsoft.Exchange.UM.UMCore
{
	internal delegate void VoipPlatformEventHandler<TEventArgs>(BaseUMVoipPlatform voipPlatform, TEventArgs e);
}
