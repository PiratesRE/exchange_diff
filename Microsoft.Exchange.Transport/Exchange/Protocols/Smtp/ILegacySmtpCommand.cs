using System;

namespace Microsoft.Exchange.Protocols.Smtp
{
	internal interface ILegacySmtpCommand
	{
		byte[] ProtocolCommand { get; }

		int ProtocolCommandLength { get; }

		int CurrentOffset { get; }
	}
}
