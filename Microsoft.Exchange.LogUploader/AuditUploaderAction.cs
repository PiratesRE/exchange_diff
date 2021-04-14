using System;

namespace Microsoft.Exchange.LogUploader
{
	public class AuditUploaderAction
	{
		public AuditUploaderAction(Actions action, TimeSpan? interval)
		{
			this.ActionToPerform = action;
			this.ActionThrottlingInterval = interval;
			this.LastTriggerDate = DateTime.MinValue;
		}

		public Actions ActionToPerform { get; set; }

		public TimeSpan? ActionThrottlingInterval { get; set; }

		public DateTime LastTriggerDate { get; set; }
	}
}
