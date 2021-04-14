using System;

namespace Microsoft.Exchange.UM.UMCommon
{
	internal interface IMwiTarget : IRpcTarget
	{
		void SendMessageAsync(MwiMessage message);
	}
}
