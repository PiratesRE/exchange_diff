using System;
using System.Collections.Specialized;
using System.Net;
using System.Web;
using Microsoft.Exchange.Clients.Common;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal class AttachmentWebOperationContext : IAttachmentWebOperationContext, IOutgoingWebResponseContext
	{
		internal AttachmentWebOperationContext(HttpContext httpContext, IOutgoingWebResponseContext response)
		{
			this.response = response;
			this.httpContext = httpContext;
			this.userAgent = new UserAgent(httpContext.Request.UserAgent, UserContextManager.GetUserContext(httpContext).FeaturesManager.ClientServerSettings.ChangeLayout.Enabled, httpContext.Request.Cookies);
			UserContext userContext = UserContextManager.GetUserContext(CallContext.Current.HttpContext, CallContext.Current.EffectiveCaller, true);
			this.isPublicLogon = userContext.IsPublicLogon;
		}

		public UserAgent UserAgent
		{
			get
			{
				return this.userAgent;
			}
		}

		public bool IsPublicLogon
		{
			get
			{
				return this.isPublicLogon;
			}
		}

		public string GetRequestHeader(string name)
		{
			return this.httpContext.Request.Headers[name];
		}

		public void SetNoCacheNoStore()
		{
			HttpContext.Current.Response.Cache.SetNoStore();
			HttpContext.Current.Response.Cache.SetCacheability(HttpCacheability.NoCache);
		}

		public string ContentType
		{
			set
			{
				this.response.ContentType = value;
			}
		}

		public string ETag
		{
			set
			{
				this.response.ETag = value;
			}
		}

		public string Expires
		{
			set
			{
				this.response.Expires = value;
			}
		}

		public HttpStatusCode StatusCode
		{
			get
			{
				return this.response.StatusCode;
			}
			set
			{
				this.response.StatusCode = value;
			}
		}

		public NameValueCollection Headers
		{
			get
			{
				return this.response.Headers;
			}
		}

		public bool SuppressContent
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		private readonly bool isPublicLogon;

		private IOutgoingWebResponseContext response;

		private UserAgent userAgent;

		private readonly HttpContext httpContext;
	}
}
