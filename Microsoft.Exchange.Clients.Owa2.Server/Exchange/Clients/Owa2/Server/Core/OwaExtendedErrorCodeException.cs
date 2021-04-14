using System;
using System.ServiceModel;
using Microsoft.Exchange.Clients.Common;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	public abstract class OwaExtendedErrorCodeException : FaultException
	{
		public OwaExtendedErrorCodeException(OwaExtendedErrorCode extendedErrorCode, string message, string user, FaultCode faultCode) : this(extendedErrorCode, message, user, faultCode, null)
		{
		}

		public OwaExtendedErrorCodeException(OwaExtendedErrorCode extendedErrorCode, string message, string user, FaultCode faultCode, string errorData) : base(message, faultCode)
		{
			this.extendedErrorCode = extendedErrorCode;
			this.user = user;
			this.errorData = errorData;
		}

		public OwaExtendedErrorCode ExtendedErrorCode
		{
			get
			{
				return this.extendedErrorCode;
			}
		}

		public string User
		{
			get
			{
				return this.user;
			}
		}

		public string ErrorData
		{
			get
			{
				return this.errorData;
			}
		}

		private readonly OwaExtendedErrorCode extendedErrorCode;

		private readonly string user;

		private readonly string errorData;
	}
}
