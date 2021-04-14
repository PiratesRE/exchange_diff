using System;
using System.Globalization;
using System.IO;
using System.Reflection;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Performance;

namespace Microsoft.Exchange.Data.ApplicationLogic.UserPhotos
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class PhotoGarbageCollectionLogger : DisposeTrackableBase, ITracer, IPerformanceDataLogger
	{
		public PhotoGarbageCollectionLogger(PhotosConfiguration configuration, string logFileName)
		{
			ArgumentValidator.ThrowIfNull("configuration", configuration);
			ArgumentValidator.ThrowIfNullOrEmpty("logFileName", logFileName);
			this.build = ExchangeSetupContext.InstalledVersion.ToString();
			this.log = new Log(logFileName, new LogHeaderFormatter(PhotoGarbageCollectionLogger.Schema), "PhotoGarbageCollector");
			this.log.Configure(configuration.GarbageCollectionLoggingPath, configuration.GarbageCollectionLogFileMaxAge, configuration.GarbageCollectionLogDirectoryMaxSize, configuration.GarbageCollectionLogFileMaxSize);
		}

		public void TraceDebug<T0>(long id, string formatString, T0 arg0)
		{
			this.LogDebug(id, string.Format(formatString, arg0));
		}

		public void TraceDebug<T0, T1>(long id, string formatString, T0 arg0, T1 arg1)
		{
			this.LogDebug(id, string.Format(formatString, arg0, arg1));
		}

		public void TraceDebug<T0, T1, T2>(long id, string formatString, T0 arg0, T1 arg1, T2 arg2)
		{
			this.LogDebug(id, string.Format(formatString, arg0, arg1, arg2));
		}

		public void TraceDebug(long id, string formatString, params object[] args)
		{
			this.LogDebug(id, string.Format(formatString, args));
		}

		public void TraceDebug(long id, string message)
		{
			this.LogDebug(id, message);
		}

		public void TraceWarning<T0>(long id, string formatString, T0 arg0)
		{
			this.LogWarning(id, string.Format(formatString, arg0));
		}

		public void TraceWarning(long id, string message)
		{
			this.LogWarning(id, message);
		}

		public void TraceWarning(long id, string formatString, params object[] args)
		{
			this.LogWarning(id, string.Format(formatString, args));
		}

		public void TraceError(long id, string message)
		{
			this.LogError(id, message);
		}

		public void TraceError(long id, string formatString, params object[] args)
		{
			this.LogError(id, string.Format(formatString, args));
		}

		public void TraceError<T0>(long id, string formatString, T0 arg0)
		{
			this.LogError(id, string.Format(formatString, arg0));
		}

		public void TraceError<T0, T1>(long id, string formatString, T0 arg0, T1 arg1)
		{
			this.LogError(id, string.Format(formatString, arg0, arg1));
		}

		public void TraceError<T0, T1, T2>(long id, string formatString, T0 arg0, T1 arg1, T2 arg2)
		{
			this.LogError(id, string.Format(formatString, arg0, arg1, arg2));
		}

		public void TracePerformance(long id, string message)
		{
			this.LogPerformance(id, message);
		}

		public void TracePerformance(long id, string formatString, params object[] args)
		{
			this.LogPerformance(id, string.Format(formatString, args));
		}

		public void TracePerformance<T0>(long id, string formatString, T0 arg0)
		{
			this.LogPerformance(id, string.Format(formatString, arg0));
		}

		public void TracePerformance<T0, T1>(long id, string formatString, T0 arg0, T1 arg1)
		{
			this.LogPerformance(id, string.Format(formatString, arg0, arg1));
		}

		public void TracePerformance<T0, T1, T2>(long id, string formatString, T0 arg0, T1 arg1, T2 arg2)
		{
			this.LogPerformance(id, string.Format(formatString, arg0, arg1, arg2));
		}

		public void Dump(TextWriter writer, bool addHeader, bool verbose)
		{
			throw new NotSupportedException();
		}

		public ITracer Compose(ITracer other)
		{
			return new CompositeTracer(this, other);
		}

		public bool IsTraceEnabled(TraceType traceType)
		{
			return true;
		}

		public void Log(string marker, string counter, TimeSpan dataPoint)
		{
			this.Log(0L, "PERFORMANCE", string.Empty, marker, counter, dataPoint.TotalMilliseconds.ToString(CultureInfo.InvariantCulture));
		}

		public void Log(string marker, string counter, uint dataPoint)
		{
			this.Log(0L, "PERFORMANCE", string.Empty, marker, counter, dataPoint.ToString(CultureInfo.InvariantCulture));
		}

		public void Log(string marker, string counter, string dataPoint)
		{
			this.Log(0L, "PERFORMANCE", string.Empty, marker, counter, dataPoint);
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<PhotoGarbageCollectionLogger>(this);
		}

		protected override void InternalDispose(bool disposing)
		{
			if (!disposing)
			{
				return;
			}
			if (this.log != null)
			{
				this.log.Close();
				this.log = null;
			}
		}

		private void LogDebug(long sessionId, string message)
		{
			this.Log(sessionId, "DEBUG", message);
		}

		private void LogError(long sessionId, string message)
		{
			this.Log(sessionId, "ERROR", message);
		}

		private void LogWarning(long sessionId, string message)
		{
			this.Log(sessionId, "WARNING", message);
		}

		private void LogPerformance(long sessionId, string message)
		{
			this.Log(sessionId, "PERFORMANCE", message);
		}

		private void Log(long sessionId, string eventType, string message, string perfMarker, string perfCounter, string perfDatapoint)
		{
			LogRowFormatter logRowFormatter = new LogRowFormatter(PhotoGarbageCollectionLogger.Schema, true);
			logRowFormatter[1] = this.build;
			logRowFormatter[2] = sessionId.ToString("X");
			logRowFormatter[3] = Environment.MachineName;
			logRowFormatter[4] = eventType;
			logRowFormatter[5] = message;
			logRowFormatter[6] = perfMarker;
			logRowFormatter[7] = perfCounter;
			logRowFormatter[8] = perfDatapoint;
			this.log.Append(logRowFormatter, 0);
		}

		private void Log(long sessionId, string eventType, string message)
		{
			this.Log(sessionId, eventType, message, string.Empty, string.Empty, string.Empty);
		}

		private const string LogComponentName = "PhotoGarbageCollector";

		private const long NoSessionId = 0L;

		private static readonly string[] LogColumns = new string[]
		{
			"Time",
			"Build",
			"SessionId",
			"Server",
			"EventType",
			"Message",
			"PerfMarker",
			"PerfCounter",
			"PerfDatapoint"
		};

		private static readonly LogSchema Schema = new LogSchema("Microsoft Exchange Server", Assembly.GetExecutingAssembly().GetName().Version.ToString(), "Photo Garbage Collection log", PhotoGarbageCollectionLogger.LogColumns);

		private readonly string build;

		private Log log;

		private static class LogColumnIndices
		{
			public const int Time = 0;

			public const int Build = 1;

			public const int SessionId = 2;

			public const int Server = 3;

			public const int EventType = 4;

			public const int Message = 5;

			public const int PerfMarker = 6;

			public const int PerfCounter = 7;

			public const int PerfDatapoint = 8;
		}

		private static class EventTypes
		{
			public const string Debug = "DEBUG";

			public const string Error = "ERROR";

			public const string Warning = "WARNING";

			public const string Performance = "PERFORMANCE";
		}
	}
}
