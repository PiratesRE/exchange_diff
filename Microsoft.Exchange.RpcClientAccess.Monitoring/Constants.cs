using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Net.Protocols;

namespace Microsoft.Exchange.RpcClientAccess.Monitoring
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal static class Constants
	{
		public const string HttpProxyInfoHeaderName = "Via";

		public const string RpcProxyPath = "rpc/rpcproxy.dll";

		public const int EmsmdbEndpoint = 6001;

		public const int RfriEndpoint = 6002;

		public const int NspiEndpoint = 6004;

		public const int ConsolidatedEndpoint = 6001;

		public const int GuidSize = 16;

		public static readonly string DiagInfoHeaderName = WellKnownHeader.E14DiagInfo;

		public static readonly string ClientAccessServerHeaderName = WellKnownHeader.XFEServer;

		public static readonly string BackendServerHeaderName = WellKnownHeader.XCalculatedBETarget;

		public static readonly TimeSpan DefaultRpcTimeout = TimeSpan.FromSeconds(58.0);
	}
}
