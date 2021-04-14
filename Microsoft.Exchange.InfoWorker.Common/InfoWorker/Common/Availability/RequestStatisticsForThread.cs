using System;
using Microsoft.Exchange.Win32;

namespace Microsoft.Exchange.InfoWorker.Common.Availability
{
	internal sealed class RequestStatisticsForThread
	{
		public static RequestStatisticsForThread Begin()
		{
			return new RequestStatisticsForThread
			{
				begin = ThreadTimes.GetFromCurrentThread()
			};
		}

		public RequestStatistics End(RequestStatisticsType tag)
		{
			return this.End(tag, null);
		}

		public RequestStatistics End(RequestStatisticsType tag, string destination)
		{
			ThreadTimes fromCurrentThread = ThreadTimes.GetFromCurrentThread();
			if (this.begin == null || fromCurrentThread == null)
			{
				return null;
			}
			long timeTaken = (long)(fromCurrentThread.Kernel.TotalMilliseconds - this.begin.Kernel.TotalMilliseconds) + (long)(fromCurrentThread.User.TotalMilliseconds - this.begin.User.TotalMilliseconds);
			if (destination == null)
			{
				return RequestStatistics.Create(tag, timeTaken);
			}
			return RequestStatistics.Create(tag, timeTaken, destination);
		}

		private ThreadTimes begin;
	}
}
