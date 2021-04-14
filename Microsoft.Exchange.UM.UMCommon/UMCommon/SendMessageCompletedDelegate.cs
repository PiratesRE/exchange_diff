using System;

namespace Microsoft.Exchange.UM.UMCommon
{
	internal delegate void SendMessageCompletedDelegate(MwiMessage message, MwiDeliveryException error);
}
