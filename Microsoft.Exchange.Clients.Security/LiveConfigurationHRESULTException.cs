using System;
using Microsoft.Exchange.Security;

namespace Microsoft.Exchange.Clients.Security
{
	public class LiveConfigurationHRESULTException : LiveConfigurationException
	{
		public LiveConfigurationHRESULTException(Exception e, uint errorCode) : base(Strings.LiveConfigurationHRESULTExceptionMessage(errorCode, Enum.GetName(typeof(RPSErrorCode), errorCode) ?? string.Empty), e)
		{
		}
	}
}
