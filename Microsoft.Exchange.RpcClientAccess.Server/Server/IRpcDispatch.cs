using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Rpc;

namespace Microsoft.Exchange.RpcClientAccess.Server
{
	internal interface IRpcDispatch : IDisposable
	{
		int MaximumConnections { get; }

		void ReportBytesRead(long bytesRead, long uncompressedBytesRead);

		void ReportBytesWritten(long bytesWritten, long uncompressedBytesWritten);

		int Connect(ProtocolRequestInfo protocolRequestInfo, ClientBinding clientBinding, out IntPtr contextHandle, string userDn, int flags, int connectionModulus, int codePage, int stringLocale, int sortLocale, out TimeSpan pollsMax, out int retryCount, out TimeSpan retryDelay, out string dnPrefix, out string displayName, short[] clientVersion, ArraySegment<byte> auxIn, ArraySegment<byte> auxOut, out int sizeAuxOut, IStandardBudget budget);

		int Disconnect(ProtocolRequestInfo protocolRequestInfo, ref IntPtr contextHandle, bool rundown);

		int Execute(ProtocolRequestInfo protocolRequestInfo, ref IntPtr contextHandle, IList<ArraySegment<byte>> ropInArray, ArraySegment<byte> ropOut, out int sizeRopOut, ArraySegment<byte> auxIn, ArraySegment<byte> auxOut, out int sizeAuxOut, bool isFake, out byte[] fakeOut);

		int NotificationConnect(ProtocolRequestInfo protocolRequestInfo, IntPtr contextHandle, out IntPtr asynchronousContextHandle);

		int Dummy(ProtocolRequestInfo protocolRequestInfo, ClientBinding clientBinding);

		void NotificationWait(ProtocolRequestInfo protocolRequestInfo, IntPtr asynchronousContextHandle, uint flags, Action<bool, int> completion);

		void DroppedConnection(IntPtr asynchronousContextHandle);
	}
}
