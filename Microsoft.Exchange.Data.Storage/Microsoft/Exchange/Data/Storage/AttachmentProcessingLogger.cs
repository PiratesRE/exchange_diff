using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Net;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class AttachmentProcessingLogger : ExtensibleLogger
	{
		public AttachmentProcessingLogger() : base(new AttachmentProcessingLogger.AttachmentProcessingLogConfig())
		{
		}

		public static void Initialize()
		{
			if (AttachmentProcessingLogger.instance == null)
			{
				AttachmentProcessingLogger.instance = new AttachmentProcessingLogger();
			}
		}

		public static void LogEvent(string eventId, string key, object value)
		{
			if (AttachmentProcessingLogger.instance != null)
			{
				AttachmentProcessingLogger.AttachmentProcessingLogEvent logEvent = new AttachmentProcessingLogger.AttachmentProcessingLogEvent(eventId, key, value);
				AttachmentProcessingLogger.instance.LogEvent(logEvent);
			}
		}

		public static void LogEvent(string eventId, string key1, object value1, string key2, object value2)
		{
			if (AttachmentProcessingLogger.instance != null)
			{
				AttachmentProcessingLogger.AttachmentProcessingLogEvent logEvent = new AttachmentProcessingLogger.AttachmentProcessingLogEvent(eventId, key1, value1, key2, value2);
				AttachmentProcessingLogger.instance.LogEvent(logEvent);
			}
		}

		private static AttachmentProcessingLogger instance;

		private class AttachmentProcessingLogConfig : ILogConfiguration
		{
			public AttachmentProcessingLogConfig()
			{
				this.LogPath = AttachmentProcessingLogger.AttachmentProcessingLogConfig.DefaultLogFolderPath;
				this.MaxLogAge = TimeSpan.FromDays((double)AttachmentProcessingLogger.AttachmentProcessingLogConfig.MaxLogRetentionInDays.Value);
				this.MaxLogDirectorySizeInBytes = (long)(AttachmentProcessingLogger.AttachmentProcessingLogConfig.MaxLogDirectorySizeInGB.Value * 1024 * 1024 * 1024);
				this.MaxLogFileSizeInBytes = (long)(AttachmentProcessingLogger.AttachmentProcessingLogConfig.MaxLogFileSizeInMB.Value * 1024 * 1024);
			}

			public bool IsLoggingEnabled
			{
				get
				{
					return true;
				}
			}

			public bool IsActivityEventHandler
			{
				get
				{
					return false;
				}
			}

			public string LogPath { get; private set; }

			public string LogPrefix
			{
				get
				{
					return "XSOAttachmentProcessing_";
				}
			}

			public string LogComponent
			{
				get
				{
					return "XSOAttachmentProcessing_Logs";
				}
			}

			public string LogType
			{
				get
				{
					return "XSOAttachmentProcessing Logs";
				}
			}

			public long MaxLogDirectorySizeInBytes { get; private set; }

			public long MaxLogFileSizeInBytes { get; private set; }

			public TimeSpan MaxLogAge { get; private set; }

			private static string DefaultLogFolderPath
			{
				get
				{
					string result;
					try
					{
						result = Path.Combine(ExchangeSetupContext.InstallPath, "Logging\\XSOAttachmentProcessing");
					}
					catch (SetupVersionInformationCorruptException)
					{
						result = "C:\\Program Files\\Microsoft\\Exchange Server\\V15";
					}
					return result;
				}
			}

			public const string LogPrefixValue = "XSOAttachmentProcessing_";

			private const string LogTypeValue = "XSOAttachmentProcessing Logs";

			private const string LogComponentValue = "XSOAttachmentProcessing_Logs";

			private static readonly IntAppSettingsEntry MaxLogRetentionInDays = new IntAppSettingsEntry("MaxLogRetentionInDays", 30, null);

			private static readonly IntAppSettingsEntry MaxLogDirectorySizeInGB = new IntAppSettingsEntry("MaxLogDirectorySizeInGB", 1, null);

			private static readonly IntAppSettingsEntry MaxLogFileSizeInMB = new IntAppSettingsEntry("MaxLogFileSizeInMB", 10, null);
		}

		private class AttachmentProcessingLogEvent : ILogEvent
		{
			public AttachmentProcessingLogEvent(string eventId, string key, object value)
			{
				this.EventId = eventId;
				this.EventData = new List<KeyValuePair<string, object>>(1);
				this.EventData.Add(new KeyValuePair<string, object>(AttachmentProcessingLogger.AttachmentProcessingLogEvent.SanitizeKey(key), value));
			}

			public AttachmentProcessingLogEvent(string eventId, string key1, object value1, string key2, object value2)
			{
				this.EventId = eventId;
				this.EventData = new List<KeyValuePair<string, object>>(2);
				this.EventData.Add(new KeyValuePair<string, object>(AttachmentProcessingLogger.AttachmentProcessingLogEvent.SanitizeKey(key1), value1));
				this.EventData.Add(new KeyValuePair<string, object>(AttachmentProcessingLogger.AttachmentProcessingLogEvent.SanitizeKey(key2), value2));
			}

			public string EventId { get; private set; }

			public List<KeyValuePair<string, object>> EventData { get; private set; }

			public ICollection<KeyValuePair<string, object>> GetEventData()
			{
				return this.EventData;
			}

			private static string SanitizeKey(string key)
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.EnsureCapacity(key.Length);
				foreach (char c in key)
				{
					if (SpecialCharacters.IsValidKeyChar(c))
					{
						stringBuilder.Append(c);
					}
					else
					{
						stringBuilder.Append('.');
					}
				}
				return stringBuilder.ToString();
			}
		}
	}
}
