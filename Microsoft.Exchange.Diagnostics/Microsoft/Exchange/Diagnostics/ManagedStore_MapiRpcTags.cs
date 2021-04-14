using System;

namespace Microsoft.Exchange.Diagnostics
{
	public struct ManagedStore_MapiRpcTags
	{
		public const int General = 0;

		public const int RpcOperation = 1;

		public const int FaultInjection = 20;

		public static Guid guid = new Guid("2B7F1123-5B0C-415b-8B74-B8563871D33D");
	}
}
