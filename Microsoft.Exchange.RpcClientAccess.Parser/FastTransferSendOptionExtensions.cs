using System;

namespace Microsoft.Exchange.RpcClientAccess
{
	internal static class FastTransferSendOptionExtensions
	{
		public static bool UseCpidOrUnicode(this FastTransferSendOption sendOptions)
		{
			return (byte)(sendOptions & (FastTransferSendOption.Unicode | FastTransferSendOption.UseCpId | FastTransferSendOption.ForceUnicode)) != 0;
		}

		public static bool UseCpid(this FastTransferSendOption sendOptions)
		{
			return (byte)(sendOptions & FastTransferSendOption.UseCpId) != 0;
		}

		public static bool IsUpload(this FastTransferSendOption sendOptions)
		{
			return (byte)(sendOptions & FastTransferSendOption.Upload) == 3;
		}

		public static bool WantUnicode(this FastTransferSendOption sendOptions)
		{
			return (byte)(sendOptions & (FastTransferSendOption.Unicode | FastTransferSendOption.ForceUnicode)) != 0;
		}
	}
}
