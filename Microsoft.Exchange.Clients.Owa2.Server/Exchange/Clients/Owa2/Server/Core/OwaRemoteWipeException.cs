using System;
using System.ServiceModel;
using Microsoft.Exchange.Clients.Common;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	public class OwaRemoteWipeException : OwaExtendedErrorCodeException
	{
		public OwaRemoteWipeException(string message, string user) : base(OwaExtendedErrorCode.RemoteWipe, message, user, FaultCode.CreateSenderFaultCode("RemoteWipe", "Owa"))
		{
		}
	}
}
