using System;
using System.IO;
using System.Net;
using System.ServiceModel.Channels;
using System.Web;

namespace Microsoft.Exchange.Services.Wcf
{
	internal class HttpResponseRenderer : BaseResponseRenderer
	{
		public static HttpResponseRenderer Create(HttpStatusCode statusCode)
		{
			return new HttpResponseRenderer(statusCode, null, null);
		}

		public static HttpResponseRenderer Create(HttpStatusCode statusCode, string body)
		{
			return new HttpResponseRenderer(statusCode, body, null);
		}

		public static HttpResponseRenderer CreateRedirect(string location)
		{
			return new HttpResponseRenderer(HttpStatusCode.Found, null, location);
		}

		private HttpResponseRenderer(HttpStatusCode statusCode, string body, string location)
		{
			this.statusCode = statusCode;
			this.responseBody = body;
			this.location = location;
		}

		internal override void Render(Message message, Stream stream)
		{
			this.Render(message, stream, HttpContext.Current.Response);
		}

		internal override void Render(Message message, Stream stream, HttpResponse response)
		{
			if (response.IsClientConnected)
			{
				response.StatusCode = (int)this.statusCode;
				if (this.statusCode == HttpStatusCode.Found)
				{
					response.Redirect(this.location, false);
				}
				if (string.IsNullOrEmpty(this.responseBody))
				{
					response.SuppressContent = true;
				}
				else
				{
					response.Write(this.responseBody);
				}
				response.Flush();
			}
		}

		private HttpStatusCode statusCode;

		private string responseBody;

		private string location;
	}
}
