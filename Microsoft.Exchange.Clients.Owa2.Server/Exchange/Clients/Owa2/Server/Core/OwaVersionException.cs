using System;
using System.ServiceModel;
using Microsoft.Exchange.Clients.Common;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	public class OwaVersionException : OwaExtendedErrorCodeException
	{
		public OwaVersionException(string message, string user) : base(OwaExtendedErrorCode.VersionMismatch, message, user, FaultCode.CreateSenderFaultCode("Version", "Owa"))
		{
		}
	}
}
