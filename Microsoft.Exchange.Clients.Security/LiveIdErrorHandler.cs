using System;
using System.Runtime.InteropServices;
using Microsoft.Exchange.Security;

namespace Microsoft.Exchange.Clients.Security
{
	internal static class LiveIdErrorHandler
	{
		public static void ThrowRPSException(COMException e)
		{
			switch (RPSErrorHandler.CategorizeRPSException(e))
			{
			case RPSErrorCategory.ConfigurationError:
				throw new LiveConfigurationHRESULTException(e, (uint)e.ErrorCode);
			case RPSErrorCategory.TransientError:
				throw new LiveTransientHRESULTException(e, (uint)e.ErrorCode);
			case RPSErrorCategory.ExternalError:
				throw new LiveExternalHRESULTException(e, (uint)e.ErrorCode);
			case RPSErrorCategory.ClientError:
				throw new LiveClientHRESULTException(e, (uint)e.ErrorCode);
			}
			throw new LiveOperationException(e, (uint)e.ErrorCode);
		}
	}
}
