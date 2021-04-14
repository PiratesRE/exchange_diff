﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Local;
using Microsoft.Office.Datacenter.ActiveMonitoring;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.TimeBasedAssistants
{
	internal abstract class TimeBasedAssistantsOutOfSlaProbe : ProbeWorkItem
	{
		protected abstract TimeBasedAssistantsOutOfSlaDecisionMaker CreateDecisionMakerInstance();

		protected override void DoWork(CancellationToken cancellationToken)
		{
			base.Result.StateAttribute1 = base.Definition.TargetResource;
			base.Result.StateAttribute2 = base.Definition.TargetExtension;
			TimeBasedAssistantsOutOfSlaDecisionMaker timeBasedAssistantsOutOfSlaDecisionMaker = this.CreateDecisionMakerInstance();
			Dictionary<AssistantInfo, Dictionary<MailboxDatabase, WindowJob[]>> fullDiagnostics = TimeBasedAssistantsDiscoveryHelpers.ReadTimeBasedAssistantsDiagnostics("history");
			Dictionary<AssistantInfo, Dictionary<MailboxDatabase, WindowJob[]>> dictionary = timeBasedAssistantsOutOfSlaDecisionMaker.FindOutOfCriteria(fullDiagnostics);
			if (dictionary.Any<KeyValuePair<AssistantInfo, Dictionary<MailboxDatabase, WindowJob[]>>>())
			{
				string error = TimeBasedAssistantsDiscoveryHelpers.GenerateMessageForLastNFailures(dictionary, timeBasedAssistantsOutOfSlaDecisionMaker);
				throw new AssistantsOutOfSlaException(error);
			}
		}

		private const string AssistantsComponentArgument = "history";
	}
}
