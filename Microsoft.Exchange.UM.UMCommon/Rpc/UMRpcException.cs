using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.UM.UMCommon;
using Microsoft.Exchange.UM.UMCommon.Exceptions;

namespace Microsoft.Exchange.UM.Rpc
{
	internal class UMRpcException : LocalizedException
	{
		internal UMRpcException(int result, string userName, string serverName, Exception innerException) : base(UMRpcException.GetLocalizedMessage(userName, serverName, result), innerException)
		{
			base.ErrorCode = result;
		}

		internal UMRpcException(Exception innerException) : this(UMRpcException.GetHRFromException(innerException), null, null, innerException)
		{
		}

		private static LocalizedString GetLocalizedMessage(string userName, string serverName, int errorCode)
		{
			switch (errorCode)
			{
			case -2147466751:
				return Strings.UMRpcTransientError(userName, serverName);
			case -2147466750:
				break;
			case -2147466749:
				return Strings.UMRPCIncompatibleVersionError(serverName);
			default:
				if (errorCode == 5)
				{
					return Strings.UMRpcAccessDeniedError(serverName);
				}
				break;
			}
			return Strings.UMRpcGenericError(userName, errorCode, serverName);
		}

		private static int GetHRFromException(Exception exception)
		{
			int result = -2147466752;
			if (exception is StorageTransientException || exception is DataSourceTransientException)
			{
				result = -2147466751;
			}
			else if (exception is UMRPCIncompabibleVersionException)
			{
				result = -2147466749;
			}
			else if (exception is UMInvalidPartnerMessageException)
			{
				result = -2147466750;
			}
			return result;
		}
	}
}
