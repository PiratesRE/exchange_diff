using System;

namespace Microsoft.Exchange.Transport.Agent.InterceptorAgent
{
	[Flags]
	public enum InterceptorAgentEvent : ushort
	{
		Invalid = 0,
		OnMailFrom = 1,
		OnRcptTo = 2,
		OnEndOfHeaders = 4,
		OnEndOfData = 8,
		OnSubmittedMessage = 16,
		OnResolvedMessage = 32,
		OnRoutedMessage = 64,
		OnCategorizedMessage = 128,
		OnInitMsg = 256,
		OnPromotedMessage = 512,
		OnCreatedMessage = 1024,
		OnDemotedMessage = 8192,
		OnLoadedMessage = 16384
	}
}
