using System;
using System.Net;

namespace Microsoft.Exchange.Services.OData
{
	[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
	internal class ODataHttpStatusCodeAttribute : Attribute
	{
		public ODataHttpStatusCodeAttribute(HttpStatusCode statusCode)
		{
			this.HttpStatusCode = statusCode;
		}

		public HttpStatusCode HttpStatusCode { get; set; }
	}
}
