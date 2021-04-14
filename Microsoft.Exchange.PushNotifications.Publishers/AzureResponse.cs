using System;
using System.IO;
using System.Net;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Extensions;
using Microsoft.Exchange.PushNotifications.Extensions;

namespace Microsoft.Exchange.PushNotifications.Publishers
{
	internal class AzureResponse
	{
		public AzureResponse(string responseBody, WebHeaderCollection responseHeaders = null)
		{
			this.OriginalStatusCode = new HttpStatusCode?(HttpStatusCode.OK);
			this.OriginalBody = responseBody;
			this.ResponseHeaders = responseHeaders;
		}

		public AzureResponse(Exception exception, Uri targetUri, string responseBody)
		{
			ArgumentValidator.ThrowIfNull("exception", exception);
			ArgumentValidator.ThrowIfNull("targetUri", targetUri);
			this.Exception = exception;
			this.TargetUri = targetUri;
			this.OriginalBody = responseBody;
		}

		public AzureResponse(WebException webException, Uri targetUri, string responseBody)
		{
			ArgumentValidator.ThrowIfNull("webException", webException);
			ArgumentValidator.ThrowIfNull("targetUri", targetUri);
			this.Exception = webException;
			this.TargetUri = targetUri;
			this.OriginalBody = responseBody;
			HttpWebResponse httpWebResponse = webException.Response as HttpWebResponse;
			if (httpWebResponse != null)
			{
				this.OriginalStatusCode = new HttpStatusCode?(httpWebResponse.StatusCode);
			}
			if (responseBody == null && webException.Response != null)
			{
				using (StreamReader streamReader = new StreamReader(webException.Response.GetResponseStream()))
				{
					this.OriginalBody = streamReader.ReadToEnd();
				}
			}
		}

		public Exception Exception { get; private set; }

		public Uri TargetUri { get; private set; }

		public HttpStatusCode? OriginalStatusCode { get; private set; }

		public string OriginalBody { get; private set; }

		public WebHeaderCollection ResponseHeaders { get; private set; }

		public bool HasSucceeded
		{
			get
			{
				return this.OriginalStatusCode != null && this.OriginalStatusCode.Value >= HttpStatusCode.OK && this.OriginalStatusCode.Value < HttpStatusCode.MultipleChoices;
			}
		}

		public string ToTraceString()
		{
			if (this.toFullString == null)
			{
				this.toFullString = string.Format("{{ Target-Uri:{0}; Statue-Code:{1}; Response-Body:{2}; Headers:[{3}]; Exception:{4}; {5}}}", new object[]
				{
					this.TargetUri,
					this.OriginalStatusCode.ToNullableString<HttpStatusCode>(),
					this.OriginalBody.ToNullableString(),
					this.ResponseHeaders.ToTraceString(null),
					this.Exception.ToTraceString(),
					this.InternalToTraceString()
				});
			}
			return this.toFullString;
		}

		protected virtual string InternalToTraceString()
		{
			return null;
		}

		private string toFullString;
	}
}
