using System;

namespace Microsoft.Exchange.Rpc
{
	public static class RpcProtocolSequence
	{
		public static string ToDisplayString(string protocolSequence)
		{
			if (string.IsNullOrEmpty(protocolSequence))
			{
				return "RPC/UNDEFINED";
			}
			if (protocolSequence.StartsWith("ncacn_http", StringComparison.OrdinalIgnoreCase))
			{
				return "RPC/HTTP";
			}
			if (protocolSequence.StartsWith("ncacn_ip_tcp", StringComparison.OrdinalIgnoreCase))
			{
				return "RPC/TCP";
			}
			if (protocolSequence.StartsWith("ncalrpc", StringComparison.OrdinalIgnoreCase))
			{
				return "RPC/LOCAL";
			}
			if (protocolSequence.StartsWith("xrop", StringComparison.OrdinalIgnoreCase))
			{
				return "RPC/XROP";
			}
			return string.Format("RPC/{0}", protocolSequence);
		}

		public const string Tcp = "ncacn_ip_tcp";

		public const string Http = "ncacn_http";

		public const string Local = "ncalrpc";

		public const string Xrop = "xrop";
	}
}
