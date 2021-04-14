using System;
using Microsoft.Exchange.Rpc;
using Microsoft.Exchange.RpcClientAccess.Parser;

namespace Microsoft.Exchange.RpcClientAccess.Server
{
	internal sealed class AuthContextDecompressor : IAuthenticationContextCompression
	{
		public bool TryDecompress(ArraySegment<byte> source, ArraySegment<byte> destination)
		{
			return CompressAndObfuscate.Instance.TryExpand(source, destination);
		}
	}
}
