using System;
using System.Runtime.InteropServices;
using Microsoft.Exchange.Security;

namespace Microsoft.Exchange.Clients.Security
{
	public class LiveTransientHRESULTException : LiveTransientException
	{
		public LiveTransientHRESULTException(COMException e, uint errorCode) : base(Strings.LiveTransientHRESULTExceptionMessage(errorCode, Enum.GetName(typeof(RPSErrorCode), errorCode) ?? string.Empty), e)
		{
		}
	}
}
