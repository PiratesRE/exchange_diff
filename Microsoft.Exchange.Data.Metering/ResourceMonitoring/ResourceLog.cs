using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Metering.ResourceMonitoring
{
	internal sealed class ResourceLog
	{
		public ResourceLog(bool enabled, string logSource, string logDirectory, TimeSpan maxAge, TimeSpan streamFlushInterval, TimeSpan backgroundWriteInterval, long maxDirectorySize = 0L, long maxLogFileSize = 0L, int bufferSize = 0)
		{
			this.enabled = enabled;
			if (enabled)
			{
				ArgumentValidator.ThrowIfNullOrEmpty("logDirectory", logDirectory);
				ArgumentValidator.ThrowIfNullOrEmpty("logSource", logSource);
				ArgumentValidator.ThrowIfInvalidValue<TimeSpan>("maxAge", maxAge, (TimeSpan age) => age > TimeSpan.Zero);
				ArgumentValidator.ThrowIfInvalidValue<TimeSpan>("streamFlushInterval", streamFlushInterval, (TimeSpan interval) => interval > TimeSpan.Zero);
				ArgumentValidator.ThrowIfInvalidValue<TimeSpan>("backgroundWriteInterval", backgroundWriteInterval, (TimeSpan interval) => interval > TimeSpan.Zero);
				ArgumentValidator.ThrowIfInvalidValue<long>("maxDirectorySize", maxDirectorySize, (long number) => number >= 0L);
				ArgumentValidator.ThrowIfInvalidValue<long>("maxLogFileSize", maxLogFileSize, (long number) => number >= 0L);
				ArgumentValidator.ThrowIfNegative("bufferSize", bufferSize);
				this.logSource = logSource;
				this.schema = new LogSchema("Microsoft Exchange Server", Assembly.GetExecutingAssembly().GetName().Version.ToString(), "Transport Resource Log", ResourceLog.Fields);
				this.asyncLogWrapper = AsyncLogWrapperFactory.CreateAsyncLogWrapper("ResourceLog_" + Process.GetCurrentProcess().ProcessName, new LogHeaderFormatter(this.schema), "ResourceLog");
				this.asyncLogWrapper.Configure(logDirectory, maxAge, maxDirectorySize, maxLogFileSize, bufferSize, streamFlushInterval, backgroundWriteInterval);
			}
		}

		public IAsyncLogWrapper AsyncLogWrapper
		{
			get
			{
				return this.asyncLogWrapper;
			}
		}

		public void LogResourceUsePeriodic(ResourceUse resourceUse, Dictionary<string, object> customData)
		{
			this.LogResourceUse(0, resourceUse, customData);
		}

		public void LogResourceUseChange(ResourceUse resourceUse, Dictionary<string, object> customData)
		{
			this.LogResourceUse(1, resourceUse, customData);
		}

		private static string[] InitializeHeaders()
		{
			string[] array = new string[Enum.GetValues(typeof(ResourceLog.Field)).Length];
			array[0] = "DateTime";
			array[1] = "EventId";
			array[2] = "LogSource";
			array[3] = "ResourceId";
			array[4] = "OldValue";
			array[5] = "NewValue";
			array[6] = "CustomData";
			return array;
		}

		private void LogResourceUse(int eventId, ResourceUse resourceUse, Dictionary<string, object> customData)
		{
			if (!this.enabled)
			{
				return;
			}
			LogRowFormatter logRowFormatter = new LogRowFormatter(this.schema);
			logRowFormatter[1] = eventId;
			logRowFormatter[2] = this.logSource;
			logRowFormatter[3] = resourceUse.Resource.ToString();
			logRowFormatter[4] = resourceUse.PreviousUseLevel.ToString();
			logRowFormatter[5] = resourceUse.CurrentUseLevel.ToString();
			if (customData != null)
			{
				logRowFormatter[6] = customData;
			}
			this.Append(logRowFormatter);
		}

		private void Append(LogRowFormatter row)
		{
			this.asyncLogWrapper.Append(row, 0);
		}

		private const string LogComponentName = "ResourceLog";

		private const string LogFilePrefix = "ResourceLog";

		private static readonly string[] Fields = ResourceLog.InitializeHeaders();

		private readonly IAsyncLogWrapper asyncLogWrapper;

		private readonly LogSchema schema;

		private readonly bool enabled;

		private readonly string logSource;

		private enum Field
		{
			Time,
			EventId,
			LogSource,
			ResourceId,
			OldValue,
			NewValue,
			CustomData
		}

		private enum ResourceLogEvent
		{
			Periodic,
			Change
		}
	}
}
