using System;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal class SuccessfulRegisterNotificationResult : RopResult
	{
		internal SuccessfulRegisterNotificationResult(IServerObject serverObject) : base(RopId.RegisterNotification, ErrorCode.None, serverObject)
		{
			if (serverObject == null)
			{
				throw new ArgumentNullException("serverObject");
			}
		}

		internal SuccessfulRegisterNotificationResult(Reader reader) : base(reader)
		{
		}

		internal static SuccessfulRegisterNotificationResult Parse(Reader reader)
		{
			return new SuccessfulRegisterNotificationResult(reader);
		}
	}
}
