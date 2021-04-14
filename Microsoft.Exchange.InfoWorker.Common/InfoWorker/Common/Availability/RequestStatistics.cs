using System;
using System.Globalization;

namespace Microsoft.Exchange.InfoWorker.Common.Availability
{
	internal sealed class RequestStatistics
	{
		public static RequestStatistics Create(RequestStatisticsType tag, long timeTaken)
		{
			return new RequestStatistics
			{
				info = RequestStatistics.Info.TimeTaken,
				Tag = tag,
				TimeTaken = timeTaken
			};
		}

		public static RequestStatistics Create(RequestStatisticsType tag, long timeTaken, string destination)
		{
			return new RequestStatistics
			{
				info = (RequestStatistics.Info.TimeTaken | RequestStatistics.Info.Destination),
				Tag = tag,
				TimeTaken = timeTaken,
				Destination = destination
			};
		}

		public static RequestStatistics Create(RequestStatisticsType tag, long timeTaken, int requestCount)
		{
			return new RequestStatistics
			{
				info = (RequestStatistics.Info.TimeTaken | RequestStatistics.Info.RequestCount),
				Tag = tag,
				TimeTaken = timeTaken,
				RequestCount = requestCount
			};
		}

		public static RequestStatistics Create(RequestStatisticsType tag, long timeTaken, int requestCount, string destination)
		{
			return new RequestStatistics
			{
				info = (RequestStatistics.Info.TimeTaken | RequestStatistics.Info.RequestCount | RequestStatistics.Info.Destination),
				Tag = tag,
				TimeTaken = timeTaken,
				RequestCount = requestCount,
				Destination = destination
			};
		}

		public RequestStatisticsType Tag { get; private set; }

		public long TimeTaken { get; private set; }

		public int RequestCount { get; private set; }

		public string Destination { get; private set; }

		public void Log(RequestLogger requestLogger)
		{
			if (requestLogger == null)
			{
				throw new ArgumentNullException("requestLogger");
			}
			if (this.info == RequestStatistics.Info.TimeTaken)
			{
				requestLogger.AppendToLog<long>(this.Tag.Name, this.TimeTaken);
				return;
			}
			if (this.info == RequestStatistics.Info.RequestCount)
			{
				requestLogger.AppendToLog<int>(this.Tag.Name, this.RequestCount);
				return;
			}
			if ((this.info & RequestStatistics.Info.TimeTaken) != (RequestStatistics.Info)0)
			{
				requestLogger.AppendToLog<long>(this.Tag.Name + ".T", this.TimeTaken);
			}
			if ((this.info & RequestStatistics.Info.RequestCount) != (RequestStatistics.Info)0)
			{
				requestLogger.AppendToLog<int>(this.Tag.Name + ".R", this.RequestCount);
			}
			if ((this.info & RequestStatistics.Info.Destination) != (RequestStatistics.Info)0)
			{
				requestLogger.AppendToLog<string>(this.Tag.Name + ".D", this.Destination);
			}
		}

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "{0}:TimeTaken={1}, RequestCount={2}, Destination={3}", new object[]
			{
				this.Tag.Name,
				this.TimeTaken,
				this.RequestCount,
				this.Destination
			});
		}

		private RequestStatistics()
		{
		}

		private RequestStatistics.Info info;

		[Flags]
		private enum Info
		{
			TimeTaken = 1,
			RequestCount = 2,
			Destination = 4
		}
	}
}
