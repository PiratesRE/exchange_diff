using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.WorkloadManagement;

namespace Microsoft.Exchange.Assistants
{
	internal interface ITimeBasedAssistantType : IAssistantType
	{
		TimeBasedAssistantIdentifier Identifier { get; }

		WorkloadType WorkloadType { get; }

		PropertyTagPropertyDefinition ControlDataPropertyDefinition { get; }

		PropertyTagPropertyDefinition[] MailboxExtendedProperties { get; }

		TimeSpan WorkCycle { get; }

		TimeSpan WorkCycleCheckpoint { get; }

		void OnWorkCycleStart(DatabaseInfo databaseInfo);

		void OnWorkCycleCheckpoint();

		bool IsMailboxInteresting(MailboxInformation mailboxInformation);

		ITimeBasedAssistant CreateInstance(DatabaseInfo databaseInfo);

		TimeBasedDatabaseDriver CreateDriver(ThrottleGovernor governor, DatabaseInfo databaseInfo, ITimeBasedAssistantType timeBasedAssistantType, PoisonMailboxControl poisonControl, PerformanceCountersPerDatabaseInstance databaseCounters);
	}
}
