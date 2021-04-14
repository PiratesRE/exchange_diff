using System;

namespace Microsoft.Exchange.Transport.MessageThrottling
{
	internal interface IMessageThrottlingManagerConfig
	{
		bool Enabled { get; }
	}
}
