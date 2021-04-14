using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal interface IRopDriver : IDisposable
	{
		void Execute(IList<ArraySegment<byte>> inputBufferArray, ArraySegment<byte> outputBuffer, out int outputSize, AuxiliaryData auxiliaryData, bool isFake, out byte[] fakeOut);

		ServerObjectMap CreateLogon(byte logonIndex, LogonFlags logonFlags);

		bool TryGetServerObjectMap(byte logonIndex, out ServerObjectMap serverObjectMap, out ErrorCode errorCode);

		bool TryGetServerObject(byte logonIndex, ServerObjectHandle handle, out IServerObject serverObject, out ErrorCode errorCode);

		void ReleaseHandle(byte logonIndex, ServerObjectHandle handleToRelease);

		IRopHandler RopHandler { get; }
	}
}
