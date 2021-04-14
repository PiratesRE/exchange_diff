using System;
using System.IO;

namespace Microsoft.Exchange.Protocols.Smtp
{
	internal interface ISmtpInStreamBuilder
	{
		bool IsDiscardingData { get; set; }

		Stream BodyStream { get; set; }

		long TotalBytesRead { get; }

		long TotalBytesWritten { get; }

		long EohPos { get; }

		bool IsEodSeen { get; }

		void Reset();

		bool Write(byte[] data, int offset, int numBytes, out int numBytesConsumed);

		bool Write(CommandContext commandContext, out int numBytesConsumed);
	}
}
