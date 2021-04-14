using System;
using System.Net;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Transport.Logging
{
	internal interface IProtocolLog
	{
		void Configure(LocalLongFullPath path, TimeSpan ageQuota, Unlimited<ByteQuantifiedSize> sizeQuota, Unlimited<ByteQuantifiedSize> perFileSizeQuota, int bufferSize, TimeSpan streamFlushInterval, TimeSpan backgroundWriteInterval);

		void Configure(string path, TimeSpan ageQuota, long sizeQuota, long perFileSizeQuota, int bufferSize, TimeSpan streamFlushInterval, TimeSpan backgroundWriteInterval);

		IProtocolLogSession OpenSession(string connectorId, ulong sessionId, IPEndPoint remoteEndPoint, IPEndPoint localEndPoint, ProtocolLoggingLevel loggingLevel);

		void Flush();

		void Close();
	}
}
