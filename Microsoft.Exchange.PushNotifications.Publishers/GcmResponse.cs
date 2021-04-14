using System;
using System.Collections.Generic;
using System.Net;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Extensions;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.PushNotifications.Extensions;

namespace Microsoft.Exchange.PushNotifications.Publishers
{
	internal class GcmResponse
	{
		public GcmResponse(string responseBody)
		{
			this.TransportStatus = GcmTransportStatusCode.Success;
			this.OriginalStatusCode = new HttpStatusCode?(HttpStatusCode.OK);
			this.ResponseStatus = GcmResponseStatusCode.InvalidResponse;
			this.OriginalBody = responseBody;
			GcmPayloadReader gcmPayloadReader = new GcmPayloadReader();
			Dictionary<string, string> dictionary = gcmPayloadReader.Read(responseBody);
			if (dictionary.ContainsKey("id"))
			{
				this.ResponseStatus = GcmResponseStatusCode.Success;
				this.Id = dictionary["id"].ToNullableString();
				return;
			}
			GcmResponseStatusCode responseStatus;
			if (dictionary.ContainsKey("Error") && Enum.TryParse<GcmResponseStatusCode>(dictionary["Error"], true, out responseStatus))
			{
				this.ResponseStatus = responseStatus;
			}
		}

		public GcmResponse(Exception exception)
		{
			ArgumentValidator.ThrowIfNull("exception", exception);
			if (exception is DownloadCanceledException || exception is DownloadTimeoutException)
			{
				this.TransportStatus = GcmTransportStatusCode.Timeout;
			}
			else
			{
				this.TransportStatus = GcmTransportStatusCode.Unknown;
			}
			this.ResponseStatus = GcmResponseStatusCode.Undefined;
			this.Exception = exception;
		}

		public GcmResponse(WebException webException)
		{
			ArgumentValidator.ThrowIfNull("webException", webException);
			this.TransportStatus = GcmTransportStatusCode.Unknown;
			this.ResponseStatus = GcmResponseStatusCode.Undefined;
			this.Exception = webException;
			HttpWebResponse httpWebResponse = webException.Response as HttpWebResponse;
			if (httpWebResponse != null)
			{
				this.OriginalStatusCode = new HttpStatusCode?(httpWebResponse.StatusCode);
				if (this.OriginalStatusCode == HttpStatusCode.Unauthorized)
				{
					this.TransportStatus = GcmTransportStatusCode.Unauthorized;
					return;
				}
				if (this.OriginalStatusCode >= HttpStatusCode.InternalServerError && this.OriginalStatusCode.Value < (HttpStatusCode)600)
				{
					this.TransportStatus = ((this.OriginalStatusCode == HttpStatusCode.InternalServerError) ? GcmTransportStatusCode.InternalServerError : GcmTransportStatusCode.ServerUnavailable);
					this.OriginalRetryAfter = httpWebResponse.GetResponseHeader("Retry-After");
					if (!string.IsNullOrEmpty(this.OriginalRetryAfter))
					{
						int num = 0;
						if (int.TryParse(this.OriginalRetryAfter, out num))
						{
							this.BackOffEndTime = new ExDateTime?(ExDateTime.UtcNow.AddSeconds((double)num));
							return;
						}
						ExDateTime value;
						if (ExDateTime.TryParse(this.OriginalRetryAfter, out value))
						{
							this.BackOffEndTime = new ExDateTime?(value);
						}
					}
				}
			}
		}

		public GcmTransportStatusCode TransportStatus { get; private set; }

		public GcmResponseStatusCode ResponseStatus { get; private set; }

		public string Id { get; private set; }

		public Exception Exception { get; private set; }

		public ExDateTime? BackOffEndTime { get; private set; }

		private HttpStatusCode? OriginalStatusCode { get; set; }

		private string OriginalRetryAfter { get; set; }

		private string OriginalBody { get; set; }

		public string ToFullString()
		{
			if (this.toFullString == null)
			{
				this.toFullString = string.Format("{{ transportStatus:{0}; responseStatus:{1}; id:{2}; backOffEndTime:{3}; originalStatus:{4}; originalRetryAfter:{5}; originalBody:{6}; exception:{7} }}", new object[]
				{
					this.TransportStatus,
					this.ResponseStatus,
					this.Id.ToNullableString(),
					this.BackOffEndTime.ToNullableString<ExDateTime>(),
					this.OriginalStatusCode.ToNullableString<HttpStatusCode>(),
					this.OriginalRetryAfter.ToNullableString(),
					this.OriginalBody.ToNullableString(),
					(this.Exception != null) ? this.Exception.ToTraceString() : "null"
				});
			}
			return this.toFullString;
		}

		public const string IdKey = "id";

		public const string ErrorKey = "Error";

		public const string RetryAfterHeaderKey = "Retry-After";

		public static readonly char[] KeyValueSeparator = new char[]
		{
			'='
		};

		private string toFullString;
	}
}
