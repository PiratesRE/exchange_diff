using System;
using System.Net;
using System.Web;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.OData
{
	internal abstract class ODataResponseException : Exception
	{
		public ODataResponseException(HttpStatusCode httpStatusCode, string errorCode, LocalizedString errorMessage, Exception innerException = null) : base(errorMessage, innerException)
		{
			this.HttpStatusCode = httpStatusCode;
			this.ErrorCode = errorCode;
		}

		public ODataResponseException(HttpStatusCode httpStatusCode, ResponseCodeType errorCode, LocalizedString errorMessage, Exception innerException = null) : this(httpStatusCode, errorCode.ToString(), errorMessage, innerException)
		{
		}

		public HttpStatusCode HttpStatusCode { get; protected set; }

		public string ErrorCode { get; protected set; }

		public virtual void AppendResponseHeader(HttpContext httpContext)
		{
		}
	}
}
