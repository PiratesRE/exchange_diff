using System;

namespace Microsoft.Exchange.RpcClientAccess.Server
{
	[Serializable]
	internal sealed class LoginPermException : RpcServerException
	{
		internal LoginPermException(string message) : base(message, RpcErrorCode.LoginPerm)
		{
		}
	}
}
