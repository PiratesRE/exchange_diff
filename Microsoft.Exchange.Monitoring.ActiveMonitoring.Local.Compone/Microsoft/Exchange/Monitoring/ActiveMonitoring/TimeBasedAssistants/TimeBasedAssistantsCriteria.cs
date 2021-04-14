using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.TimeBasedAssistants
{
	internal abstract class TimeBasedAssistantsCriteria
	{
		public Dictionary<AssistantInfo, Dictionary<MailboxDatabase, WindowJob[]>> FindOutOfCriteria(Dictionary<AssistantInfo, Dictionary<MailboxDatabase, WindowJob[]>> fullDiagnostics)
		{
			ArgumentValidator.ThrowIfNull("fullDiagnostics", fullDiagnostics);
			Dictionary<MailboxDatabase, WindowJob[]> dictionary = null;
			Dictionary<AssistantInfo, Dictionary<MailboxDatabase, WindowJob[]>> dictionary2 = new Dictionary<AssistantInfo, Dictionary<MailboxDatabase, WindowJob[]>>();
			foreach (KeyValuePair<AssistantInfo, Dictionary<MailboxDatabase, WindowJob[]>> keyValuePair in fullDiagnostics)
			{
				AssistantInfo key = keyValuePair.Key;
				IEnumerable<MailboxDatabase> keys = keyValuePair.Value.Keys;
				foreach (MailboxDatabase mailboxDatabase in keys)
				{
					if (mailboxDatabase.IsAssistantEnabled)
					{
						WindowJob[] array = keyValuePair.Value[mailboxDatabase];
						if (!this.IsSatisfied(key, mailboxDatabase, array))
						{
							if (dictionary == null)
							{
								dictionary = new Dictionary<MailboxDatabase, WindowJob[]>();
							}
							dictionary.Add(mailboxDatabase, array);
						}
					}
				}
				if (dictionary != null)
				{
					dictionary2.Add(key, dictionary);
					dictionary = null;
				}
			}
			return dictionary2;
		}

		protected abstract bool IsSatisfied(AssistantInfo assistantInfo, MailboxDatabase database, WindowJob[] history);

		protected readonly DateTime CurrentSampleTime = DateTime.UtcNow;
	}
}
