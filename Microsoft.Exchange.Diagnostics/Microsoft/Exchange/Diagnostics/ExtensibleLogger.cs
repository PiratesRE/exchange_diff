using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Exchange.Diagnostics.WorkloadManagement;

namespace Microsoft.Exchange.Diagnostics
{
	internal class ExtensibleLogger : DisposeTrackableBase, IExtensibleLogger, IWorkloadLogger
	{
		public ExtensibleLogger(ILogConfiguration configuration) : this(configuration, LogRowFormatter.DefaultEscapeLineBreaks)
		{
		}

		public ExtensibleLogger(ILogConfiguration configuration, bool escapeLineBreaks)
		{
			if (configuration == null)
			{
				throw new ArgumentNullException("configuration");
			}
			this.configuration = configuration;
			this.escapeLineBreaks = escapeLineBreaks;
			try
			{
				this.buildNumber = ExchangeSetupContext.InstalledVersion.ToString();
			}
			catch (SetupVersionInformationCorruptException)
			{
				this.buildNumber = string.Empty;
			}
			string version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
			this.logSchema = new LogSchema("Microsoft Exchange Server", version, this.configuration.LogType, ExtensibleLogger.Fields);
			this.log = new Log(this.configuration.LogPrefix, new LogHeaderFormatter(this.logSchema), this.configuration.LogComponent);
			this.log.Configure(this.configuration.LogPath, this.configuration.MaxLogAge, this.configuration.MaxLogDirectorySizeInBytes, this.configuration.MaxLogFileSizeInBytes);
			if (this.configuration.IsActivityEventHandler)
			{
				ActivityContext.OnActivityEvent += this.OnActivityContextEvent;
			}
			ActivityContext.RegisterMetadata(typeof(ExtensibleLoggerMetadata));
		}

		protected ILogConfiguration Configuration
		{
			get
			{
				return this.configuration;
			}
		}

		public static string FormatPIIValue(string value)
		{
			if (string.IsNullOrEmpty(value))
			{
				return string.Empty;
			}
			return "<PII>" + value + "</PII>";
		}

		public void LogEvent(ILogEvent logEvent)
		{
			if (logEvent == null)
			{
				throw new ArgumentNullException("logEvent");
			}
			if (!this.configuration.IsLoggingEnabled)
			{
				return;
			}
			this.LogRow(logEvent.EventId, this.GetEventData(logEvent));
		}

		public void LogEvent(IEnumerable<ILogEvent> logEvents)
		{
			if (logEvents == null)
			{
				throw new ArgumentNullException("logEvents");
			}
			if (!this.configuration.IsLoggingEnabled)
			{
				return;
			}
			List<LogRowFormatter> list = new List<LogRowFormatter>(64);
			foreach (ILogEvent logEvent in logEvents)
			{
				list.Add(this.CreateRow(logEvent.EventId, this.GetEventData(logEvent)));
			}
			this.log.Append(list, 0);
		}

		public void LogActivityEvent(IActivityScope activityScope, ActivityEventType eventType)
		{
			if (activityScope == null)
			{
				throw new ArgumentNullException("activityScope");
			}
			if (!this.configuration.IsLoggingEnabled || !this.IsInterestingEvent(activityScope, eventType))
			{
				return;
			}
			ILogEvent logEvent = activityScope.UserState as ILogEvent;
			ICollection<KeyValuePair<string, object>> collection;
			string text;
			if (logEvent != null)
			{
				collection = this.GetEventData(logEvent);
				text = logEvent.EventId;
			}
			else
			{
				collection = new List<KeyValuePair<string, object>>(0);
				if (activityScope.ActivityType == ActivityType.Global)
				{
					text = "GlobalActivity";
				}
				else
				{
					text = activityScope.GetProperty(ExtensibleLoggerMetadata.EventId);
				}
			}
			if (string.IsNullOrEmpty(text))
			{
				text = "<null>";
			}
			ICollection<KeyValuePair<string, object>> componentSpecificData = this.GetComponentSpecificData(activityScope, text);
			List<KeyValuePair<string, object>> list = this.FormatWlmActivity(activityScope);
			int capacity = collection.Count + componentSpecificData.Count + list.Count + 2;
			List<KeyValuePair<string, object>> list2 = new List<KeyValuePair<string, object>>(capacity);
			list2.AddRange(collection);
			list2.AddRange(componentSpecificData);
			list2.Add(new KeyValuePair<string, object>("Bld", this.buildNumber));
			list2.Add(new KeyValuePair<string, object>("ActID", activityScope.ActivityId));
			list2.AddRange(list);
			this.LogRow(text, list2);
		}

		protected static void CopyPIIProperty(IActivityScope source, Dictionary<string, object> target, Enum sourceKey, string targetKey)
		{
			ExtensibleLogger.CopyProperty(source, target, sourceKey, targetKey, true);
		}

		protected static void CopyProperty(IActivityScope source, Dictionary<string, object> target, Enum sourceKey, string targetKey)
		{
			ExtensibleLogger.CopyProperty(source, target, sourceKey, targetKey, false);
		}

		protected static void CopyProperties(IActivityScope source, Dictionary<string, object> target, IEnumerable<KeyValuePair<Enum, string>> enumToKeyMappings)
		{
			foreach (KeyValuePair<Enum, string> keyValuePair in enumToKeyMappings)
			{
				ExtensibleLogger.CopyProperty(source, target, keyValuePair.Key, keyValuePair.Value, false);
			}
		}

		protected virtual ICollection<KeyValuePair<string, object>> GetEventData(ILogEvent logEvent)
		{
			return logEvent.GetEventData();
		}

		protected virtual List<KeyValuePair<string, object>> FormatWlmActivity(IActivityScope activityScope)
		{
			List<KeyValuePair<string, object>> list = null;
			if (activityScope != null)
			{
				list = activityScope.GetFormattableStatistics();
				List<KeyValuePair<string, object>> formattableMetadata = activityScope.GetFormattableMetadata();
				if (formattableMetadata != null)
				{
					foreach (KeyValuePair<string, object> item in formattableMetadata)
					{
						if (item.Key.StartsWith("WLM"))
						{
							list.Add(item);
						}
					}
				}
			}
			return list;
		}

		protected virtual ICollection<KeyValuePair<string, object>> GetComponentSpecificData(IActivityScope activityScope, string eventId)
		{
			return new KeyValuePair<string, object>[0];
		}

		protected virtual bool IsInterestingEvent(IActivityScope activityScope, ActivityEventType eventType)
		{
			return eventType == ActivityEventType.EndActivity;
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<ExtensibleLogger>(this);
		}

		protected override void InternalDispose(bool disposing)
		{
			if (disposing)
			{
				ActivityContext.OnActivityEvent -= this.OnActivityContextEvent;
				this.log.Flush();
				this.log.Close();
			}
		}

		private static void CopyProperty(IActivityScope source, Dictionary<string, object> target, Enum sourceKey, string targetKey, bool isPII)
		{
			string property = source.GetProperty(sourceKey);
			if (property == null)
			{
				return;
			}
			if (target.ContainsKey(targetKey))
			{
				throw new ArgumentException(string.Format("targetKey '{0}' is already being used by another property: {1}", targetKey, sourceKey));
			}
			target.Add(targetKey, isPII ? ExtensibleLogger.FormatPIIValue(property) : property);
		}

		private LogRowFormatter CreateRow(string eventId, IEnumerable<KeyValuePair<string, object>> eventData)
		{
			if (string.IsNullOrEmpty(eventId))
			{
				throw new ArgumentException("eventId cannot be null or empty");
			}
			LogRowFormatter logRowFormatter = new LogRowFormatter(this.logSchema, this.escapeLineBreaks);
			logRowFormatter[1] = ExtensibleLogger.MachineName;
			logRowFormatter[2] = eventId;
			logRowFormatter[3] = eventData;
			return logRowFormatter;
		}

		private void LogRow(string eventId, IEnumerable<KeyValuePair<string, object>> eventData)
		{
			LogRowFormatter row = this.CreateRow(eventId, eventData);
			this.log.Append(row, 0);
		}

		private void OnActivityContextEvent(object sender, ActivityEventArgs args)
		{
			this.LogActivityEvent((IActivityScope)sender, args.ActivityEventType);
		}

		internal const string CorrelationIdKey = "CorrelationID";

		private const string SoftwareName = "Microsoft Exchange Server";

		private const string ActivityIdKey = "ActID";

		private const string Build = "Bld";

		private static readonly string MachineName = Environment.MachineName;

		private static readonly string[] Fields = new string[]
		{
			"TimeStamp",
			"ServerName",
			"EventId",
			"EventData"
		};

		private readonly LogSchema logSchema;

		private readonly Log log;

		private readonly ILogConfiguration configuration;

		private readonly string buildNumber;

		private readonly bool escapeLineBreaks;

		private enum Field
		{
			TimeStamp,
			ServerName,
			EventId,
			EventData
		}
	}
}
