using System;

namespace Microsoft.Exchange.InfoWorker.Common.Availability
{
	internal static class ClientStatisticsReporter
	{
		internal static void ReportRequest(string messageId, DateTime requestTime, int responseTime, int responseSize, int responseCode, int[] recipientErrorCodes)
		{
			TimeSpan t = DateTime.UtcNow.Subtract(requestTime);
			if (t.Ticks < 0L)
			{
				t = DateTime.UtcNow.AddHours(24.0).Subtract(requestTime);
				if (t.Ticks < 0L)
				{
					return;
				}
			}
			if (t < TimeSpan.FromHours(24.0))
			{
				int num = responseCode;
				int num2 = 200;
				if (num == num2 && recipientErrorCodes != null)
				{
					for (int i = 0; i < recipientErrorCodes.Length; i++)
					{
						if (recipientErrorCodes[i] > 0 && ClientStatisticsReporter.IsServerError(recipientErrorCodes[i]))
						{
							num = recipientErrorCodes[i];
							break;
						}
					}
				}
				if (num != num2)
				{
					PerformanceCounters.FailedClientReportedRequestsTotal.Increment();
					ErrorConstants errorConstants = (ErrorConstants)num;
					if (errorConstants == ErrorConstants.ServiceDiscoveryFailed)
					{
						PerformanceCounters.FailedClientRequestsNoASUrl.Increment();
						return;
					}
					if (errorConstants == ErrorConstants.TimeoutExpired)
					{
						PerformanceCounters.FailedClientRequestsTimeouts.Increment();
						return;
					}
					PerformanceCounters.FailedClientRequestsPartialOrOther.Increment();
					return;
				}
				else
				{
					PerformanceCounters.PastTotalClientSuccessRequests.Increment();
					int num3 = (int)Math.Round((double)(responseTime / 1000));
					if (num3 <= 5)
					{
						PerformanceCounters.PastClientRequestsUnder5.Increment();
					}
					if (num3 <= 10)
					{
						PerformanceCounters.PastClientRequestsUnder10.Increment();
					}
					if (num3 <= 20)
					{
						PerformanceCounters.PastClientRequestsUnder20.Increment();
					}
					if (num3 > 20)
					{
						PerformanceCounters.PastClientRequestsOver20.Increment();
					}
				}
			}
		}

		private static bool IsServerError(int errorCode)
		{
			if (errorCode <= 5012)
			{
				switch (errorCode)
				{
				case 5001:
				case 5002:
				case 5003:
					break;
				default:
					if (errorCode != 5009 && errorCode != 5012)
					{
						goto IL_7C;
					}
					break;
				}
			}
			else
			{
				switch (errorCode)
				{
				case 5026:
				case 5028:
				case 5029:
				case 5032:
					break;
				case 5027:
				case 5030:
				case 5031:
					goto IL_7C;
				default:
					switch (errorCode)
					{
					case 5036:
					case 5037:
						break;
					default:
						if (errorCode != 5043)
						{
							goto IL_7C;
						}
						break;
					}
					break;
				}
			}
			return false;
			IL_7C:
			return true;
		}
	}
}
