using System;
using System.Diagnostics.CodeAnalysis;
using System.Net;

namespace Microsoft.Exchange.AutoDiscoverV2
{
	[ExcludeFromCodeCoverage]
	internal class AutoDiscoverResponseException : Exception
	{
		protected AutoDiscoverResponseException(HttpStatusCode httpStatusCode, string errorCode, string errorMessage, Exception innerException = null) : base(errorMessage, innerException)
		{
			this.HttpStatusCodeValue = (int)httpStatusCode;
			this.ErrorCode = errorCode;
		}

		private AutoDiscoverResponseException(HttpStatusCode httpStatusCode, string errorCode, Exception innerException) : base(innerException.Message, innerException)
		{
			this.HttpStatusCodeValue = (int)httpStatusCode;
			this.ErrorCode = errorCode;
		}

		public int HttpStatusCodeValue { get; protected set; }

		public string ErrorCode { get; protected set; }

		public static AutoDiscoverResponseException BadRequest(string errorCode, string errorMessage, Exception innerException = null)
		{
			return new AutoDiscoverResponseException(HttpStatusCode.BadRequest, errorCode, errorMessage, innerException);
		}

		public static AutoDiscoverResponseException NotFound()
		{
			return new AutoDiscoverResponseException(HttpStatusCode.NotFound, "UserNotFound", "The given user is not found", null);
		}

		public static AutoDiscoverResponseException NotFound(string errorCode, string errorMessage, Exception innerException = null)
		{
			return new AutoDiscoverResponseException(HttpStatusCode.NotFound, errorCode, errorMessage, innerException);
		}

		public static AutoDiscoverResponseException ServiceUnavailable(string errorMessage, Exception innerException = null)
		{
			return new AutoDiscoverResponseException(HttpStatusCode.ServiceUnavailable, errorMessage, innerException);
		}

		public static AutoDiscoverResponseException InternalServerError(string errorMessage, Exception innerException = null)
		{
			return new AutoDiscoverResponseException(HttpStatusCode.InternalServerError, "InternalServerError", errorMessage, innerException);
		}

		public static AutoDiscoverResponseException NotImplemented(string errorMessage, Exception innerException = null)
		{
			return new AutoDiscoverResponseException(HttpStatusCode.NotImplemented, "NotImplemented", errorMessage, innerException);
		}

		public static AutoDiscoverResponseException DomainNotFound(string errorMessage, Exception innerException = null)
		{
			return new AutoDiscoverResponseException(HttpStatusCode.NotFound, "DomainNotFound", errorMessage, innerException);
		}
	}
}
