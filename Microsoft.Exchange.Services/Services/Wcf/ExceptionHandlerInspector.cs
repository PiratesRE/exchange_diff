using System;
using System.Security.Principal;
using System.ServiceModel;

namespace Microsoft.Exchange.Services.Wcf
{
	internal class ExceptionHandlerInspector : ExceptionHandlerBase
	{
		static ExceptionHandlerInspector()
		{
			ExceptionHandlerBase.InternalServerErrorFaultCode = FaultCode.CreateReceiverFaultCode("InternalServerError", "http://schemas.microsoft.com/exchange/services/2006/errors");
		}

		public static bool IsLocalAccount(SecurityIdentifier sid)
		{
			return sid.IsEqualDomainSid(ExceptionHandlerInspector.localSid);
		}

		private static SecurityIdentifier localSid = new SecurityIdentifier(WellKnownSidType.BuiltinUsersSid, null);
	}
}
