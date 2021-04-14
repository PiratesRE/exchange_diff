using System;
using System.ServiceModel;
using Microsoft.Exchange.Clients.Common;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	public class OwaDeviceNotProvisionedException : OwaExtendedErrorCodeException
	{
		public OwaDeviceNotProvisionedException(string message, string user) : base(OwaExtendedErrorCode.InvalidDeviceId, message, user, FaultCode.CreateSenderFaultCode("DeviceNotProvisioned", "Owa"))
		{
		}
	}
}
