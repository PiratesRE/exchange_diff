using System;
using System.Globalization;
using System.Net;
using System.Web;
using Microsoft.Exchange.Diagnostics.Components.Clients;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal sealed class RequestContext
	{
		private RequestContext(HttpContext httpContext)
		{
			this.httpContext = httpContext;
		}

		public static RequestContext Current
		{
			get
			{
				HttpContext httpContext = HttpContext.Current;
				if (httpContext != null)
				{
					return (RequestContext)httpContext.Items["RequestContext"];
				}
				return null;
			}
		}

		public bool ErrorSent
		{
			get
			{
				return this.errorSent;
			}
			set
			{
				this.errorSent = value;
			}
		}

		public HttpContext HttpContext
		{
			get
			{
				return this.httpContext;
			}
		}

		public IMailboxContext UserContext
		{
			get
			{
				return this.userContext;
			}
			set
			{
				this.userContext = value;
			}
		}

		public OwaRequestType RequestType
		{
			get
			{
				return this.requestType;
			}
			set
			{
				this.requestType = value;
			}
		}

		internal HttpStatusCode HttpStatusCode
		{
			get
			{
				return this.httpStatusCode;
			}
			set
			{
				this.httpStatusCode = value;
			}
		}

		internal string DestinationUrl
		{
			get
			{
				return this.destinationUrl;
			}
			set
			{
				this.destinationUrl = value;
			}
		}

		internal string DestinationUrlQueryString
		{
			get
			{
				return this.destinationUrlQueryString;
			}
			set
			{
				this.destinationUrlQueryString = value;
			}
		}

		internal bool FailedToSaveUserCulture
		{
			get
			{
				return this.failedToSaveUserCulture;
			}
			set
			{
				this.failedToSaveUserCulture = value;
			}
		}

		internal CultureInfo LanguagePostUserCulture
		{
			get
			{
				return this.languagePostUserCulture;
			}
			set
			{
				this.languagePostUserCulture = value;
			}
		}

		internal static RequestContext Create(HttpContext httpContext)
		{
			RequestContext requestContext = new RequestContext(httpContext);
			ExTraceGlobals.RequestTracer.TraceDebug<DateTime>(0L, "New request received: {0}", requestContext.creationTime);
			return requestContext;
		}

		internal static RequestContext Get(HttpContext httpContext)
		{
			return (RequestContext)httpContext.Items["RequestContext"];
		}

		internal void Set(HttpContext httpContext)
		{
			httpContext.Items["RequestContext"] = this;
		}

		private const string RequestContextKey = "RequestContext";

		private HttpContext httpContext;

		private OwaRequestType requestType;

		private HttpStatusCode httpStatusCode = HttpStatusCode.OK;

		private string destinationUrl;

		private string destinationUrlQueryString;

		private bool failedToSaveUserCulture;

		private CultureInfo languagePostUserCulture;

		private IMailboxContext userContext;

		private bool errorSent;

		private readonly DateTime creationTime = DateTime.UtcNow;
	}
}
