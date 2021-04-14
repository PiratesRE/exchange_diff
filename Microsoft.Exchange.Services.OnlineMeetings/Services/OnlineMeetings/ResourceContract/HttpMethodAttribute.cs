using System;
using System.Net;

namespace Microsoft.Exchange.Services.OnlineMeetings.ResourceContract
{
	[AttributeUsage(AttributeTargets.Class)]
	internal class HttpMethodAttribute : Attribute
	{
		public HttpMethodAttribute(string method)
		{
			this.method = method;
		}

		public string Method
		{
			get
			{
				return this.method;
			}
		}

		public HttpStatusCode StatusCode
		{
			get
			{
				return this.statusCode;
			}
			protected set
			{
				this.statusCode = value;
			}
		}

		public string ContextToken
		{
			get
			{
				return this.contextToken;
			}
			set
			{
				this.contextToken = value;
			}
		}

		public bool AppliesToToken(string token)
		{
			return this.contextToken == null || token == null || string.Compare(this.contextToken, token, StringComparison.CurrentCultureIgnoreCase) == 0;
		}

		private readonly string method;

		private HttpStatusCode statusCode = HttpStatusCode.OK;

		private string contextToken;
	}
}
