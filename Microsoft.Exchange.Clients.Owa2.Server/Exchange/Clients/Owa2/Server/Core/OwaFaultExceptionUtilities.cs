using System;
using System.ServiceModel;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Services.Core;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal static class OwaFaultExceptionUtilities
	{
		public static ServiceError GetServiceError(LocalizedException exception)
		{
			return OwaFaultExceptionUtilities.exceptionMapper.GetServiceError(exception);
		}

		public static FaultException CreateFault(LocalizedException exception)
		{
			return new OwaServiceFaultException(OwaFaultExceptionUtilities.GetServiceError(exception), exception);
		}

		private static ExceptionMapper exceptionMapper = new ExceptionMapper(ServiceErrors.GetExceptionMapper(), new ExceptionMappingBase[]
		{
			new OwaExceptionMapper(typeof(OwaException)),
			new OwaExceptionMapper(typeof(OwaPermanentException)),
			new OwaExceptionMapper(typeof(OwaTransientException))
		});
	}
}
