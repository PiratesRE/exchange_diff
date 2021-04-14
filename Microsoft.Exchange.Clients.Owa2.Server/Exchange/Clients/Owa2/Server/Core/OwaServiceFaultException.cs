using System;
using System.ServiceModel;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal class OwaServiceFaultException : FaultException
	{
		internal OwaServiceFaultException(ServiceError serviceError, LocalizedException serviceException) : base(new FaultReason(serviceException.Message))
		{
			this.ServiceError = serviceError;
			this.ServiceException = serviceException;
		}

		internal ServiceError ServiceError { get; private set; }

		internal LocalizedException ServiceException { get; private set; }
	}
}
