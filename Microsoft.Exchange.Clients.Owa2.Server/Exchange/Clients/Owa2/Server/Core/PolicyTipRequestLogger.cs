using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class PolicyTipRequestLogger
	{
		private PolicyTipRequestLogger(string correlationId)
		{
			this.correlationId = (correlationId ?? string.Empty);
			this.creationTime = DateTime.UtcNow;
		}

		internal static PolicyTipRequestLogger CreateInstance(string correlationId)
		{
			return new PolicyTipRequestLogger(correlationId);
		}

		internal static List<string> MarkAsPII(List<string> data)
		{
			if (data == null)
			{
				return data;
			}
			for (int i = 0; i < data.Count; i++)
			{
				data[i] = PolicyTipRequestLogger.MarkAsPII(data[i]);
			}
			return data;
		}

		internal static string MarkAsPII(string data)
		{
			if (string.IsNullOrEmpty(data))
			{
				return string.Empty;
			}
			return string.Format("<PII>{0}</PII>", data);
		}

		internal void StartStage(LogStage stage)
		{
			this.StartStage(stage, DateTime.UtcNow);
		}

		internal void StartStage(LogStage stage, DateTime creationTime)
		{
			this.currentStageLogEntry = new PolicyTipRequestLogger.LogEntry(stage, creationTime);
		}

		internal void EndStageAndTransitionToStage(LogStage stage)
		{
			this.EndStage();
			if (stage == LogStage.SendResponse)
			{
				this.StartStage(stage, this.creationTime);
				return;
			}
			this.StartStage(stage);
		}

		internal TimeSpan EndStage()
		{
			TimeSpan elapsed = this.currentStageLogEntry.GetElapsed();
			PolicyTipProtocolLog.WriteToLog(this.correlationId, this.currentStageLogEntry.Stage.ToString(), this.currentStageLogEntry.Data, this.currentStageLogEntry.ExtraData, elapsed, this.currentStageLogEntry.OuterExceptionType, this.currentStageLogEntry.OuterExceptionMessage, this.currentStageLogEntry.InnerExceptionType, this.currentStageLogEntry.InnerExceptionMessage, this.currentStageLogEntry.ExceptionChain);
			return elapsed;
		}

		internal void AppendData(string key, string value)
		{
			this.currentStageLogEntry.AppendData(key, value);
		}

		internal void AppendExtraData(string key, string value)
		{
			this.currentStageLogEntry.AppendExtraData(key, value);
		}

		internal void SetException(Exception e)
		{
			this.currentStageLogEntry.SetException(e);
		}

		private const string PIIFormat = "<PII>{0}</PII>";

		internal const string TrueOr1 = "1";

		internal const string FalseOr0 = "0";

		internal readonly string correlationId;

		internal readonly DateTime creationTime;

		private PolicyTipRequestLogger.LogEntry currentStageLogEntry;

		private sealed class LogEntry
		{
			internal LogStage Stage { get; private set; }

			internal string OuterExceptionType { get; private set; }

			internal string OuterExceptionMessage { get; private set; }

			internal string InnerExceptionType { get; private set; }

			internal string InnerExceptionMessage { get; private set; }

			internal string ExceptionChain { get; private set; }

			internal string Data
			{
				get
				{
					return this.data.ToString();
				}
			}

			internal string ExtraData
			{
				get
				{
					if (this.extraData == null)
					{
						return string.Empty;
					}
					return this.extraData.ToString();
				}
			}

			internal LogEntry(LogStage stage) : this(stage, DateTime.UtcNow)
			{
			}

			internal LogEntry(LogStage stage, DateTime creationTime)
			{
				this.Stage = stage;
				this.creationTime = creationTime;
			}

			internal void AppendData(string key, string value)
			{
				if (key != null)
				{
					this.data.Append(key);
					this.data.Append(":");
					this.data.Append(value ?? string.Empty);
					this.data.Append(";");
				}
			}

			internal void AppendExtraData(string key, string value)
			{
				if (key != null)
				{
					if (this.extraData == null)
					{
						this.extraData = new StringBuilder();
					}
					this.extraData.Append(key);
					this.extraData.Append(":");
					this.extraData.Append(value ?? string.Empty);
					this.extraData.Append(";");
				}
			}

			internal void SetException(Exception e)
			{
				if (e != null)
				{
					List<string> list = null;
					List<string> list2 = null;
					string exceptionChain = null;
					PolicyTipProtocolLog.GetExceptionTypeAndDetails(e, out list, out list2, out exceptionChain, false);
					this.OuterExceptionType = list[0];
					this.OuterExceptionMessage = list2[0];
					if (list.Count > 1)
					{
						this.InnerExceptionType = list[list.Count - 1];
						this.InnerExceptionMessage = list2[list2.Count - 1];
					}
					this.ExceptionChain = exceptionChain;
				}
			}

			internal TimeSpan GetElapsed()
			{
				return DateTime.UtcNow - this.creationTime;
			}

			private const string delimiter = ";";

			private const string keyValueDelimiter = ":";

			private readonly DateTime creationTime;

			private StringBuilder data = new StringBuilder();

			private StringBuilder extraData;
		}
	}
}
