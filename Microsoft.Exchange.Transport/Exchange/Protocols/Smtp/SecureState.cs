using System;

namespace Microsoft.Exchange.Protocols.Smtp
{
	[Flags]
	internal enum SecureState : byte
	{
		None = 0,
		StartTls = 1,
		AnonymousTls = 2,
		NegotiationRequested = 128
	}
}
