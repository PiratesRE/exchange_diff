using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Microsoft.Exchange.Services.OnlineMeetings
{
	internal class ErrorInformation
	{
		internal ErrorInformation()
		{
			this.Parameters = new Dictionary<string, string>();
			this.DebugInfo = new Dictionary<string, string>();
		}

		public ErrorCode Code { get; internal set; }

		public ErrorSubcode Subcode { get; internal set; }

		public string Message { get; internal set; }

		public IDictionary<string, string> Parameters { get; internal set; }

		public IDictionary<string, string> DebugInfo { get; internal set; }

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat("Code:{0};Subcode:{1};Message:{2};", this.Code, this.Subcode, this.Message);
			if (this.Parameters != null)
			{
				stringBuilder.Append("Parameters:");
				foreach (string text in this.Parameters.Keys)
				{
					stringBuilder.AppendFormat("{0}-{1}.", text, this.Parameters[text]);
				}
				stringBuilder.Append(";");
			}
			if (this.DebugInfo != null)
			{
				stringBuilder.Append("DebugInfo:");
				foreach (string text2 in this.DebugInfo.Keys)
				{
					stringBuilder.AppendFormat("{0}-{1}.", text2, this.DebugInfo[text2]);
				}
				stringBuilder.Append(";");
			}
			return stringBuilder.ToString();
		}

		public static ErrorCode TryGetErrorFromHttpStatusCode(HttpStatusCode httpResponseCode)
		{
			ErrorCode result = ErrorCode.Unknown;
			switch (httpResponseCode)
			{
			case HttpStatusCode.BadRequest:
				result = ErrorCode.BadRequest;
				break;
			case HttpStatusCode.Unauthorized:
			case HttpStatusCode.PaymentRequired:
			case HttpStatusCode.NotAcceptable:
			case HttpStatusCode.ProxyAuthenticationRequired:
			case HttpStatusCode.RequestTimeout:
			case HttpStatusCode.LengthRequired:
				break;
			case HttpStatusCode.Forbidden:
				result = ErrorCode.Forbidden;
				break;
			case HttpStatusCode.NotFound:
				result = ErrorCode.NotFound;
				break;
			case HttpStatusCode.MethodNotAllowed:
				result = ErrorCode.MethodNotAllowed;
				break;
			case HttpStatusCode.Conflict:
				result = ErrorCode.Conflict;
				break;
			case HttpStatusCode.Gone:
				result = ErrorCode.Gone;
				break;
			case HttpStatusCode.PreconditionFailed:
				result = ErrorCode.PreconditionFailed;
				break;
			default:
				if (httpResponseCode != HttpStatusCode.UnsupportedMediaType)
				{
					switch (httpResponseCode)
					{
					case HttpStatusCode.InternalServerError:
						result = ErrorCode.ServiceFailure;
						break;
					case HttpStatusCode.BadGateway:
						result = ErrorCode.BadGateway;
						break;
					case HttpStatusCode.ServiceUnavailable:
						result = ErrorCode.ServiceUnavailable;
						break;
					case HttpStatusCode.GatewayTimeout:
						result = ErrorCode.Timeout;
						break;
					}
				}
				else
				{
					result = ErrorCode.UnsupportedMediaType;
				}
				break;
			}
			return result;
		}
	}
}
