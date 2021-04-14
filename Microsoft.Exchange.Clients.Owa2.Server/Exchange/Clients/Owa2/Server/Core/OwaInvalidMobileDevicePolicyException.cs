using System;
using System.ServiceModel;
using Microsoft.Exchange.Clients.Common;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	public class OwaInvalidMobileDevicePolicyException : OwaExtendedErrorCodeException
	{
		public OwaInvalidMobileDevicePolicyException(string message, string user, string expectedPolicyId) : base(OwaExtendedErrorCode.InvalidMobileDevicePolicy, message, user, FaultCode.CreateSenderFaultCode("InvalidMobileDevicePolicy", "Owa"), expectedPolicyId)
		{
		}
	}
}
