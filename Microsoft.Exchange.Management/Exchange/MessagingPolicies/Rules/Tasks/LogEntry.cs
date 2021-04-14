using System;

namespace Microsoft.Exchange.MessagingPolicies.Rules.Tasks
{
	internal class LogEntry
	{
		public LogEntry(DateTime timestamp, string id, string name, string details, string action, string from, string recipients)
		{
			this.timeStamp = timestamp;
			this.messageID = id;
			this.ruleName = name;
			this.details = details;
			this.action = action;
			this.fromAddress = from;
			this.recipientAddress = recipients;
		}

		public string RuleName
		{
			get
			{
				return this.ruleName;
			}
		}

		public string MessageID
		{
			get
			{
				return this.messageID;
			}
		}

		public string Details
		{
			get
			{
				return this.details;
			}
		}

		public DateTime TimeStamp
		{
			get
			{
				return this.timeStamp;
			}
		}

		public string Action
		{
			get
			{
				return this.action;
			}
		}

		public string FromAddress
		{
			get
			{
				return this.fromAddress;
			}
		}

		public string RecipientAddress
		{
			get
			{
				return this.recipientAddress;
			}
		}

		private readonly DateTime timeStamp;

		private readonly string messageID;

		private readonly string ruleName;

		private readonly string details;

		private readonly string action;

		private readonly string fromAddress;

		private readonly string recipientAddress;
	}
}
