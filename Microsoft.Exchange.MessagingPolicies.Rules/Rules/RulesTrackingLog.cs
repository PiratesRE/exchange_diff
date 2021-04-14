using System;
using System.IO;
using System.Reflection;
using System.Text;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Transport.Internal;

namespace Microsoft.Exchange.MessagingPolicies.Rules
{
	internal sealed class RulesTrackingLog
	{
		private RulesTrackingLog()
		{
			string[] array = new string[Enum.GetValues(typeof(RulesTrackingLog.Field)).Length];
			array[0] = "date-time";
			array[1] = "message-id";
			array[2] = "rule-name";
			array[3] = "details";
			array[4] = "action";
			array[5] = "from-address";
			array[6] = "recipient-address";
			this.rulesTrackingSchema = new LogSchema("Microsoft Exchange Server", Assembly.GetExecutingAssembly().GetName().Version.ToString(), "Rules Tracking Log", array);
			this.log = new Log("RULESTRK", new LogHeaderFormatter(this.rulesTrackingSchema), "RulesTrackingLogs");
		}

		internal static RulesTrackingLog GetLog()
		{
			if (RulesTrackingLog.trackingLog != null)
			{
				return RulesTrackingLog.trackingLog;
			}
			RulesTrackingLog result;
			lock (RulesTrackingLog.lockVar)
			{
				if (RulesTrackingLog.trackingLog == null)
				{
					RulesTrackingLog.trackingLog = new RulesTrackingLog();
				}
				result = RulesTrackingLog.trackingLog;
			}
			return result;
		}

		internal void Close()
		{
			this.log.Close();
		}

		internal void TrackRuleAction(string actionType, string ruleName, string details, MailMessage message)
		{
			LogRowFormatter logRowFormatter = new LogRowFormatter(this.rulesTrackingSchema);
			logRowFormatter[1] = message.MessageId;
			logRowFormatter[2] = ruleName;
			logRowFormatter[3] = details;
			logRowFormatter[4] = actionType;
			logRowFormatter[5] = message.EnvelopeFrom;
			StringBuilder stringBuilder = new StringBuilder(string.Empty);
			foreach (string value in message.EnvelopeRecipients)
			{
				stringBuilder.Append(value);
				stringBuilder.Append(";");
			}
			if (stringBuilder.Length > 0)
			{
				logRowFormatter[6] = stringBuilder.ToString(0, stringBuilder.Length - 1);
			}
			else
			{
				logRowFormatter[6] = string.Empty;
			}
			if (this.rulesTrackingLogPath != Configuration.TransportServer.PipelineTracingPath.PathName)
			{
				this.rulesTrackingLogPath = Configuration.TransportServer.PipelineTracingPath.PathName;
				this.Configure();
			}
			this.Append(logRowFormatter);
		}

		private void Append(LogRowFormatter row)
		{
			this.log.Append(row, 0);
		}

		private void Configure()
		{
			this.log.Configure(Path.Combine(this.rulesTrackingLogPath, "RulesTracking\\"), this.maxAgeInDays, (long)this.maxDirectorySizeInBytes.Value.ToBytes(), (long)this.maxPerFileSizeInBytes.Value.ToBytes());
		}

		private static readonly object lockVar = new object();

		private static RulesTrackingLog trackingLog;

		private TimeSpan maxAgeInDays = new TimeSpan(30, 0, 0, 0);

		private Unlimited<ByteQuantifiedSize> maxDirectorySizeInBytes = ByteQuantifiedSize.FromMB(25UL);

		private Unlimited<ByteQuantifiedSize> maxPerFileSizeInBytes = ByteQuantifiedSize.FromMB(2UL);

		private string rulesTrackingLogPath;

		private LogSchema rulesTrackingSchema;

		private Log log;

		private enum Field
		{
			DateTime,
			MessageID,
			RuleName,
			Details,
			Action,
			FromAddress,
			RecipientAddress
		}
	}
}
