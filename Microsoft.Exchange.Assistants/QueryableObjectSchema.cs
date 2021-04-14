using System;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Assistants
{
	internal class QueryableObjectSchema : SimpleProviderObjectSchema
	{
		public static readonly SimpleProviderPropertyDefinition ActiveMailboxes = new SimpleProviderPropertyDefinition("ActiveMailboxes", ExchangeObjectVersion.Exchange2010, typeof(Guid), PropertyDefinitionFlags.MultiValued, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition ActiveQueueLength = new SimpleProviderPropertyDefinition("ActiveQueueLength", ExchangeObjectVersion.Exchange2010, typeof(int), PropertyDefinitionFlags.PersistDefaultValue, 0, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition ActiveWorkers = new SimpleProviderPropertyDefinition("ActiveWorkers", ExchangeObjectVersion.Exchange2010, typeof(int), PropertyDefinitionFlags.PersistDefaultValue, 0, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition ActiveWorkItems = new SimpleProviderPropertyDefinition("ActiveWorkItems", ExchangeObjectVersion.Exchange2010, typeof(int), PropertyDefinitionFlags.PersistDefaultValue, 0, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition AssistantGuid = new SimpleProviderPropertyDefinition("AssistantGuid", ExchangeObjectVersion.Exchange2010, typeof(Guid), PropertyDefinitionFlags.PersistDefaultValue, Guid.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition AssistantName = new SimpleProviderPropertyDefinition("AssistantName", ExchangeObjectVersion.Exchange2010, typeof(string), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition CurrentThrottle = new SimpleProviderPropertyDefinition("CurrentThrottle", ExchangeObjectVersion.Exchange2010, typeof(int), PropertyDefinitionFlags.PersistDefaultValue, 0, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition CommittedWatermark = new SimpleProviderPropertyDefinition("CommittedWatermark", ExchangeObjectVersion.Exchange2010, typeof(long), PropertyDefinitionFlags.PersistDefaultValue, 0L, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition DatabaseName = new SimpleProviderPropertyDefinition("DatabaseName", ExchangeObjectVersion.Exchange2010, typeof(string), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition DatabaseGuid = new SimpleProviderPropertyDefinition("DatabaseGuid", ExchangeObjectVersion.Exchange2010, typeof(Guid), PropertyDefinitionFlags.PersistDefaultValue, Guid.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition DeadMailboxes = new SimpleProviderPropertyDefinition("DeadMailboxes", ExchangeObjectVersion.Exchange2010, typeof(Guid), PropertyDefinitionFlags.MultiValued, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition DecayedEventCounter = new SimpleProviderPropertyDefinition("DecayedEventCounter", ExchangeObjectVersion.Exchange2010, typeof(long), PropertyDefinitionFlags.PersistDefaultValue, 0L, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition EventController = new SimpleProviderPropertyDefinition("EventController", ExchangeObjectVersion.Exchange2010, typeof(QueryableEventController), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition EventFilter = new SimpleProviderPropertyDefinition("EventFilter", ExchangeObjectVersion.Exchange2010, typeof(string), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition Governor = new SimpleProviderPropertyDefinition("Governor", ExchangeObjectVersion.Exchange2010, typeof(QueryableThrottleGovernor), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition HighestEventPolled = new SimpleProviderPropertyDefinition("HighestEventPolled", ExchangeObjectVersion.Exchange2010, typeof(long), PropertyDefinitionFlags.PersistDefaultValue, 0L, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition HighestEventQueued = new SimpleProviderPropertyDefinition("HighestEventQueued", ExchangeObjectVersion.Exchange2010, typeof(long), PropertyDefinitionFlags.PersistDefaultValue, 0L, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition IsIdle = new SimpleProviderPropertyDefinition("IsIdle", ExchangeObjectVersion.Exchange2010, typeof(bool), PropertyDefinitionFlags.PersistDefaultValue, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition IsInRetry = new SimpleProviderPropertyDefinition("IsInRetry", ExchangeObjectVersion.Exchange2010, typeof(bool), PropertyDefinitionFlags.PersistDefaultValue, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition IsMailboxDead = new SimpleProviderPropertyDefinition("IsMailboxDead", ExchangeObjectVersion.Exchange2010, typeof(bool), PropertyDefinitionFlags.PersistDefaultValue, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition IsStopping = new SimpleProviderPropertyDefinition("IsStopping", ExchangeObjectVersion.Exchange2010, typeof(bool), PropertyDefinitionFlags.PersistDefaultValue, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition LastRunTime = new SimpleProviderPropertyDefinition("LastRunTime", ExchangeObjectVersion.Exchange2010, typeof(DateTime), PropertyDefinitionFlags.PersistDefaultValue, DateTime.MinValue, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition MailboxGuid = new SimpleProviderPropertyDefinition("MailboxGuid", ExchangeObjectVersion.Exchange2010, typeof(Guid), PropertyDefinitionFlags.PersistDefaultValue, Guid.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition MailboxType = new SimpleProviderPropertyDefinition("MailboxType", ExchangeObjectVersion.Exchange2010, typeof(string), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition MapiEventType = new SimpleProviderPropertyDefinition("MapiEventType", ExchangeObjectVersion.Exchange2010, typeof(string), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition NeedMailboxSession = new SimpleProviderPropertyDefinition("NeedMailboxSession", ExchangeObjectVersion.Exchange2010, typeof(bool), PropertyDefinitionFlags.PersistDefaultValue, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition NumberConsecutiveFailures = new SimpleProviderPropertyDefinition("NumberConsecutiveFailures", ExchangeObjectVersion.Exchange2010, typeof(long), PropertyDefinitionFlags.PersistDefaultValue, 0L, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition NumberEventsInQueueCurrent = new SimpleProviderPropertyDefinition("NumberEventsInQueueCurrent", ExchangeObjectVersion.Exchange2010, typeof(long), PropertyDefinitionFlags.PersistDefaultValue, 0L, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition NumberOfActiveDispatchers = new SimpleProviderPropertyDefinition("NumberOfActiveDispatchers", ExchangeObjectVersion.Exchange2010, typeof(int), PropertyDefinitionFlags.PersistDefaultValue, 0, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition ObjectClass = new SimpleProviderPropertyDefinition("ObjectClass", ExchangeObjectVersion.Exchange2010, typeof(string), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition OverThrottle = new SimpleProviderPropertyDefinition("OverThrottle", ExchangeObjectVersion.Exchange2010, typeof(bool), PropertyDefinitionFlags.PersistDefaultValue, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition PendingQueueLength = new SimpleProviderPropertyDefinition("PendingQueueLength", ExchangeObjectVersion.Exchange2010, typeof(int), PropertyDefinitionFlags.PersistDefaultValue, 0, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition PendingWorkers = new SimpleProviderPropertyDefinition("PendingWorkers", ExchangeObjectVersion.Exchange2010, typeof(int), PropertyDefinitionFlags.PersistDefaultValue, 0, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition PendingWorkItemsOnBase = new SimpleProviderPropertyDefinition("PendingWorkItemsOnBase", ExchangeObjectVersion.Exchange2010, typeof(int), PropertyDefinitionFlags.PersistDefaultValue, 0, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition QueueLength = new SimpleProviderPropertyDefinition("QueueLength", ExchangeObjectVersion.Exchange2010, typeof(int), PropertyDefinitionFlags.PersistDefaultValue, 0, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition RecoveryEventCounter = new SimpleProviderPropertyDefinition("RecoveryEventCounter", ExchangeObjectVersion.Exchange2010, typeof(long), PropertyDefinitionFlags.PersistDefaultValue, 0L, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition RecoveryEventDispatcheres = new SimpleProviderPropertyDefinition("RecoveryEventDispatcheres", ExchangeObjectVersion.Exchange2010, typeof(Guid), PropertyDefinitionFlags.MultiValued, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition RestartRequired = new SimpleProviderPropertyDefinition("RestartRequired", ExchangeObjectVersion.Exchange2010, typeof(bool), PropertyDefinitionFlags.PersistDefaultValue, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition ShutdownState = new SimpleProviderPropertyDefinition("ShutdownState", ExchangeObjectVersion.Exchange2010, typeof(string), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition StartState = new SimpleProviderPropertyDefinition("StartState", ExchangeObjectVersion.Exchange2010, typeof(string), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition Status = new SimpleProviderPropertyDefinition("Status", ExchangeObjectVersion.Exchange2010, typeof(string), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition Throttle = new SimpleProviderPropertyDefinition("Throttle", ExchangeObjectVersion.Exchange2010, typeof(QueryableThrottle), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition ThrottleName = new SimpleProviderPropertyDefinition("ThrottleName", ExchangeObjectVersion.Exchange2010, typeof(string), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition TimeToSaveWatermarks = new SimpleProviderPropertyDefinition("TimeToSaveWatermarks", ExchangeObjectVersion.Exchange2010, typeof(DateTime), PropertyDefinitionFlags.PersistDefaultValue, DateTime.MinValue, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition TimeToUpdateIdleWatermarks = new SimpleProviderPropertyDefinition("TimeToUpdateIdleWatermarks", ExchangeObjectVersion.Exchange2010, typeof(DateTime), PropertyDefinitionFlags.PersistDefaultValue, DateTime.MinValue, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition UpToDateMailboxes = new SimpleProviderPropertyDefinition("UpToDateMailboxes", ExchangeObjectVersion.Exchange2010, typeof(Guid), PropertyDefinitionFlags.MultiValued, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);
	}
}
