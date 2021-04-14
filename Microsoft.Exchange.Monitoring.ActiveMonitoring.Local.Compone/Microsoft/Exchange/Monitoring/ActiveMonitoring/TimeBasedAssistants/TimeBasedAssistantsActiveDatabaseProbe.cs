using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Local;
using Microsoft.Office.Datacenter.ActiveMonitoring;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.TimeBasedAssistants
{
	internal class TimeBasedAssistantsActiveDatabaseProbe : ProbeWorkItem
	{
		protected override void DoWork(CancellationToken cancellationToken)
		{
			base.Result.StateAttribute1 = base.Definition.TargetResource;
			base.Result.StateAttribute2 = base.Definition.TargetExtension;
			Dictionary<AssistantInfo, Dictionary<MailboxDatabase, WindowJob[]>> fullDiagnostics = TimeBasedAssistantsDiscoveryHelpers.ReadTimeBasedAssistantsDiagnostics("summary");
			TimeBasedAssistantsActiveDatabaseCriteria timeBasedAssistantsActiveDatabaseCriteria = new TimeBasedAssistantsActiveDatabaseCriteria();
			List<KeyValuePair<string, Guid[]>> list = timeBasedAssistantsActiveDatabaseCriteria.FindOutOfCriteria(fullDiagnostics, false);
			if (list.Any<KeyValuePair<string, Guid[]>>())
			{
				StringBuilder stringBuilder = new StringBuilder();
				foreach (KeyValuePair<string, Guid[]> keyValuePair in list)
				{
					stringBuilder.Append(string.Format("Assistant: {0}, not running databases: {1}", keyValuePair.Key, string.Join<Guid>(",", keyValuePair.Value)));
				}
				throw new AssistantsActiveDatabaseException(stringBuilder.ToString());
			}
		}

		private const string AssistantsComponentArgument = "summary";
	}
}
