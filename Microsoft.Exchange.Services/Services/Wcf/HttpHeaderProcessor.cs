using System;
using System.Collections.Generic;
using System.Web;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Wcf
{
	internal static class HttpHeaderProcessor
	{
		internal static List<ClientStatistics> ProcessClientStatisticsHttpHeader(HttpContext httpContext)
		{
			string text = httpContext.Request.Headers[HttpHeaderProcessor.RequestStatisticsKey];
			if (string.IsNullOrEmpty(text))
			{
				return null;
			}
			string[] array = text.Split(HttpHeaderProcessor.RequestDelimiter, StringSplitOptions.RemoveEmptyEntries);
			if (array == null || array.Length <= 0 || array.Length > 200)
			{
				ExTraceGlobals.CommonAlgorithmTracer.TraceDebug(0L, "[ProcessClientStatisticsHttpHeader] No requests received.");
				return null;
			}
			int num = (array.Length > 200) ? 200 : array.Length;
			List<ClientStatistics> list = new List<ClientStatistics>(num);
			for (int i = 0; i < num; i++)
			{
				string request = array[i];
				ClientStatistics clientStatistics = HttpHeaderProcessor.ParseRequestEntries(request, i);
				if (clientStatistics != null && clientStatistics.IsValid())
				{
					list.Add(clientStatistics);
				}
				else
				{
					ExTraceGlobals.CommonAlgorithmTracer.TraceDebug(0L, "[ProcessClientStatisticsHttpHeader] Client request information is invalid, skipping.");
				}
			}
			return list;
		}

		private static ClientStatistics ParseRequestEntries(string request, int requestIndex)
		{
			string[] array = request.Split(HttpHeaderProcessor.EntryDelimiter, StringSplitOptions.RemoveEmptyEntries);
			if (array == null || array.Length <= 0)
			{
				ExTraceGlobals.CommonAlgorithmTracer.TraceDebug(0L, "[ProcessClientStatisticsHttpHeader] Got an empty entry");
				return null;
			}
			ClientStatistics clientStatistics = new ClientStatistics();
			int num = 0;
			for (int i = 0; i < array.Length; i++)
			{
				string[] array2 = array[i].Split(HttpHeaderProcessor.ValueDelimiter, StringSplitOptions.RemoveEmptyEntries);
				if (array2 == null || array2.Length != 2)
				{
					ExTraceGlobals.CommonAlgorithmTracer.TraceDebug(0L, "[ProcessClientStatisticsHttpHeader] Invalid Part");
					return null;
				}
				if (string.IsNullOrEmpty(array2[0]) || string.IsNullOrEmpty(array2[1]))
				{
					ExTraceGlobals.CommonAlgorithmTracer.TraceDebug(0L, "[ProcessClientStatisticsHttpHeader] Got empty parts");
					return null;
				}
				string a;
				if ((a = array2[0].Trim()) != null)
				{
					if (!(a == "MessageId"))
					{
						if (!(a == "ResponseTime"))
						{
							if (!(a == "RequestTime"))
							{
								if (!(a == "ResponseSize"))
								{
									if (!(a == "HttpResponseCode"))
									{
										if (a == "ErrorCode")
										{
											int num2;
											if (!int.TryParse(array2[1], out num2))
											{
												ExTraceGlobals.CommonAlgorithmTracer.TraceDebug(0L, "[ProcessClientStatisticsHttpHeader] Unable to parse ErrorCode part");
												return null;
											}
											if (clientStatistics.ErrorCode == null)
											{
												clientStatistics.ErrorCode = new int[20];
											}
											if (num < 20)
											{
												clientStatistics.ErrorCode[num] = num2;
												num++;
											}
										}
									}
									else
									{
										int httpResponseCode;
										if (!HttpHeaderProcessor.ValidateAndParseClientRequestHeaderPart("HttpResponseCode", clientStatistics.HttpResponseCode, array2[1], out httpResponseCode))
										{
											return null;
										}
										clientStatistics.HttpResponseCode = httpResponseCode;
									}
								}
								else
								{
									int responseSize;
									if (!HttpHeaderProcessor.ValidateAndParseClientRequestHeaderPart("ResponseSize", clientStatistics.ResponseSize, array2[1], out responseSize))
									{
										return null;
									}
									clientStatistics.ResponseSize = responseSize;
								}
							}
							else
							{
								DateTime requestTime = clientStatistics.RequestTime;
								DateTime requestTime2;
								if (!DateTime.TryParse(array2[1], out requestTime2))
								{
									return null;
								}
								clientStatistics.RequestTime = requestTime2;
							}
						}
						else
						{
							int responseTime;
							if (!HttpHeaderProcessor.ValidateAndParseClientRequestHeaderPart("ResponseTime", clientStatistics.ResponseTime, array2[1], out responseTime))
							{
								return null;
							}
							clientStatistics.ResponseTime = responseTime;
						}
					}
					else
					{
						if (!string.IsNullOrEmpty(clientStatistics.MessageId))
						{
							ExTraceGlobals.CommonAlgorithmTracer.TraceDebug(0L, "[ProcessClientStatisticsHttpHeader] Got a duplicate MessageId");
							return null;
						}
						clientStatistics.MessageId = array2[1].Trim();
					}
				}
				RequestDetailsLogger.Current.AppendClientStatistic(array2[0] + '_' + requestIndex, array2[1]);
			}
			return clientStatistics;
		}

		private static bool ValidateAndParseClientRequestHeaderPart(string partKey, int initialPartValue, string part, out int parsedPartValue)
		{
			parsedPartValue = -1;
			if (initialPartValue >= 0)
			{
				ExTraceGlobals.CommonAlgorithmTracer.TraceDebug(0L, "[ProcessClientStatisticsHttpHeader] Got a duplicate " + partKey);
				return false;
			}
			return int.TryParse(part, out parsedPartValue);
		}

		private const string MessageId = "MessageId";

		private const string RequestTime = "RequestTime";

		private const string ResponseTime = "ResponseTime";

		private const string ResponseSize = "ResponseSize";

		private const string HttpResponseCode = "HttpResponseCode";

		private const string PartialErrorCode = "ErrorCode";

		private static readonly string RequestStatisticsKey = "X-ClientStatistics";

		private static readonly char[] RequestDelimiter = new char[]
		{
			';'
		};

		private static readonly char[] EntryDelimiter = new char[]
		{
			','
		};

		private static readonly char[] ValueDelimiter = new char[]
		{
			'='
		};
	}
}
