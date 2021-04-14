using System;
using System.Net;
using System.Text;

namespace Microsoft.Exchange.EDiscovery.Export
{
	public class ServiceHttpContext
	{
		internal string AnchorMailbox { get; set; }

		internal Guid ClientRequestId { get; set; }

		internal WebHeaderCollection ResponseHttpHeaders { get; set; }

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine();
			ServiceHttpContext.AppendToString(stringBuilder, "X-AnchorMailbox", this.AnchorMailbox);
			ServiceHttpContext.AppendToString(stringBuilder, "client-request-id", this.ClientRequestId.ToString());
			if (this.ResponseHttpHeaders != null)
			{
				stringBuilder.Append(this.ResponseHttpHeaders.ToString());
			}
			return stringBuilder.ToString();
		}

		internal void SetRequestHttpHeaders(WebRequest request)
		{
			ServiceHttpContext.SetHttpHeader(request, "X-AnchorMailbox", this.AnchorMailbox);
			ServiceHttpContext.SetHttpHeader(request, "client-request-id", this.ClientRequestId.ToString());
			ServiceHttpContext.SetHttpHeader(request, "return-client-request-id", "true");
		}

		internal void UpdateContextFromResponse(WebResponse response)
		{
			this.ResponseHttpHeaders = response.Headers;
			HttpWebResponse httpWebResponse = response as HttpWebResponse;
			if (httpWebResponse != null && httpWebResponse.StatusCode != HttpStatusCode.OK)
			{
				Tracer.TraceError("ServiceHttpContext.UpdateContextFromResponse: HTTP status code is '{0}', it should be 'OK'. \nServiceHttpContext:\n{1}", new object[]
				{
					httpWebResponse.StatusCode,
					this
				});
			}
		}

		private static void AppendToString(StringBuilder sb, string name, string value)
		{
			sb.Append(name);
			sb.Append(":");
			sb.Append(value);
			sb.AppendLine();
		}

		private static void SetHttpHeader(WebRequest request, string name, string value)
		{
			if (string.IsNullOrEmpty(value))
			{
				request.Headers.Remove(name);
				return;
			}
			request.Headers.Set(name, value);
		}

		private const string AnchorMailboxHeaderName = "X-AnchorMailbox";

		private const string ClientRequestIdHeaderName = "client-request-id";

		private const string ReturnClientRequestIdHeaderName = "return-client-request-id";
	}
}
