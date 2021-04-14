using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using System.Threading;
using Microsoft.Exchange.Conversion;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Transport.Sync.Common.Subscription;

namespace Microsoft.Exchange.Transport.Sync.Common.Logging
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class SyncLogSession
	{
		internal SyncLogSession(SyncLog syncLog, LogRowFormatter row)
		{
			if (syncLog == null)
			{
				throw new ArgumentNullException("syncLog");
			}
			if (row == null)
			{
				throw new ArgumentNullException("row");
			}
			this.syncLog = syncLog;
			this.row = row;
			this.blackBox = new SyncLogBlackBox(true);
		}

		internal static SyncLogSession InMemorySyncLogSession
		{
			get
			{
				return SyncLogSession.inmemorySyncLogSession;
			}
		}

		internal bool SkipDebugAsserts
		{
			get
			{
				return this.skipDebugAsserts;
			}
			set
			{
				this.skipDebugAsserts = value;
			}
		}

		internal SyncLogBlackBox BlackBox
		{
			get
			{
				return this.blackBox;
			}
		}

		internal SyncLog SyncLog
		{
			get
			{
				return this.syncLog;
			}
		}

		public void SetBlackBoxCapacity(int capacity)
		{
			this.blackBox.ResizeAndClear(capacity);
		}

		public string GetBlackBoxText()
		{
			return this.blackBox.ToString();
		}

		public void ClearBlackBox()
		{
			this.blackBox.Clear();
		}

		public void ReportWatson(string message)
		{
			this.ReportWatson(message, null);
		}

		public void ReportWatson(Exception exception)
		{
			this.ReportWatson(null, exception);
		}

		public void ReportWatson(string message, Exception exception)
		{
			this.syncLog.WatsonReporter.ReportWatson(this, message, exception);
		}

		[Conditional("DEBUG")]
		public void Assert(bool expression, string formatString, params object[] parameters)
		{
			if (expression)
			{
				return;
			}
			this.LogError((TSLID)0UL, "Assert: " + formatString, parameters);
			bool flag = this.skipDebugAsserts;
		}

		public void RetailAssert(bool expression, string formatString, params object[] parameters)
		{
			if (expression)
			{
				return;
			}
			this.LogError((TSLID)0UL, "Assert: " + formatString, parameters);
			ExAssert.RetailAssert(expression, formatString, parameters);
		}

		public void AddBlackBoxLogToWatson()
		{
			SyncLog syncLog = this.syncLog;
			if (syncLog == null)
			{
				return;
			}
			ExWatson.AddExtraData(syncLog.GenerateBlackBoxLog());
		}

		public void LogConnect(TSLID logEntryId, string contextFormat, params object[] contextArguments)
		{
			this.LogEvent(logEntryId, SyncLoggingLevel.RawData, Guid.Empty, null, "+", null, contextFormat, contextArguments);
		}

		public void LogDisconnect(TSLID logEntryId, string reason)
		{
			this.LogEvent(logEntryId, SyncLoggingLevel.RawData, Guid.Empty, null, "-", null, reason, new object[0]);
		}

		public void LogSend(TSLID logEntryId, byte[] data)
		{
			this.LogEvent(logEntryId, SyncLoggingLevel.RawData, Guid.Empty, null, ">", data, null, new object[0]);
		}

		public void LogSend(TSLID logEntryId, string format, params object[] arguments)
		{
			this.LogEvent(logEntryId, SyncLoggingLevel.RawData, Guid.Empty, null, ">", null, null, arguments);
		}

		public void LogConnectionInformation(TSLID logEntryId, string endpoint)
		{
			this.LogEvent(logEntryId, SyncLoggingLevel.Information, Guid.Empty, null, null, null, "Destination Server: {0}", new object[]
			{
				endpoint
			});
		}

		public void LogReceive(TSLID logEntryId, byte[] data)
		{
			this.LogEvent(logEntryId, SyncLoggingLevel.RawData, Guid.Empty, null, "<", data, null, new object[0]);
		}

		public void LogReceive(TSLID logEntryId, string format, params object[] arguments)
		{
			this.LogEvent(logEntryId, SyncLoggingLevel.RawData, Guid.Empty, null, "<", null, format, arguments);
		}

		public void LogInformation(TSLID logEntryId, string format, params object[] arguments)
		{
			this.LogEvent(logEntryId, SyncLoggingLevel.Information, Guid.Empty, null, null, null, format, arguments);
		}

		public void LogInformation(TSLID logEntryId, Microsoft.Exchange.Diagnostics.Trace trace, long id, string format, params object[] arguments)
		{
			this.LogInformation(logEntryId, format, arguments);
			if (trace.IsTraceEnabled(TraceType.DebugTrace))
			{
				trace.TraceDebug(id, this.MakeTracerMessage(format, arguments));
			}
		}

		public void LogInformation(TSLID logEntryId, Microsoft.Exchange.Diagnostics.Trace trace, string format, params object[] arguments)
		{
			this.LogInformation(logEntryId, trace, 0L, format, arguments);
		}

		public void LogVerbose(TSLID logEntryId, string format, params object[] arguments)
		{
			this.LogEvent(logEntryId, SyncLoggingLevel.Verbose, Guid.Empty, null, null, null, format, arguments);
		}

		public void LogVerbose(TSLID logEntryId, Microsoft.Exchange.Diagnostics.Trace trace, long id, string format, params object[] arguments)
		{
			this.LogVerbose(logEntryId, format, arguments);
			if (trace.IsTraceEnabled(TraceType.DebugTrace))
			{
				trace.TraceDebug(id, this.MakeTracerMessage(format, arguments));
			}
		}

		public void LogVerbose(TSLID logEntryId, Microsoft.Exchange.Diagnostics.Trace trace, string format, params object[] arguments)
		{
			this.LogVerbose(logEntryId, trace, 0L, format, arguments);
		}

		public void LogError(TSLID logEntryId, string format, params object[] arguments)
		{
			this.LogEvent(logEntryId, SyncLoggingLevel.Error, Guid.Empty, null, null, null, format, arguments);
		}

		public void LogError(TSLID logEntryId, Microsoft.Exchange.Diagnostics.Trace trace, long id, string format, params object[] arguments)
		{
			this.LogError(logEntryId, format, arguments);
			if (trace.IsTraceEnabled(TraceType.ErrorTrace))
			{
				trace.TraceError(id, this.MakeTracerMessage(format, arguments));
			}
		}

		public void LogError(TSLID logEntryId, Microsoft.Exchange.Diagnostics.Trace trace, string format, params object[] arguments)
		{
			this.LogError(logEntryId, trace, 0L, format, arguments);
		}

		public void LogRawData(TSLID logEntryId, string format, params object[] arguments)
		{
			this.LogEvent(logEntryId, SyncLoggingLevel.RawData, Guid.Empty, null, null, null, format, arguments);
		}

		public void LogRawData(TSLID logEntryId, Microsoft.Exchange.Diagnostics.Trace trace, long id, string format, params object[] arguments)
		{
			this.LogRawData(logEntryId, format, arguments);
			if (trace.IsTraceEnabled(TraceType.DebugTrace))
			{
				trace.TraceDebug(id, this.MakeTracerMessage(format, arguments));
			}
		}

		public void LogRawData(TSLID logEntryId, Microsoft.Exchange.Diagnostics.Trace trace, string format, params object[] arguments)
		{
			this.LogRawData(logEntryId, trace, 0L, format, arguments);
		}

		public void LogDebugging(TSLID logEntryId, string format, params object[] arguments)
		{
			this.LogEvent(logEntryId, SyncLoggingLevel.Debugging, Guid.Empty, null, null, null, format, arguments);
		}

		public void LogDebugging(TSLID logEntryId, Guid subscriptionGuid, Guid mailboxGuid, string format, params object[] arguments)
		{
			this.LogEvent(logEntryId, SyncLoggingLevel.Debugging, subscriptionGuid, mailboxGuid, null, null, format, arguments);
		}

		public void LogDebugging(TSLID logEntryId, Microsoft.Exchange.Diagnostics.Trace trace, long id, string format, params object[] arguments)
		{
			this.LogDebugging(logEntryId, format, arguments);
			if (trace.IsTraceEnabled(TraceType.DebugTrace))
			{
				trace.TraceDebug(id, this.MakeTracerMessage(format, arguments));
			}
		}

		public void LogDebugging(TSLID logEntryId, Microsoft.Exchange.Diagnostics.Trace trace, string format, params object[] arguments)
		{
			this.LogDebugging(logEntryId, trace, 0L, format, arguments);
		}

		public void Log(TSLID logEntryId, SyncLoggingLevel loggingLevel, byte[] data, string context)
		{
			this.LogEvent(logEntryId, loggingLevel, Guid.Empty, null, null, data, context, new object[0]);
		}

		public SyncLogSession OpenWithContext(Guid mailboxGuid, AggregationSubscription subscription)
		{
			SyncLogSession syncLogSession = this.syncLog.OpenSession(mailboxGuid, subscription.SubscriptionType, subscription.SubscriptionGuid);
			this.ShareBlackBoxWith(syncLogSession);
			return syncLogSession;
		}

		public SyncLogSession OpenWithContext(Guid mailboxId, Guid subscriptionId)
		{
			return this.syncLog.OpenSession(mailboxId, subscriptionId);
		}

		public SyncLogSession OpenWithContext(Guid subscriptionId)
		{
			SyncLogSession syncLogSession = this.syncLog.OpenSession(Guid.Empty, subscriptionId);
			this.ShareBlackBoxWith(syncLogSession);
			return syncLogSession;
		}

		public void LogItemLevelError(TSLID logEntryId, params KeyValuePair<string, object>[] propertyBag)
		{
			this.LogEventWithProperyBag(logEntryId, SyncLoggingLevel.Error, Guid.Empty, null, "ItemLevelError", propertyBag);
		}

		protected void LogEventWithProperyBag(TSLID logEntryId, SyncLoggingLevel loggingLevel, Guid subscriptionGuid, object user, string eventId, params KeyValuePair<string, object>[] propertyBag)
		{
			if (this.syncLog.LoggingLevel < loggingLevel)
			{
				return;
			}
			if (propertyBag != null)
			{
				for (int i = 0; i < propertyBag.Length; i++)
				{
					if (propertyBag[i].Value is string)
					{
						propertyBag.SetValue(new KeyValuePair<string, object>(propertyBag[i].Key, propertyBag[i].Value), i);
					}
				}
			}
			this.InternalLogEvent(logEntryId, loggingLevel, subscriptionGuid, user, null, null, eventId, propertyBag);
		}

		protected void LogEvent(TSLID logEntryId, SyncLoggingLevel loggingLevel, Guid subscriptionGuid, object user, string prefix, byte[] data, string format, params object[] arguments)
		{
			if (this.syncLog.LoggingLevel < loggingLevel)
			{
				return;
			}
			string context = null;
			if (format != null)
			{
				if (arguments != null)
				{
					try
					{
						context = string.Format(CultureInfo.InvariantCulture, format, arguments);
						goto IL_3C;
					}
					catch (FormatException exception)
					{
						this.ReportWatson("Malformed logging format found.", exception);
						return;
					}
				}
				context = format;
			}
			IL_3C:
			this.InternalLogEvent(logEntryId, loggingLevel, subscriptionGuid, user, prefix, data, context, null);
		}

		protected void InternalLogEvent(TSLID logEntryId, SyncLoggingLevel loggingLevel, Guid subscriptionGuid, object user, string prefix, byte[] data, string context, KeyValuePair<string, object>[] propertyBag)
		{
			if (this.syncLog.LoggingLevel < loggingLevel)
			{
				return;
			}
			lock (this.syncRoot)
			{
				object obj2 = null;
				object value = null;
				try
				{
					if (user != null)
					{
						obj2 = this.row[3];
						this.row[3] = user.ToString();
					}
					if (!object.Equals(subscriptionGuid, Guid.Empty))
					{
						value = this.row[4];
						this.row[4] = subscriptionGuid.ToString();
					}
					this.row[2] = (int)loggingLevel;
					this.row[1] = Thread.CurrentThread.ManagedThreadId;
					this.row[6] = logEntryId.ToString();
					this.row[8] = propertyBag;
					if (data == null)
					{
						this.row[7] = prefix + context;
						this.syncLog.Append(this.row);
						this.blackBox.Append(this.row);
					}
					else
					{
						int num = 0;
						string line;
						do
						{
							int num2 = data.Length + " - ".Length;
							if (context != null)
							{
								num2 += context.Length;
							}
							if (prefix != null)
							{
								num2 += prefix.Length;
							}
							StringBuilder stringBuilder = new StringBuilder(num2);
							line = SyncLogSession.GetLine(data, num);
							if (line != null)
							{
								num += line.Length + 2;
							}
							if (line != null || (line == null && num == 0))
							{
								stringBuilder.Append(prefix);
								stringBuilder.Append(line);
								stringBuilder.Append(" - ");
								stringBuilder.Append(context);
								this.row[7] = stringBuilder.ToString();
								this.syncLog.Append(this.row);
								this.blackBox.Append(this.row);
							}
						}
						while (line != null);
					}
				}
				finally
				{
					if (obj2 != null)
					{
						this.row[3] = obj2;
					}
					if (!object.Equals(subscriptionGuid, Guid.Empty))
					{
						this.row[4] = value;
					}
					this.row[6] = string.Empty;
				}
			}
		}

		private static string GetLine(byte[] buffer, int start)
		{
			int i = start;
			int num = -1;
			byte[] array = null;
			if (buffer == null)
			{
				return null;
			}
			while (i < buffer.Length)
			{
				i = SyncLogSession.IndexOf(buffer, 10, i);
				if (i == -1)
				{
					num = buffer.Length - start;
					break;
				}
				if (i > start && buffer[i - 1] == 13)
				{
					num = i - start + 1 - 2;
					break;
				}
				i++;
			}
			if (num > 0)
			{
				if (start == 0 && num == buffer.Length)
				{
					array = buffer;
				}
				else
				{
					array = new byte[num];
					Buffer.BlockCopy(buffer, start, array, 0, num);
				}
			}
			if (array == null)
			{
				return null;
			}
			return Encoding.UTF8.GetString(array);
		}

		private static int IndexOf(byte[] buffer, byte val, int offset)
		{
			return ExBuffer.IndexOf(buffer, val, offset, buffer.Length - offset);
		}

		private string MakeTracerMessage(string format, object[] arguments)
		{
			string result;
			try
			{
				result = string.Format(SyncLogSession.TracerFormatProvider.Instance, format, arguments);
			}
			catch (FormatException exception)
			{
				this.ReportWatson("Malformed logging format found.", exception);
				result = string.Empty;
			}
			return result;
		}

		private void ShareBlackBoxWith(SyncLogSession newSession)
		{
			newSession.blackBox = this.blackBox;
		}

		private const string Connect = "+";

		private const string Disconnect = "-";

		private const string Send = ">";

		private const string Receive = "<";

		private const string Separator = " - ";

		private const string ItemLevelError = "ItemLevelError";

		private static readonly SyncLogSession inmemorySyncLogSession = SyncLog.InMemorySyncLog.OpenSession();

		private SyncLog syncLog;

		private SyncLogBlackBox blackBox;

		private LogRowFormatter row;

		private object syncRoot = new object();

		private bool skipDebugAsserts;

		internal sealed class TracerFormatProvider : IFormatProvider, ICustomFormatter
		{
			private TracerFormatProvider()
			{
			}

			public static SyncLogSession.TracerFormatProvider Instance
			{
				get
				{
					return SyncLogSession.TracerFormatProvider.instance;
				}
			}

			public object GetFormat(Type formatType)
			{
				if (formatType == typeof(ICustomFormatter))
				{
					return this;
				}
				return CultureInfo.InvariantCulture.GetFormat(formatType);
			}

			public string Format(string format, object argument, IFormatProvider formatProvider)
			{
				Exception ex = argument as Exception;
				if (ex != null)
				{
					return string.Format(CultureInfo.InvariantCulture, "[{0}: {1}]", new object[]
					{
						ex.GetType().FullName,
						ex.Message
					});
				}
				IFormattable formattable = argument as IFormattable;
				if (formattable != null)
				{
					return formattable.ToString(format, formatProvider);
				}
				if (argument == null)
				{
					return "<null>";
				}
				return argument.ToString();
			}

			private const string FormattedNull = "<null>";

			private static SyncLogSession.TracerFormatProvider instance = new SyncLogSession.TracerFormatProvider();
		}
	}
}
