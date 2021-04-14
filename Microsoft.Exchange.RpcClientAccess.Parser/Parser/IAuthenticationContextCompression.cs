using System;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal interface IAuthenticationContextCompression
	{
		bool TryDecompress(ArraySegment<byte> source, ArraySegment<byte> destination);
	}
}
