using System;
using System.Runtime.InteropServices;
using Microsoft.Exchange.Security;

namespace Microsoft.Exchange.Clients.Security
{
	public class LiveClientHRESULTException : LiveClientException
	{
		public LiveClientHRESULTException(COMException e, uint errorCode) : base(Strings.LiveClientHRESULTExceptionMessage(errorCode, Enum.GetName(typeof(RPSErrorCode), errorCode) ?? string.Empty), e)
		{
		}
	}
}
