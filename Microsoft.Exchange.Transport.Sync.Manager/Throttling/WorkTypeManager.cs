using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Transport.Sync.Common.Subscription;
using Microsoft.Exchange.Transport.Sync.Common.Subscription.Pim;

namespace Microsoft.Exchange.Transport.Sync.Manager.Throttling
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class WorkTypeManager
	{
		internal static WorkTypeManager Instance
		{
			get
			{
				return WorkTypeManager.workTypeManager;
			}
		}

		internal WorkTypeDefinition this[WorkType workType]
		{
			get
			{
				return this.workTypeDefinitions[workType];
			}
		}

		internal static WorkType ClassifyWorkTypeFromSubscriptionInformation(AggregationType aggregationType, SyncPhase syncPhase)
		{
			if (aggregationType != AggregationType.Aggregation)
			{
				if (aggregationType != AggregationType.Migration)
				{
					if (aggregationType != AggregationType.PeopleConnection)
					{
						throw new InvalidOperationException("unsupported aggregation type");
					}
					switch (syncPhase)
					{
					case SyncPhase.Initial:
						return WorkType.PeopleConnectionInitial;
					case SyncPhase.Incremental:
						return WorkType.PeopleConnectionIncremental;
					case SyncPhase.Delete:
						return WorkType.PolicyInducedDelete;
					}
					throw new InvalidOperationException("unsupported sync phase");
				}
				else
				{
					switch (syncPhase)
					{
					case SyncPhase.Initial:
						return WorkType.MigrationInitial;
					case SyncPhase.Incremental:
						return WorkType.MigrationIncremental;
					case SyncPhase.Finalization:
						return WorkType.MigrationFinalization;
					case SyncPhase.Completed:
						throw new NotSupportedException("Completed migration subscriptions are supposed to be disabled and not added to SQM");
					default:
						throw new InvalidOperationException("unsupported sync phase: " + syncPhase);
					}
				}
			}
			else
			{
				switch (syncPhase)
				{
				case SyncPhase.Initial:
					return WorkType.AggregationInitial;
				case SyncPhase.Incremental:
					return WorkType.AggregationIncremental;
				default:
					throw new InvalidOperationException("unsupported sync phase: " + syncPhase);
				}
			}
		}

		internal static bool IsOneOffWorkType(WorkType workType)
		{
			if (workType != WorkType.AggregationSubscriptionSaved)
			{
				switch (workType)
				{
				case WorkType.OwaLogonTriggeredSyncNow:
				case WorkType.OwaActivityTriggeredSyncNow:
				case WorkType.OwaRefreshButtonTriggeredSyncNow:
				case WorkType.PeopleConnectionTriggered:
					return true;
				}
				return false;
			}
			return true;
		}

		internal static bool IsLightWeightWorkType(WorkType workType)
		{
			switch (workType)
			{
			case WorkType.AggregationSubscriptionSaved:
			case WorkType.AggregationIncremental:
			case WorkType.MigrationIncremental:
			case WorkType.OwaLogonTriggeredSyncNow:
			case WorkType.OwaActivityTriggeredSyncNow:
			case WorkType.OwaRefreshButtonTriggeredSyncNow:
			case WorkType.PeopleConnectionTriggered:
			case WorkType.PeopleConnectionIncremental:
			case WorkType.PolicyInducedDelete:
				return true;
			}
			return false;
		}

		internal void Initialize()
		{
			if (WorkTypeManager.initialized)
			{
				return;
			}
			this.Add(WorkType.AggregationSubscriptionSaved, new WorkTypeDefinition(WorkType.AggregationSubscriptionSaved, ContentAggregationConfig.SyncNowTime, ContentAggregationConfig.AggregationSubscriptionSavedSyncWeight, true));
			this.Add(WorkType.AggregationIncremental, new WorkTypeDefinition(WorkType.AggregationIncremental, ContentAggregationConfig.AggregationIncrementalSyncInterval, ContentAggregationConfig.AggregationIncrementalSyncWeight, false));
			this.Add(WorkType.AggregationInitial, new WorkTypeDefinition(WorkType.AggregationInitial, ContentAggregationConfig.AggregationInitialSyncInterval, ContentAggregationConfig.AggregationInitialSyncWeight, false));
			this.Add(WorkType.MigrationInitial, new WorkTypeDefinition(WorkType.MigrationInitial, ContentAggregationConfig.MigrationInitialSyncInterval, ContentAggregationConfig.MigrationInitialSyncWeight, false));
			this.Add(WorkType.MigrationIncremental, new WorkTypeDefinition(WorkType.MigrationIncremental, ContentAggregationConfig.MigrationIncrementalSyncInterval, ContentAggregationConfig.MigrationIncrementalSyncWeight, false));
			this.Add(WorkType.MigrationFinalization, new WorkTypeDefinition(WorkType.MigrationFinalization, ContentAggregationConfig.MigrationInitialSyncInterval, ContentAggregationConfig.MigrationFinalizationSyncWeight, true));
			this.Add(WorkType.OwaLogonTriggeredSyncNow, new WorkTypeDefinition(WorkType.OwaLogonTriggeredSyncNow, ContentAggregationConfig.OwaTriggeredSyncNowTime, ContentAggregationConfig.OwaLogonTriggeredSyncWeight, true));
			this.Add(WorkType.OwaRefreshButtonTriggeredSyncNow, new WorkTypeDefinition(WorkType.OwaRefreshButtonTriggeredSyncNow, ContentAggregationConfig.OwaTriggeredSyncNowTime, ContentAggregationConfig.OwaRefreshButtonTriggeredSyncWeight, true));
			this.Add(WorkType.OwaActivityTriggeredSyncNow, new WorkTypeDefinition(WorkType.OwaActivityTriggeredSyncNow, ContentAggregationConfig.OwaTriggeredSyncNowTime, ContentAggregationConfig.OwaSessionTriggeredSyncWeight, true));
			this.Add(WorkType.PeopleConnectionInitial, new WorkTypeDefinition(WorkType.PeopleConnectionInitial, ContentAggregationConfig.PeopleConnectionInitialSyncInterval, ContentAggregationConfig.PeopleConnectionInitialSyncWeight, false));
			this.Add(WorkType.PeopleConnectionTriggered, new WorkTypeDefinition(WorkType.PeopleConnectionTriggered, ContentAggregationConfig.PeopleConnectionTriggeredSyncInterval, ContentAggregationConfig.PeopleConnectionTriggeredSyncWeight, true));
			this.Add(WorkType.PeopleConnectionIncremental, new WorkTypeDefinition(WorkType.PeopleConnectionIncremental, ContentAggregationConfig.PeopleConnectionIncrementalSyncInterval, ContentAggregationConfig.PeopleConnectionIncrementalSyncWeight, false));
			this.Add(WorkType.PolicyInducedDelete, new WorkTypeDefinition(WorkType.PolicyInducedDelete, ContentAggregationConfig.OwaMailboxPolicyInducedDeleteInterval, ContentAggregationConfig.OwaMailboxPolicyInducedDeleteWeight, false));
			byte b = 0;
			foreach (WorkTypeDefinition workTypeDefinition in this.workTypeDefinitions.Values)
			{
				b += workTypeDefinition.Weight;
			}
			if (b != 100)
			{
				throw new NotSupportedException("Total weight of all work types must equal 100");
			}
			WorkTypeManager.initialized = true;
		}

		internal virtual WorkTypeDefinition GetWorkTypeDefinition(WorkType workType)
		{
			return this.workTypeDefinitions[workType];
		}

		internal void Add(WorkType workType, WorkTypeDefinition workTypeDefinition)
		{
			this.workTypeDefinitions.Add(workType, workTypeDefinition);
		}

		private readonly Dictionary<WorkType, WorkTypeDefinition> workTypeDefinitions = new Dictionary<WorkType, WorkTypeDefinition>();

		private static bool initialized = false;

		private static WorkTypeManager workTypeManager = new WorkTypeManager();
	}
}
