using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Ews.Probes;
using Microsoft.Exchange.WebServices.Data;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Calendar.Probes
{
	public abstract class CalendarProbeBase : AutodiscoverCommon
	{
		protected string GetResponseHeaders(ExchangeService exchService)
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (exchService != null && exchService.HttpResponseHeaders != null)
			{
				foreach (string text in exchService.HttpResponseHeaders.Keys)
				{
					stringBuilder.AppendFormat("{0}={1}{2}", text, exchService.HttpResponseHeaders[text], Environment.NewLine);
				}
			}
			return stringBuilder.ToString();
		}

		protected string GetProbeRunTrace(ExchangeService exchangeService)
		{
			string result = string.Empty;
			if (exchangeService != null && exchangeService.TraceListener != null)
			{
				result = ((EWSCommon.TraceListener)exchangeService.TraceListener).RequestLog;
			}
			return result;
		}

		protected void LogServiceProperties(ExchangeService service, string[] headers, StateAttribute[] stateAttributes)
		{
			if (service != null && service.HttpResponseHeaders != null)
			{
				base.UpdateLogWithHeader(headers, service.HttpResponseHeaders, 0, stateAttributes);
			}
		}

		protected string IgnoredServiceResponseErrorCode(ServiceResponseException exception)
		{
			if (this.serviceResponseErrorCodesToBeIgnored.Contains(exception.ErrorCode))
			{
				return exception.ErrorCode.ToString();
			}
			if (string.IsNullOrEmpty(base.Result.StateAttribute5))
			{
				return exception.ErrorCode.ToString();
			}
			return string.Empty;
		}

		protected string IgnoredServiceRequestErrorCode(ServiceRequestException exception)
		{
			Exception innerException = exception.InnerException;
			while (innerException.InnerException != null)
			{
				innerException = innerException.InnerException;
			}
			if (innerException.GetType().Equals(typeof(WebException)))
			{
				HttpWebResponse httpWebResponse = ((WebException)innerException).Response as HttpWebResponse;
				if (httpWebResponse != null)
				{
					HttpStatusCode statusCode = httpWebResponse.StatusCode;
					if (this.serviceRequestErrorCodesToBeIgnored.Contains(statusCode))
					{
						return statusCode.ToString();
					}
				}
			}
			else if (innerException.GetType().Equals(typeof(SocketException)))
			{
				return innerException.GetType().Name;
			}
			return string.Empty;
		}

		private List<HttpStatusCode> serviceRequestErrorCodesToBeIgnored = new List<HttpStatusCode>
		{
			HttpStatusCode.ServiceUnavailable,
			HttpStatusCode.Unauthorized,
			HttpStatusCode.RequestTimeout
		};

		private List<ServiceError> serviceResponseErrorCodesToBeIgnored = new List<ServiceError>
		{
			128,
			262,
			263,
			98,
			249
		};
	}
}
