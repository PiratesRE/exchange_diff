using System;
using System.Globalization;
using System.Text;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Mapi;

namespace Microsoft.Exchange.AnchorService
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class AnchorLogger : DisposeTrackableBase, ILogger, IDisposeTrackable, IDisposable
	{
		public AnchorLogger(string applicationName, AnchorConfig config) : this(applicationName, config, AnchorLogger.AnchorEventLogger)
		{
		}

		public AnchorLogger(string applicationName, AnchorConfig config, ExEventLog eventLogger)
		{
			this.Config = config;
			this.log = new AnchorLog(applicationName, this.Config);
			this.EventLogger = eventLogger;
		}

		public AnchorLogger(string applicationName, AnchorConfig config, Trace trace, ExEventLog eventLogger)
		{
			this.Config = config;
			this.log = new AnchorLog(applicationName, this.Config, trace);
			this.EventLogger = eventLogger;
		}

		internal Action<string, MigrationEventType, object, string> InMemoryLogger { get; set; }

		internal ExEventLog EventLogger { get; private set; }

		private AnchorConfig Config { get; set; }

		public static string PropertyBagToString(PropertyBag bag)
		{
			AnchorUtil.ThrowOnNullArgument(bag, "bag");
			StringBuilder stringBuilder = new StringBuilder(bag.Count * 128);
			foreach (object obj in bag.Keys)
			{
				PropertyDefinition propertyDefinition = obj as PropertyDefinition;
				if (propertyDefinition != null)
				{
					stringBuilder.Append(string.Format(CultureInfo.InvariantCulture, "[{0}:{1}]", new object[]
					{
						propertyDefinition.Name,
						bag[propertyDefinition]
					}));
				}
			}
			return stringBuilder.ToString();
		}

		public static string GetDiagnosticInfo(Exception ex, string diagnosticInfo)
		{
			AnchorUtil.ThrowOnNullArgument(ex, "ex");
			Exception innerException = ex.InnerException;
			int num = 0;
			while (num < 10 && innerException != null)
			{
				MapiPermanentException ex2 = innerException as MapiPermanentException;
				MapiRetryableException ex3 = innerException as MapiRetryableException;
				string text = innerException.Message;
				if (ex2 != null)
				{
					text = ex2.DiagCtx.ToCompactString();
				}
				else if (ex3 != null)
				{
					text = ex3.DiagCtx.ToCompactString();
				}
				if (!string.IsNullOrEmpty(text))
				{
					if (diagnosticInfo == null)
					{
						diagnosticInfo = string.Format(CultureInfo.InvariantCulture, "InnerException:{0}:{1}", new object[]
						{
							innerException.GetType().Name,
							text
						});
					}
					else
					{
						diagnosticInfo = string.Format(CultureInfo.InvariantCulture, "{0} InnerException:{1}:{2}", new object[]
						{
							diagnosticInfo,
							innerException.GetType().Name,
							text
						});
					}
				}
				num++;
				innerException = innerException.InnerException;
			}
			string value = string.Empty;
			MigrationPermanentException ex4 = ex as MigrationPermanentException;
			MigrationTransientException ex5 = ex as MigrationTransientException;
			if (ex4 != null)
			{
				value = ex4.InternalError + ". ";
			}
			else if (ex5 != null)
			{
				value = ex5.InternalError + ". ";
			}
			StringBuilder stringBuilder = new StringBuilder(value);
			stringBuilder.Append(diagnosticInfo);
			stringBuilder.Append(ex.ToString());
			return AnchorLogger.SanitizeDiagnosticInfo(stringBuilder.ToString());
		}

		public static string SanitizeDiagnosticInfo(string diagnosticInfo)
		{
			AnchorUtil.ThrowOnNullOrEmptyArgument(diagnosticInfo, "diagnosticInfo");
			diagnosticInfo = diagnosticInfo.Replace("  ", " ");
			diagnosticInfo = diagnosticInfo.Replace("\n", " ");
			diagnosticInfo = diagnosticInfo.Replace("\r", " ");
			diagnosticInfo = diagnosticInfo.Replace("\t", " ");
			diagnosticInfo = diagnosticInfo.Replace("{", "[");
			diagnosticInfo = diagnosticInfo.Replace("}", "]");
			if (diagnosticInfo.Length > 16384)
			{
				return diagnosticInfo.Substring(0, 16381) + "...";
			}
			return diagnosticInfo;
		}

		public void LogEvent(MigrationEventType eventType, params string[] args)
		{
			this.LogEventInternal(eventType, null, true, null, args);
		}

		public void LogEvent(MigrationEventType eventType, ExEventLog.EventTuple eventId, params string[] args)
		{
			this.LogEventInternal(eventType, new ExEventLog.EventTuple?(eventId), true, null, args);
		}

		public void LogEvent(MigrationEventType eventType, Exception ex, params string[] args)
		{
			this.LogEventInternal(eventType, null, true, ex, args);
		}

		public void LogEvent(MigrationEventType eventType, ExEventLog.EventTuple eventId, Exception ex, params string[] args)
		{
			this.LogEventInternal(eventType, new ExEventLog.EventTuple?(eventId), true, ex, args);
		}

		public void LogTerseEvent(MigrationEventType eventType, params string[] args)
		{
			this.LogEventInternal(eventType, null, false, null, args);
		}

		public void LogTerseEvent(MigrationEventType eventType, ExEventLog.EventTuple eventId, params string[] args)
		{
			this.LogEventInternal(eventType, new ExEventLog.EventTuple?(eventId), false, null, args);
		}

		public void LogTerseEvent(MigrationEventType eventType, Exception ex, params string[] args)
		{
			this.LogEventInternal(eventType, null, false, ex, args);
		}

		public void LogTerseEvent(MigrationEventType eventType, ExEventLog.EventTuple eventId, Exception ex, params string[] args)
		{
			this.LogEventInternal(eventType, new ExEventLog.EventTuple?(eventId), false, ex, args);
		}

		public void Log(MigrationEventType eventType, Exception exception, string format)
		{
			this.Log(eventType, exception, format, null);
		}

		public void LogError(Exception exception, string formatString, params object[] formatArgs)
		{
			AnchorUtil.ThrowOnNullArgument(formatArgs, "formatArgs");
			AnchorUtil.ThrowOnNullArgument(formatString, "formatString");
			if (formatArgs.Length == 0)
			{
				this.Log(MigrationEventType.Error, exception, "{0}", new object[]
				{
					formatString
				});
				return;
			}
			this.Log(MigrationEventType.Error, exception, formatString, formatArgs);
		}

		public void LogVerbose(string formatString, params object[] formatArgs)
		{
			this.Log(MigrationEventType.Verbose, formatString, formatArgs);
		}

		public void LogWarning(string formatString, params object[] formatArgs)
		{
			this.Log(MigrationEventType.Warning, formatString, formatArgs);
		}

		public void LogInformation(string formatString, params object[] formatArgs)
		{
			this.Log(MigrationEventType.Information, formatString, formatArgs);
		}

		public void Log(MigrationEventType eventType, Exception exception, string formatString, params object[] args)
		{
			if (exception != null)
			{
				this.Log(eventType, formatString + ", exception " + AnchorLogger.GetDiagnosticInfo(exception, null), args);
				return;
			}
			this.Log(eventType, formatString, args);
		}

		public void Log(MigrationEventType eventType, string format)
		{
			this.Log(eventType, format, null);
		}

		public void Log(MigrationEventType eventType, string formatString, params object[] args)
		{
			this.Log(AnchorLogContext.Current.Source, eventType, AnchorLogContext.Current.ToString(), formatString, args);
		}

		public void Log(string source, MigrationEventType eventType, object context, string format, params object[] args)
		{
			if (this.log != null)
			{
				this.log.Log(source, eventType, context, format, args);
			}
			if (this.InMemoryLogger != null)
			{
				string arg = (args != null && args.Length > 0) ? string.Format(format, args) : format;
				this.InMemoryLogger(source, eventType, context, arg);
			}
		}

		internal static void LogEvent(ExEventLog.EventTuple eventId, params string[] messageArgs)
		{
			AnchorLogger.LogEvent(AnchorLogger.AnchorEventLogger, eventId, true, messageArgs);
		}

		internal static void LogEvent(ExEventLog eventLogger, ExEventLog.EventTuple eventId, params string[] messageArgs)
		{
			AnchorLogger.LogEvent(eventLogger, eventId, true, messageArgs);
		}

		internal static void LogEvent(ExEventLog eventLogger, ExEventLog.EventTuple eventId, bool includeAnchorContext, params string[] messageArgs)
		{
			AnchorUtil.ThrowOnNullArgument(eventLogger, "eventLogger");
			AnchorUtil.ThrowOnNullArgument(messageArgs, "messageArgs");
			if (messageArgs == null || messageArgs.Length <= 0)
			{
				return;
			}
			if (includeAnchorContext)
			{
				messageArgs[0] = AnchorLogContext.Current.ToString() + ":" + messageArgs[0];
			}
			eventLogger.LogEvent(eventId, AnchorLogContext.Current.Source, messageArgs);
		}

		protected void LogEventInternal(MigrationEventType eventType, ExEventLog.EventTuple? eventId, bool includeAnchorContext, Exception exception, params string[] args)
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (eventId != null)
			{
				stringBuilder.Append("Event " + (eventId.Value.EventId & 65535U).ToString() + " ");
			}
			else
			{
				eventId = this.EventIdFromLogLevel(eventType);
			}
			if (eventId == null)
			{
				return;
			}
			AnchorLogger.LogEvent(this.EventLogger, eventId.Value, includeAnchorContext, args);
			if (args != null)
			{
				foreach (string value in args)
				{
					stringBuilder.Append(value);
					stringBuilder.Append(',');
				}
			}
			this.Log(eventType, exception, stringBuilder.ToString());
		}

		protected virtual ExEventLog.EventTuple? EventIdFromLogLevel(MigrationEventType eventType)
		{
			switch (eventType)
			{
			case MigrationEventType.Error:
			case MigrationEventType.Warning:
				return new ExEventLog.EventTuple?(MSExchangeAnchorServiceEventLogConstants.Tuple_CriticalError);
			case MigrationEventType.Information:
				return new ExEventLog.EventTuple?(MSExchangeAnchorServiceEventLogConstants.Tuple_Information);
			default:
				return null;
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<AnchorLogger>(this);
		}

		protected override void InternalDispose(bool disposing)
		{
			if (disposing)
			{
				if (this.log != null)
				{
					this.log.Close();
				}
				this.log = null;
			}
		}

		private const int MaxCharsPerEventLog = 16384;

		private static readonly ExEventLog AnchorEventLogger = new ExEventLog(new Guid("0218300d-40aa-4060-91b6-beccda131340"), "MSExchange Anchor Service");

		private AnchorLog log;
	}
}
