using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Web;
using Microsoft.Exchange.Diagnostics.Components.HttpProxy;
using Microsoft.Exchange.Diagnostics.Components.Security;

namespace Microsoft.Exchange.Diagnostics
{
	internal class RequestFailureContext
	{
		public RequestFailureContext(RequestFailureContext.RequestFailurePoint failurePoint, int httpStatusCode, string error, string details, HttpProxySubErrorCode? httpProxySubErrorCode = null, WebExceptionStatus? webExceptionStatus = null, LiveIdAuthResult? liveIdAuthResult = null)
		{
			if (error == null)
			{
				throw new ArgumentNullException("error");
			}
			if (details == null)
			{
				throw new ArgumentNullException("details");
			}
			this.FailurePoint = failurePoint;
			this.HttpStatusCode = httpStatusCode;
			this.Error = error;
			this.Details = details;
			this.HttpProxySubErrorCode = httpProxySubErrorCode;
			this.WebExceptionStatus = webExceptionStatus;
			this.LiveIdAuthResult = liveIdAuthResult;
		}

		private RequestFailureContext(string serializedFailureContext)
		{
			if (string.IsNullOrEmpty(serializedFailureContext))
			{
				throw new ArgumentException("serializedFailureContext is null or empty");
			}
			this.Deserialize(serializedFailureContext);
		}

		public RequestFailureContext.RequestFailurePoint FailurePoint { get; private set; }

		public int HttpStatusCode { get; private set; }

		public HttpProxySubErrorCode? HttpProxySubErrorCode { get; private set; }

		public WebExceptionStatus? WebExceptionStatus { get; private set; }

		public LiveIdAuthResult? LiveIdAuthResult { get; private set; }

		public string Error { get; private set; }

		public string Details { get; private set; }

		public string UnrecognizedFailurePoint { get; private set; }

		public static bool TryCreateFromResponse(HttpWebResponse cafeResponse, out RequestFailureContext requestFailureContext)
		{
			if (cafeResponse == null)
			{
				throw new ArgumentNullException("cafeResponse");
			}
			return RequestFailureContext.TryCreateFromResponseHeaders(cafeResponse.Headers, out requestFailureContext);
		}

		public static bool TryCreateFromResponseHeaders(WebHeaderCollection webHeaderCollection, out RequestFailureContext requestFailureContext)
		{
			if (webHeaderCollection == null)
			{
				throw new ArgumentNullException("webHeaderCollection");
			}
			return RequestFailureContext.TryDeserialize(webHeaderCollection[RequestFailureContext.HeaderKey], out requestFailureContext);
		}

		public static bool TryCreateFromResponseHeaders(IDictionary<string, string> headerDictionary, out RequestFailureContext requestFailureContext)
		{
			if (headerDictionary == null)
			{
				throw new ArgumentNullException("headerDictionary");
			}
			requestFailureContext = null;
			string headerValue;
			return headerDictionary.TryGetValue(RequestFailureContext.HeaderKey, out headerValue) && RequestFailureContext.TryDeserialize(headerValue, out requestFailureContext);
		}

		public static bool TryDeserialize(string headerValue, out RequestFailureContext requestFailureContext)
		{
			requestFailureContext = null;
			if (!string.IsNullOrEmpty(headerValue))
			{
				try
				{
					requestFailureContext = new RequestFailureContext(headerValue);
					return true;
				}
				catch (FormatException)
				{
				}
				return false;
			}
			return false;
		}

		private static string ConvertStringFromBase64(string input)
		{
			string @string;
			try
			{
				byte[] bytes = Convert.FromBase64String(input);
				@string = Encoding.UTF8.GetString(bytes);
			}
			catch (ArgumentException innerException)
			{
				throw new FormatException(string.Format("Couldn't convert base64 string: {0}", input), innerException);
			}
			return @string;
		}

		private static int ConvertStringToInt(string input)
		{
			int result;
			if (!int.TryParse(input, out result))
			{
				throw new FormatException(string.Format("Couldn't parse input string as an int: {0}", input));
			}
			return result;
		}

		public string Serialize()
		{
			return string.Format("{0};{1};{2};{3};{4};{5};{6}", new object[]
			{
				this.FailurePoint.ToString(),
				this.HttpStatusCode,
				Convert.ToBase64String(Encoding.UTF8.GetBytes(this.Error)),
				Convert.ToBase64String(Encoding.UTF8.GetBytes(this.Details)),
				(this.HttpProxySubErrorCode != null) ? this.HttpProxySubErrorCode.ToString() : string.Empty,
				(this.WebExceptionStatus != null) ? this.WebExceptionStatus.ToString() : string.Empty,
				(this.LiveIdAuthResult != null) ? this.LiveIdAuthResult.ToString() : string.Empty
			});
		}

		public void UpdateResponse(HttpResponse response)
		{
			if (response == null)
			{
				throw new ArgumentNullException("response");
			}
			response.Headers[RequestFailureContext.HeaderKey] = this.Serialize();
		}

		private void Deserialize(string serializedFailureContext)
		{
			string[] array = serializedFailureContext.Split(new char[]
			{
				';'
			});
			int num = 0;
			if (array.Length >= 4)
			{
				string text = array[num++];
				RequestFailureContext.RequestFailurePoint failurePoint;
				if (!Enum.TryParse<RequestFailureContext.RequestFailurePoint>(text, out failurePoint))
				{
					failurePoint = RequestFailureContext.RequestFailurePoint.Unrecognized;
					this.UnrecognizedFailurePoint = text;
				}
				this.FailurePoint = failurePoint;
				this.HttpStatusCode = RequestFailureContext.ConvertStringToInt(array[num++]);
				this.Error = RequestFailureContext.ConvertStringFromBase64(array[num++]);
				this.Details = RequestFailureContext.ConvertStringFromBase64(array[num++]);
				if (array.Length >= 6)
				{
					HttpProxySubErrorCode value;
					if (!Enum.TryParse<HttpProxySubErrorCode>(array[num++], out value))
					{
						this.HttpProxySubErrorCode = null;
					}
					else
					{
						this.HttpProxySubErrorCode = new HttpProxySubErrorCode?(value);
					}
					WebExceptionStatus value2;
					if (!Enum.TryParse<WebExceptionStatus>(array[num++], out value2))
					{
						this.WebExceptionStatus = null;
					}
					else
					{
						this.WebExceptionStatus = new WebExceptionStatus?(value2);
					}
				}
				if (array.Length >= 7)
				{
					LiveIdAuthResult value3;
					if (!Enum.TryParse<LiveIdAuthResult>(array[num++], out value3))
					{
						this.LiveIdAuthResult = null;
						return;
					}
					this.LiveIdAuthResult = new LiveIdAuthResult?(value3);
				}
				return;
			}
			throw new FormatException("Expected a minimum of 4 parameters.");
		}

		public override string ToString()
		{
			string text = this.FailurePoint.ToString();
			if (this.FailurePoint == RequestFailureContext.RequestFailurePoint.Unrecognized)
			{
				text = string.Format("{0}({1})", text, this.UnrecognizedFailurePoint);
			}
			return string.Format("FailurePoint={0}, HttpStatusCode={1}, Error={2}, Details={3}, HttpProxySubErrorCode={4}, WebExceptionStatus={5}, LiveIdAuthResult={6}", new object[]
			{
				text,
				this.HttpStatusCode,
				this.Error,
				this.Details,
				this.HttpProxySubErrorCode,
				this.WebExceptionStatus,
				this.LiveIdAuthResult
			});
		}

		private static readonly string HeaderKey = "X-FailureContext";

		public static readonly string HttpContextKeyName = "RequestFailureContext";

		public enum RequestFailurePoint
		{
			Unrecognized,
			FrontEnd,
			BackEnd
		}
	}
}
