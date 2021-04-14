using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Directory.ResourceHealth;

namespace Microsoft.Exchange.Assistants
{
	internal interface ITimeBasedAssistant : IAssistantBase
	{
		void OnStart();

		void OnWorkCycleCheckpoint();

		AssistantTaskContext InitialStep(AssistantTaskContext context);

		AssistantTaskContext InitializeContext(MailboxData data, TimeBasedDatabaseJob job);

		void Invoke(InvokeArgs invokeArgs, List<KeyValuePair<string, object>> customDataToLog = null);

		List<MailboxData> GetMailboxesToProcess();

		List<ResourceKey> GetResourceDependencies();

		MailboxData CreateOnDemandMailboxData(Guid itemGuid, string parameters);
	}
}
