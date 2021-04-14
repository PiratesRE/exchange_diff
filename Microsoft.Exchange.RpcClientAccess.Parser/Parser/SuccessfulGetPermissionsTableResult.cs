using System;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class SuccessfulGetPermissionsTableResult : RopResult
	{
		internal SuccessfulGetPermissionsTableResult(IServerObject serverObject) : base(RopId.GetPermissionsTable, ErrorCode.None, serverObject)
		{
			if (serverObject == null)
			{
				throw new ArgumentNullException("serverObject");
			}
		}

		internal SuccessfulGetPermissionsTableResult(Reader reader) : base(reader)
		{
		}

		internal static SuccessfulGetPermissionsTableResult Parse(Reader reader)
		{
			return new SuccessfulGetPermissionsTableResult(reader);
		}
	}
}
