using System;

namespace Microsoft.Exchange.Transport.MessageThrottling
{
	internal sealed class MessageThrottlingManagerConfig : IMessageThrottlingManagerConfig
	{
		public bool Enabled
		{
			get
			{
				return Components.TransportAppConfig.MessageThrottlingConfig.Enabled;
			}
		}
	}
}
