using System;
using System.Globalization;
using System.Net;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Diagnostics;

namespace Microsoft.Exchange.Services.Wcf
{
	internal static class WeatherCommon
	{
		internal static string ExecuteActionAndHandleException(CallContext callContext, int traceId, string defaultExceptionMessage, Action operation)
		{
			Exception exception = null;
			try
			{
				GrayException.MapAndReportGrayExceptions(delegate()
				{
					try
					{
						operation();
					}
					catch (Exception ex2)
					{
						if (ex2 is WebException)
						{
							callContext.ProtocolLog.Set(WeatherMetadata.WebExceptionStatusCode, ((WebException)ex2).Status);
							exception = ex2;
						}
						else
						{
							if (!(ex2 is ObjectDisposedException) && !(ex2 is WeatherException))
							{
								throw;
							}
							exception = ex2;
						}
					}
				});
			}
			catch (GrayException ex)
			{
				ExTraceGlobals.WeatherTracer.TraceError<GrayException>((long)traceId, "Request failed due to gray exception {0}", ex);
				exception = ex;
			}
			if (exception == null)
			{
				return null;
			}
			ExTraceGlobals.WeatherTracer.TraceError<Exception>((long)traceId, "Request failed due to exception {0}", exception);
			callContext.ProtocolLog.Set(WeatherMetadata.Failed, exception.Message);
			if (!string.IsNullOrEmpty(exception.Message))
			{
				return exception.Message;
			}
			return defaultExceptionMessage;
		}

		internal static string FormatWebFormField(string fieldName, string fieldValue)
		{
			return string.Format(CultureInfo.InvariantCulture, "{0}={1}", new object[]
			{
				fieldName,
				fieldValue
			});
		}

		internal static string FormatLatitude(double latitude)
		{
			return string.Format(CultureInfo.InvariantCulture, "{0}° {1}", new object[]
			{
				Math.Abs(latitude),
				(latitude < 0.0) ? "S" : "N"
			});
		}

		internal static string FormatLongitude(double longitude)
		{
			return string.Format(CultureInfo.InvariantCulture, "{0}° {1}", new object[]
			{
				Math.Abs(longitude),
				(longitude < 0.0) ? "W" : "E"
			});
		}

		internal static string FormatEntityId(object entityId)
		{
			return string.Format(CultureInfo.InvariantCulture, "ei:{0}", new object[]
			{
				entityId
			});
		}

		internal const int MaxLocationsResponseLength = 70000;

		internal const int MaxForecastResponseLength = 300000;

		internal const string LocationSearchStringFieldName = "weasearchstr";

		internal const string LocationStringFieldName = "wealocations";

		internal const string SourceFieldName = "src";

		internal const string CultureFieldName = "culture";

		internal const string OutputViewFieldName = "outputview";

		internal const string QueryStringFieldSeparator = "&";

		internal const string QueryStringFieldFormat = "{0}={1}";
	}
}
