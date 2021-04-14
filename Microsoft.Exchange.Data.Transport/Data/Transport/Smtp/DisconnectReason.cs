using System;

namespace Microsoft.Exchange.Data.Transport.Smtp
{
	public enum DisconnectReason
	{
		QuitVerb,
		Timeout,
		SenderDisconnected,
		TooManyErrors,
		DroppedSession,
		Remote,
		Local,
		SuppressLogging,
		None
	}
}
