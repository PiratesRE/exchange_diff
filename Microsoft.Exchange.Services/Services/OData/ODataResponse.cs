using System;
using System.Collections.Specialized;
using System.Net;
using System.Web;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Services.OData.Model;
using Microsoft.Exchange.Services.OData.Web;

namespace Microsoft.Exchange.Services.OData
{
	internal class ODataResponse
	{
		public ODataResponse(ODataRequest request)
		{
			ArgumentValidator.ThrowIfNull("request", request);
			this.Request = request;
		}

		public ODataRequest Request { get; private set; }

		public void WriteHttpResponse()
		{
			HttpContext httpContext = this.Request.ODataContext.HttpContext;
			httpContext.Response.StatusCode = (int)this.HttpResponseCodeOnSuccess;
			this.ApplyResponseHeaders(httpContext.Response.Headers);
			if (this.InternalResult != null)
			{
				ResponseMessageWriter responseMessageWriter = new ResponseMessageWriter(httpContext, this.Request.ODataContext.ServiceModel);
				responseMessageWriter.WriteDataResult(this.Request.ODataContext, this.InternalResult);
			}
		}

		protected object InternalResult { get; set; }

		protected virtual HttpStatusCode HttpResponseCodeOnSuccess
		{
			get
			{
				return HttpStatusCode.OK;
			}
		}

		protected virtual void ApplyResponseHeaders(NameValueCollection responseHeaders)
		{
			string etag = this.GetEtag();
			if (!string.IsNullOrEmpty(etag))
			{
				responseHeaders["ETag"] = etag;
			}
		}

		protected string GetEntityLocation()
		{
			string result = null;
			Entity entity = this.InternalResult as Entity;
			if (entity != null)
			{
				result = entity.GetWebUri(this.Request.ODataContext).OriginalString;
			}
			return result;
		}

		protected string GetEtag()
		{
			string result = null;
			Entity entity = this.InternalResult as Entity;
			if (entity != null)
			{
				object arg = null;
				if (entity.PropertyBag.TryGetValue(ItemSchema.ChangeKey, out arg))
				{
					result = string.Format("W/\"{0}\"", arg);
				}
			}
			return result;
		}
	}
}
