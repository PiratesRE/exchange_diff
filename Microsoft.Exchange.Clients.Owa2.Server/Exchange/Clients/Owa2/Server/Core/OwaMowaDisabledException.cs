using System;
using System.ServiceModel;
using Microsoft.Exchange.Clients.Common;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	public class OwaMowaDisabledException : OwaExtendedErrorCodeException
	{
		public OwaMowaDisabledException(string message, string user) : base(OwaExtendedErrorCode.MowaDisabled, message, user, FaultCode.CreateSenderFaultCode("MowaDisabled", "Owa"))
		{
		}
	}
}
