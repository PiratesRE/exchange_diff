using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Assistants
{
	internal class AssistantTaskContext
	{
		public AssistantTaskContext(MailboxData mailboxData, TimeBasedDatabaseJob job, List<KeyValuePair<string, object>> customDataToLog = null)
		{
			this.mailboxData = mailboxData;
			this.job = job;
			this.CustomDataToLog = (customDataToLog ?? new List<KeyValuePair<string, object>>());
			if (job != null)
			{
				this.step = new AssistantStep(job.Assistant.InitialStep);
			}
		}

		public MailboxData MailboxData
		{
			get
			{
				return this.mailboxData;
			}
		}

		public TimeBasedDatabaseJob Job
		{
			get
			{
				return this.job;
			}
		}

		public InvokeArgs Args
		{
			get
			{
				return this.args;
			}
			set
			{
				this.args = value;
			}
		}

		public AssistantStep Step
		{
			get
			{
				return this.step;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("step");
				}
				this.step = value;
			}
		}

		public List<KeyValuePair<string, object>> CustomDataToLog { get; set; }

		private MailboxData mailboxData;

		private TimeBasedDatabaseJob job;

		private InvokeArgs args;

		private AssistantStep step;
	}
}
