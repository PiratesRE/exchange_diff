using System;
using System.Collections.Generic;
using System.Text;
using System.Web;

namespace Microsoft.Exchange.InfoWorker.Common.Availability
{
	internal sealed class RequestLogger
	{
		public RequestLogger()
		{
			this.items = new Dictionary<RequestStatisticsType, RequestStatistics>();
			this.capturedTime = DateTime.UtcNow;
			this.capturedTimeList = new List<RequestLogger.TagTimePair>();
			this.logData = new StringBuilder(1024);
			this.exceptionStatistics = new Dictionary<string, int>();
		}

		public StringBuilder LogData
		{
			get
			{
				return this.logData;
			}
		}

		public List<string> ErrorData
		{
			get
			{
				return this.errorData;
			}
		}

		public Dictionary<string, int> ExceptionData
		{
			get
			{
				return this.exceptionStatistics;
			}
		}

		public void Add(RequestStatistics requestStatistics)
		{
			lock (this.items)
			{
				RequestStatistics requestStatistics2;
				if (this.items.TryGetValue(requestStatistics.Tag, out requestStatistics2))
				{
					if (requestStatistics.TimeTaken > requestStatistics2.TimeTaken)
					{
						this.items[requestStatistics.Tag] = requestStatistics;
					}
				}
				else
				{
					this.items[requestStatistics.Tag] = requestStatistics;
				}
			}
		}

		public void CaptureRequestStage(string tag)
		{
			DateTime utcNow = DateTime.UtcNow;
			this.capturedTimeList.Add(new RequestLogger.TagTimePair
			{
				Tag = tag,
				TimeTaken = (long)(utcNow - this.capturedTime).TotalMilliseconds
			});
			this.capturedTime = utcNow;
		}

		public void Log()
		{
			this.CaptureRequestStage("PostQuery");
			lock (this.items)
			{
				foreach (RequestStatistics requestStatistics in this.items.Values)
				{
					requestStatistics.Log(this);
				}
			}
			foreach (RequestLogger.TagTimePair tagTimePair in this.capturedTimeList)
			{
				this.AppendToLog<long>(tagTimePair.Tag, tagTimePair.TimeTaken);
			}
		}

		public void AppendToLog<T>(string key, T value)
		{
			this.logData.AppendFormat("{0}={1};", key, value);
		}

		public void StartLog()
		{
			this.logData.Append("[AS-Start-");
		}

		public void EndLog()
		{
			this.logData.Append("-AS-END]");
		}

		public void LogToResponse(HttpResponse httpResponse)
		{
			if (httpResponse != null)
			{
				int num = 0;
				int i = 80;
				int length = this.logData.Length;
				while (i < length)
				{
					httpResponse.AppendToLog(this.logData.ToString(num, 80));
					num = i;
					i += 80;
				}
				if (num < length)
				{
					httpResponse.AppendToLog(this.logData.ToString(num, length - num));
				}
			}
		}

		public void CalculateQueryStatistics(FreeBusyQuery[] queries)
		{
			if (queries == null)
			{
				return;
			}
			QueryLogData queryLogData = new QueryLogData();
			for (int i = 0; i < queries.Length; i++)
			{
				FreeBusyQueryResult result = queries[i].Result;
				if (result != null)
				{
					if (result.ExceptionInfo != null)
					{
						this.UpdateExceptionStatictics(result.ExceptionInfo);
						queryLogData.Add(queries[i], true);
					}
					else
					{
						queryLogData.Add(queries[i], false);
					}
				}
				else
				{
					queryLogData.Add(queries[i], false);
				}
			}
			this.logData.Append(queryLogData.GetSucceededQueryLogData());
			this.errorData = queryLogData.GetFailedQueryLogData();
		}

		private void UpdateExceptionStatictics(Exception ex)
		{
			while (ex.InnerException != null)
			{
				ex = ex.InnerException;
			}
			string name = ex.GetType().Name;
			if (!this.exceptionStatistics.ContainsKey(name))
			{
				this.exceptionStatistics.Add(name, 1);
				return;
			}
			Dictionary<string, int> dictionary;
			string key;
			(dictionary = this.exceptionStatistics)[key = name] = dictionary[key] + 1;
		}

		private const string START = "[AS-Start-";

		private const string END = "-AS-END]";

		private const int DefaultCapacity = 1024;

		private const int MaxLogAppendStringLength = 80;

		private Dictionary<RequestStatisticsType, RequestStatistics> items;

		private DateTime capturedTime;

		private List<RequestLogger.TagTimePair> capturedTimeList;

		private StringBuilder logData;

		private List<string> errorData;

		private Dictionary<string, int> exceptionStatistics;

		private class TagTimePair
		{
			public string Tag;

			public long TimeTaken;
		}
	}
}
