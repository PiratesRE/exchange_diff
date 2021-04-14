using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Exchange.Data.Directory.Sync.TenantRelocationSync;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[ObjectScope(new ConfigScopes[]
	{
		ConfigScopes.TenantLocal,
		ConfigScopes.TenantSubTree
	})]
	[Serializable]
	public sealed class TenantRelocationRequest : ADLegacyVersionableObject
	{
		internal override ADObjectSchema Schema
		{
			get
			{
				return TenantRelocationRequest.schema;
			}
		}

		internal override string MostDerivedObjectClass
		{
			get
			{
				return ExchangeConfigurationUnit.MostDerivedClass;
			}
		}

		public bool RelocationInProgress
		{
			get
			{
				return (bool)this[TenantRelocationRequestSchema.RelocationInProgress];
			}
			internal set
			{
				this[TenantRelocationRequestSchema.RelocationInProgress] = value;
			}
		}

		public TenantRelocationStatus RelocationStatus
		{
			get
			{
				return (TenantRelocationStatus)((byte)this[TenantRelocationRequestSchema.RelocationStatus]);
			}
		}

		public RelocationStatusDetailsSource RelocationStatusDetailsSource
		{
			get
			{
				return (RelocationStatusDetailsSource)((byte)this[TenantRelocationRequestSchema.RelocationStatusDetailsSource]);
			}
		}

		public RelocationStatusDetailsDestination RelocationStatusDetailsDestination
		{
			get
			{
				return (RelocationStatusDetailsDestination)((byte)this[TenantRelocationRequestSchema.RelocationStatusDetailsDestination]);
			}
			private set
			{
				this[TenantRelocationRequestSchema.RelocationStatusDetailsDestination] = value;
			}
		}

		internal RelocationStatusDetails RelocationStatusDetailsRaw
		{
			get
			{
				return (RelocationStatusDetails)((byte)this[TenantRelocationRequestSchema.RelocationStatusDetailsRaw]);
			}
			set
			{
				this[TenantRelocationRequestSchema.RelocationStatusDetailsRaw] = value;
			}
		}

		public bool Suspended
		{
			get
			{
				return (bool)this[TenantRelocationRequestSchema.Suspended];
			}
			internal set
			{
				this[TenantRelocationRequestSchema.Suspended] = value;
			}
		}

		public RelocationStateRequested RelocationStateRequested
		{
			get
			{
				return (RelocationStateRequested)((int)this[TenantRelocationRequestSchema.RelocationStateRequested]);
			}
			internal set
			{
				this[TenantRelocationRequestSchema.RelocationStateRequested] = (int)value;
			}
		}

		public RelocationError RelocationLastError
		{
			get
			{
				return (RelocationError)((int)this[TenantRelocationRequestSchema.RelocationLastError]);
			}
			internal set
			{
				this[TenantRelocationRequestSchema.RelocationLastError] = (int)value;
			}
		}

		public string TargetForest
		{
			get
			{
				return (string)this[TenantRelocationRequestSchema.TargetForest];
			}
			internal set
			{
				this[TenantRelocationRequestSchema.TargetForest] = value;
			}
		}

		public string SourceForest
		{
			get
			{
				return (string)this[TenantRelocationRequestSchema.SourceForest];
			}
			internal set
			{
				this[TenantRelocationRequestSchema.SourceForest] = value;
			}
		}

		internal string RelocationSourceForestRaw
		{
			get
			{
				return (string)this[TenantRelocationRequestSchema.RelocationSourceForestRaw];
			}
			set
			{
				this[TenantRelocationRequestSchema.RelocationSourceForestRaw] = value;
			}
		}

		public string GLSResolvedForest
		{
			get
			{
				return (string)this[TenantRelocationRequestSchema.GLSResolvedForest];
			}
			internal set
			{
				this[TenantRelocationRequestSchema.GLSResolvedForest] = value;
			}
		}

		public string SourceForestRIDMaster
		{
			get
			{
				return (string)this[TenantRelocationRequestSchema.SourceForestRIDMaster];
			}
			internal set
			{
				this[TenantRelocationRequestSchema.SourceForestRIDMaster] = value;
			}
		}

		public string TargetForestRIDMaster
		{
			get
			{
				return (string)this[TenantRelocationRequestSchema.TargetForestRIDMaster];
			}
			internal set
			{
				this[TenantRelocationRequestSchema.TargetForestRIDMaster] = value;
			}
		}

		public OrganizationId TargetOrganizationId
		{
			get
			{
				return (OrganizationId)this[TenantRelocationRequestSchema.TargetOrganizationId];
			}
			internal set
			{
				this[TenantRelocationRequestSchema.TargetOrganizationId] = value;
			}
		}

		public string TargetOriginatingServer
		{
			get
			{
				return (string)this[TenantRelocationRequestSchema.TargetOriginatingServer];
			}
			internal set
			{
				this[TenantRelocationRequestSchema.TargetOriginatingServer] = value;
			}
		}

		public bool AutoCompletionEnabled
		{
			get
			{
				return (bool)this[TenantRelocationRequestSchema.AutoCompletionEnabled];
			}
			internal set
			{
				this[TenantRelocationRequestSchema.AutoCompletionEnabled] = value;
			}
		}

		public bool LargeTenantModeEnabled
		{
			get
			{
				return (bool)this[TenantRelocationRequestSchema.LargeTenantModeEnabled];
			}
			internal set
			{
				this[TenantRelocationRequestSchema.LargeTenantModeEnabled] = value;
			}
		}

		public DateTime? LockdownStartTime
		{
			get
			{
				return (DateTime?)this[TenantRelocationRequestSchema.LockdownStartTime];
			}
			internal set
			{
				this[TenantRelocationRequestSchema.LockdownStartTime] = value;
			}
		}

		public DateTime? RelocationSyncStartTime
		{
			get
			{
				return (DateTime?)this[TenantRelocationRequestSchema.RelocationSyncStartTime];
			}
			internal set
			{
				this[TenantRelocationRequestSchema.RelocationSyncStartTime] = value;
			}
		}

		public DateTime? RetiredStartTime
		{
			get
			{
				return (DateTime?)this[TenantRelocationRequestSchema.RetiredStartTime];
			}
			internal set
			{
				this[TenantRelocationRequestSchema.RetiredStartTime] = value;
			}
		}

		public MultiValuedProperty<TransitionCount> TransitionCounter
		{
			get
			{
				return (MultiValuedProperty<TransitionCount>)this[TenantRelocationRequestSchema.TransitionCounter];
			}
			internal set
			{
				this[TenantRelocationRequestSchema.TransitionCounter] = value;
			}
		}

		public Schedule SafeLockdownSchedule
		{
			get
			{
				return (Schedule)this[TenantRelocationRequestSchema.SafeLockdownSchedule];
			}
			internal set
			{
				this[TenantRelocationRequestSchema.SafeLockdownSchedule] = value;
			}
		}

		internal byte[] TenantSyncCookie
		{
			get
			{
				return (byte[])this[TenantRelocationRequestSchema.TenantSyncCookie];
			}
			set
			{
				this[TenantRelocationRequestSchema.TenantSyncCookie] = value;
			}
		}

		public DateTime? LastSuccessfulRelocationSyncStart
		{
			get
			{
				return (DateTime?)this[TenantRelocationRequestSchema.LastSuccessfulRelocationSyncStart];
			}
			internal set
			{
				this[TenantRelocationRequestSchema.LastSuccessfulRelocationSyncStart] = value;
			}
		}

		internal byte[] TenantRelocationCompletionTargetVector
		{
			get
			{
				return (byte[])this[TenantRelocationRequestSchema.TenantRelocationCompletionTargetVector];
			}
			set
			{
				this[TenantRelocationRequestSchema.TenantRelocationCompletionTargetVector] = value;
			}
		}

		public new string Name
		{
			get
			{
				return base.OrganizationId.OrganizationalUnit.Name;
			}
		}

		public string ExternalDirectoryOrganizationId
		{
			get
			{
				return (string)this[TenantRelocationRequestSchema.ExternalDirectoryOrganizationId];
			}
		}

		public string ServicePlan
		{
			get
			{
				return (string)this[TenantRelocationRequestSchema.ServicePlan];
			}
		}

		public ADObjectId ExchangeUpgradeBucket
		{
			get
			{
				return (ADObjectId)this[TenantRelocationRequestSchema.ExchangeUpgradeBucket];
			}
		}

		public OrganizationStatus OrganizationStatus
		{
			get
			{
				return (OrganizationStatus)((int)this[TenantRelocationRequestSchema.OrganizationStatus]);
			}
			internal set
			{
				this[TenantRelocationRequestSchema.OrganizationStatus] = (int)value;
			}
		}

		public OrganizationStatus TargetOrganizationStatus
		{
			get
			{
				return (OrganizationStatus)((int)this[TenantRelocationRequestSchema.TargetOrganizationStatus]);
			}
			internal set
			{
				this[TenantRelocationRequestSchema.TargetOrganizationStatus] = value;
			}
		}

		public ExchangeObjectVersion AdminDisplayVersion
		{
			get
			{
				return (ExchangeObjectVersion)this[TenantRelocationRequestSchema.AdminDisplayVersion];
			}
		}

		internal new string AdminDisplayName
		{
			get
			{
				return null;
			}
		}

		static TenantRelocationRequest()
		{
			TenantRelocationRequest.InFlightRelocationRequestsFilter = new AndFilter(new QueryFilter[]
			{
				TenantRelocationRequest.TenantRelocationRequestFilter,
				new ExistsFilter(TenantRelocationRequestSchema.RelocationSyncStartTime)
			});
			TenantRelocationRequest.JustStartedRelocationRequestsFilter = new AndFilter(new QueryFilter[]
			{
				TenantRelocationRequest.TenantRelocationRequestFilter,
				new NotFilter(new ExistsFilter(TenantRelocationRequestSchema.RelocationSyncStartTime))
			});
		}

		internal static object GetRelocationStatus(IPropertyBag propertyBag)
		{
			RelocationStatusDetails r = (RelocationStatusDetails)propertyBag[TenantRelocationRequestSchema.RelocationStatusDetailsRaw];
			return TenantRelocationRequest.GetRelocationStatusFromStatusDetails(r);
		}

		internal static TenantRelocationStatus GetRelocationStatusFromStatusDetails(RelocationStatusDetails r)
		{
			if (r <= RelocationStatusDetails.SynchronizationFinishedDeltaSync)
			{
				if (r > RelocationStatusDetails.InitializationFinished)
				{
					if (r <= RelocationStatusDetails.SynchronizationFinishedFullSync)
					{
						if (r != RelocationStatusDetails.SynchronizationStartedFullSync && r != RelocationStatusDetails.SynchronizationFinishedFullSync)
						{
							goto IL_9E;
						}
					}
					else if (r != RelocationStatusDetails.SynchronizationStartedDeltaSync && r != RelocationStatusDetails.SynchronizationFinishedDeltaSync)
					{
						goto IL_9E;
					}
					return TenantRelocationStatus.Synchronization;
				}
				if (r == RelocationStatusDetails.NotStarted || r == RelocationStatusDetails.InitializationStarted || r == RelocationStatusDetails.InitializationFinished)
				{
					return TenantRelocationStatus.NotStarted;
				}
			}
			else
			{
				if (r <= RelocationStatusDetails.LockdownSwitchedGLS)
				{
					if (r <= RelocationStatusDetails.LockdownStartedFinalDeltaSync)
					{
						if (r != RelocationStatusDetails.LockdownStarted && r != RelocationStatusDetails.LockdownStartedFinalDeltaSync)
						{
							goto IL_9E;
						}
					}
					else if (r != RelocationStatusDetails.LockdownFinishedFinalDeltaSync && r != RelocationStatusDetails.LockdownSwitchedGLS)
					{
						goto IL_9E;
					}
					return TenantRelocationStatus.Lockdown;
				}
				if (r <= RelocationStatusDetails.RetiredUpdatedTargetForest)
				{
					if (r == RelocationStatusDetails.RetiredUpdatedSourceForest || r == RelocationStatusDetails.RetiredUpdatedTargetForest)
					{
						return TenantRelocationStatus.Retired;
					}
				}
				else
				{
					if (r == RelocationStatusDetails.Arriving)
					{
						return TenantRelocationStatus.Arriving;
					}
					if (r == RelocationStatusDetails.Active)
					{
						return TenantRelocationStatus.Active;
					}
				}
			}
			IL_9E:
			throw new DataValidationException(new PropertyValidationError(DirectoryStrings.CannotCalculateProperty("RelocationStatus", new ArgumentOutOfRangeException(r.ToString()).Message), TenantRelocationRequestSchema.RelocationStatus, r));
		}

		internal static object GetRelocationStatusDetailsSource(IPropertyBag propertyBag)
		{
			object obj = propertyBag[TenantRelocationRequestSchema.RelocationStatusDetailsRaw];
			if (obj == null)
			{
				throw new DataValidationException(new PropertyValidationError(DirectoryStrings.CannotCalculateProperty("RelocationStatusDetailsOnSource", new ArgumentNullException("RelocationStatusDetailsRaw").Message), TenantRelocationRequestSchema.RelocationStatusDetailsSource, propertyBag));
			}
			return (RelocationStatusDetailsSource)obj;
		}

		internal static object GetTransitionCounter(IPropertyBag propertyBag)
		{
			object obj = propertyBag[TenantRelocationRequestSchema.TransitionCounterRaw];
			if (obj == null)
			{
				return null;
			}
			MultiValuedProperty<int> multiValuedProperty = (MultiValuedProperty<int>)obj;
			MultiValuedProperty<TransitionCount> multiValuedProperty2 = new MultiValuedProperty<TransitionCount>();
			for (int i = 0; i < multiValuedProperty.Count; i++)
			{
				int num = multiValuedProperty[i];
				TenantRelocationTransition transition = (TenantRelocationTransition)((num & TenantRelocationRequest.TransitionCounterTypeBitmask) >> 16);
				ushort count = (ushort)(num & TenantRelocationRequest.TransitionCounterCountBitmask);
				multiValuedProperty2.Add(new TransitionCount(transition, count));
			}
			multiValuedProperty2.Sort();
			return multiValuedProperty2;
		}

		internal static void SetTransitionCounter(object value, IPropertyBag propertyBag)
		{
			MultiValuedProperty<int> multiValuedProperty = new MultiValuedProperty<int>();
			foreach (TransitionCount transitionCount in ((MultiValuedProperty<TransitionCount>)value))
			{
				int item = (int)((ushort)((int)transitionCount.Transition << 16) | transitionCount.Count);
				multiValuedProperty.Add(item);
			}
			propertyBag[TenantRelocationRequestSchema.TransitionCounterRaw] = multiValuedProperty;
		}

		internal static QueryFilter GetStaleLockedRelocationRequestsFilter(ExDateTime olderThan, bool excludeSuspended)
		{
			ComparisonFilter comparisonFilter = excludeSuspended ? new ComparisonFilter(ComparisonOperator.Equal, TenantRelocationRequestSchema.Suspended, false) : null;
			string propertyValue = olderThan.ToString("yyyyMMddHHmmss'.0Z'", CultureInfo.InvariantCulture);
			return QueryFilter.AndTogether(new QueryFilter[]
			{
				TenantRelocationRequest.LockedRelocationRequestsFilter,
				new ComparisonFilter(ComparisonOperator.LessThanOrEqual, TenantRelocationRequestSchema.LockdownStartTime, propertyValue),
				comparisonFilter
			});
		}

		internal static void PopulatePresentationObject(TenantRelocationRequest presentationObject, string targetForestDomainController, out Exception ex)
		{
			TenantRelocationRequest targetForestObject;
			TenantRelocationRequest.LoadTargetForestObject(presentationObject, targetForestDomainController, out targetForestObject, out ex);
			if (ex == null)
			{
				TenantRelocationRequest.PopulatePresentationObject(presentationObject, targetForestObject);
			}
		}

		internal static void SetRelocationCompletedOnOU(ITenantConfigurationSession session, OrganizationId organizationId)
		{
			bool useConfigNC = session.UseConfigNC;
			try
			{
				session.UseConfigNC = false;
				ADOrganizationalUnit adorganizationalUnit = session.Read<ADOrganizationalUnit>(organizationId.OrganizationalUnit);
				if (adorganizationalUnit == null)
				{
					throw new ArgumentException("Cannot read target tenant OU: " + organizationId.OrganizationalUnit.ToString());
				}
				adorganizationalUnit.RelocationInProgress = true;
				session.Save(adorganizationalUnit);
				adorganizationalUnit.RelocationInProgress = false;
				session.Save(adorganizationalUnit);
			}
			finally
			{
				session.UseConfigNC = useConfigNC;
			}
		}

		private static void LoadTargetForestObject(TenantRelocationRequest presentationObject, string targetForestDomainController, out TenantRelocationRequest targetForestObject, out Exception ex)
		{
			if (presentationObject == null)
			{
				throw new ArgumentNullException("presentationObject");
			}
			if (presentationObject.TargetForest == null)
			{
				throw new ADTransientException(DirectoryStrings.ErrorReplicationLatency);
			}
			TenantRelocationRequest.LoadOtherForestObjectInternal(targetForestDomainController, presentationObject.TargetForest, presentationObject.DistinguishedName, presentationObject.ExternalDirectoryOrganizationId, true, out targetForestObject, out ex);
		}

		internal static void LoadOtherForestObjectInternal(string dc, string searchForest, string originalObjectDN, string externalDirectoryOrganizationId, bool needTargetTenant, out TenantRelocationRequest otherForestObject, out Exception ex)
		{
			if (string.IsNullOrEmpty(searchForest))
			{
				throw new ArgumentNullException("searchForest");
			}
			if (string.IsNullOrEmpty(originalObjectDN))
			{
				throw new ArgumentNullException("originalObjectDN");
			}
			if (string.IsNullOrEmpty(externalDirectoryOrganizationId))
			{
				throw new ArgumentNullException("externalDirectoryOrganizationId");
			}
			ex = null;
			otherForestObject = null;
			PartitionId partitionId = new PartitionId(searchForest);
			ITenantConfigurationSession tenantConfigurationSession = DirectorySessionFactory.NonCacheSessionFactory.CreateTenantConfigurationSession(dc, false, ConsistencyMode.PartiallyConsistent, null, ADSessionSettings.FromAllTenantsPartitionId(partitionId), 1539, "LoadOtherForestObjectInternal", "f:\\15.00.1497\\sources\\dev\\data\\src\\directory\\SystemConfiguration\\TenantRelocationRequest.cs");
			tenantConfigurationSession.SessionSettings.TenantConsistencyMode = TenantConsistencyMode.IncludeRetiredTenants;
			QueryFilter queryFilter;
			if (needTargetTenant)
			{
				queryFilter = TenantRelocationRequest.TenantRelocationLandingFilter;
			}
			else
			{
				queryFilter = TenantRelocationRequest.TenantRelocationRequestFilter;
			}
			QueryFilter filter = new AndFilter(new QueryFilter[]
			{
				new ComparisonFilter(ComparisonOperator.Equal, ExchangeConfigurationUnitSchema.ExternalDirectoryOrganizationId, externalDirectoryOrganizationId),
				queryFilter
			});
			try
			{
				TenantRelocationRequest[] array = tenantConfigurationSession.Find<TenantRelocationRequest>(null, QueryScope.SubTree, filter, null, 2);
				if (array.Length > 1)
				{
					ex = new ADOperationException(DirectoryStrings.ErrorTargetPartitionHas2TenantsWithSameId(originalObjectDN, searchForest, externalDirectoryOrganizationId));
				}
				else if (array.Length > 0)
				{
					otherForestObject = array[0];
				}
				else
				{
					ex = new CannotFindTargetTenantException(originalObjectDN, searchForest, externalDirectoryOrganizationId);
				}
			}
			catch (ADTransientException ex2)
			{
				ex = ex2;
			}
		}

		internal static void PopulatePresentationObject(TenantRelocationRequest presentationObject, TenantRelocationRequest targetForestObject)
		{
			if (presentationObject == null)
			{
				throw new ArgumentNullException("presentationObject");
			}
			if (targetForestObject == null)
			{
				throw new ArgumentNullException("targetForestObject");
			}
			presentationObject.RelocationStatusDetailsDestination = (RelocationStatusDetailsDestination)targetForestObject.RelocationStatusDetailsRaw;
			presentationObject.SourceForest = targetForestObject.RelocationSourceForestRaw;
			presentationObject.TargetOriginatingServer = targetForestObject.OriginatingServer;
			presentationObject.TargetOrganizationStatus = targetForestObject.OrganizationStatus;
			if (targetForestObject.OrganizationId != OrganizationId.ForestWideOrgId)
			{
				presentationObject.TargetOrganizationId = targetForestObject.OrganizationId;
			}
			presentationObject.RelocationInProgress = presentationObject.IsRelocationInProgress();
		}

		internal bool HasPermanentError()
		{
			return this.RelocationLastError > RelocationError.LastTransientError;
		}

		internal bool IsLockdownAllowed()
		{
			if (TenantRelocationStateCache.IgnoreRelocationTimeConstraints())
			{
				return true;
			}
			DateTime utcNow = DateTime.UtcNow;
			if (this.RelocationSyncStartTime != null && this.RelocationSyncStartTime.Value.ToUniversalTime() + TimeSpan.FromHours((double)TenantRelocationRequest.MinTimeBetweenRelocationStartAndLockdownHours) > utcNow)
			{
				return false;
			}
			if (this.SafeLockdownSchedule == null || this.SafeLockdownSchedule == Schedule.Always)
			{
				return true;
			}
			int num = 0;
			uint num2;
			if (TenantRelocationSyncCoordinator.GetInt32ValueFromRegistryValue("TimeZoneOffset", out num2))
			{
				num = (int)num2;
			}
			return this.SafeLockdownSchedule.Contains(utcNow.AddHours((double)num));
		}

		internal bool IsLockdownTimedOut()
		{
			return !this.Suspended && this.InLockdown() && this.LockdownStartTime != null && this.LockdownStartTime.Value.ToUniversalTime() + TimeSpan.FromMinutes((double)TenantRelocationRequest.MaxLockdownTimeMinutes) < DateTime.UtcNow && !TenantRelocationStateCache.IgnoreRelocationTimeConstraints();
		}

		internal bool IsRetiredSourceHoldTimedOut()
		{
			return TenantRelocationStateCache.IgnoreRelocationTimeConstraints() || (this.RetiredStartTime != null && this.RetiredStartTime.Value.ToUniversalTime() + TimeSpan.FromDays((double)TenantRelocationRequest.WaitTimeBeforeRemoveSourceReplicaDays) < DateTime.UtcNow);
		}

		internal bool IsRelocationInProgress()
		{
			return !this.HasPermanentError() && !this.Suspended && RelocationStatusDetailsSource.RetiredUpdatedTargetForest != this.RelocationStatusDetailsSource && (this.AutoCompletionEnabled || this.RelocationStatusDetailsSource < (RelocationStatusDetailsSource)this.RelocationStateRequested);
		}

		internal bool InLockdown()
		{
			RelocationStatusDetailsSource relocationStatusDetailsSource = this.RelocationStatusDetailsSource;
			if (relocationStatusDetailsSource <= RelocationStatusDetailsSource.LockdownStartedFinalDeltaSync)
			{
				if (relocationStatusDetailsSource != RelocationStatusDetailsSource.LockdownStarted && relocationStatusDetailsSource != RelocationStatusDetailsSource.LockdownStartedFinalDeltaSync)
				{
					return false;
				}
			}
			else if (relocationStatusDetailsSource != RelocationStatusDetailsSource.LockdownFinishedFinalDeltaSync && relocationStatusDetailsSource != RelocationStatusDetailsSource.LockdownSwitchedGLS && relocationStatusDetailsSource != RelocationStatusDetailsSource.RetiredUpdatedSourceForest)
			{
				return false;
			}
			return true;
		}

		internal bool InLockdownBeforeGLSSwitchState()
		{
			RelocationStatusDetailsSource relocationStatusDetailsSource = this.RelocationStatusDetailsSource;
			return relocationStatusDetailsSource == RelocationStatusDetailsSource.LockdownStarted || relocationStatusDetailsSource == RelocationStatusDetailsSource.LockdownStartedFinalDeltaSync || relocationStatusDetailsSource == RelocationStatusDetailsSource.LockdownFinishedFinalDeltaSync;
		}

		internal bool InPostGLSSwitchState()
		{
			RelocationStatusDetailsSource relocationStatusDetailsSource = this.RelocationStatusDetailsSource;
			return relocationStatusDetailsSource == RelocationStatusDetailsSource.LockdownSwitchedGLS || relocationStatusDetailsSource == RelocationStatusDetailsSource.RetiredUpdatedSourceForest || relocationStatusDetailsSource == RelocationStatusDetailsSource.RetiredUpdatedTargetForest;
		}

		internal bool IsOrchestrated()
		{
			return !this.Suspended && !this.AutoCompletionEnabled && !this.HasPermanentError() && this.ExchangeUpgradeBucket != null && this.ExchangeUpgradeBucket.Name.StartsWith("Relocation");
		}

		internal bool HasTooManyTransitions(out TransitionCount transitionCount)
		{
			transitionCount = null;
			if (this.TransitionCounter != null)
			{
				int config = TenantRelocationConfigImpl.GetConfig<int>("MaxNumberOfTransitions");
				foreach (TransitionCount transitionCount2 in this.TransitionCounter)
				{
					if ((int)transitionCount2.Count > config)
					{
						transitionCount = transitionCount2;
						return true;
					}
				}
				return false;
			}
			return false;
		}

		internal void IncrementTransitionCounter(TenantRelocationTransition transition)
		{
			MultiValuedProperty<TransitionCount> multiValuedProperty = new MultiValuedProperty<TransitionCount>();
			bool flag = false;
			foreach (TransitionCount transitionCount in this.TransitionCounter)
			{
				if (transitionCount.Transition == transition)
				{
					TransitionCount transitionCount2 = transitionCount;
					transitionCount2.Count += 1;
					flag = true;
				}
				multiValuedProperty.Add(transitionCount);
			}
			if (!flag)
			{
				multiValuedProperty.Add(new TransitionCount(transition, 1));
			}
			this.TransitionCounter = multiValuedProperty;
		}

		protected override void ValidateRead(List<ValidationError> errors)
		{
			if (!string.IsNullOrEmpty(this.RelocationSourceForestRaw) && !string.IsNullOrEmpty(this.TargetForest))
			{
				errors.Add(new ObjectValidationError(DirectoryStrings.ErrorBothTargetAndSourceForestPopulated(this.RelocationSourceForestRaw, this.TargetForest), this.Identity, string.Empty));
			}
			if ((!string.IsNullOrEmpty(this.RelocationSourceForestRaw) || !string.IsNullOrEmpty(this.TargetForest)) && this.RelocationStatusDetailsRaw == RelocationStatusDetails.NotStarted)
			{
				errors.Add(new ObjectValidationError(DirectoryStrings.ErrorTargetOrSourceForestPopulatedStatusNotStarted(this.RelocationSourceForestRaw, this.TargetForest), this.Identity, string.Empty));
			}
			if (this.TransitionCounter != null)
			{
				HashSet<TenantRelocationTransition> hashSet = new HashSet<TenantRelocationTransition>();
				foreach (TransitionCount transitionCount in this.TransitionCounter)
				{
					if (!hashSet.Contains(transitionCount.Transition))
					{
						hashSet.Add(transitionCount.Transition);
					}
					else
					{
						errors.Add(new ObjectValidationError(DirectoryStrings.ErrorTransitionCounterHasDuplicateEntry(transitionCount.Transition.ToString()), this.Identity, string.Empty));
					}
				}
			}
			base.ValidateRead(errors);
		}

		internal const string Noun = "TenantRelocationRequest";

		internal const string RelocationBucketNamePrefix = "Relocation";

		private static readonly int TransitionCounterTypeBitmask = Convert.ToInt32("00000000111111110000000000000000", 2);

		private static readonly int TransitionCounterCountBitmask = Convert.ToInt32("00000000000000001111111111111111", 2);

		private static readonly TenantRelocationRequestSchema schema = ObjectSchema.GetInstance<TenantRelocationRequestSchema>();

		internal static readonly int MinTimeBetweenRelocationStartAndLockdownHours = 24;

		internal static readonly int WaitTimeBeforeRemoveSourceReplicaDays = 30;

		internal static readonly int MaxLockdownTimeMinutes = 30;

		internal static readonly QueryFilter TenantRelocationRequestFilter = new ExistsFilter(TenantRelocationRequestSchema.TargetForest);

		internal static readonly QueryFilter LockedRelocationRequestsFilter = new OrFilter(new QueryFilter[]
		{
			new ComparisonFilter(ComparisonOperator.Equal, TenantRelocationRequestSchema.RelocationStatusDetailsRaw, RelocationStatusDetails.LockdownStarted),
			new ComparisonFilter(ComparisonOperator.Equal, TenantRelocationRequestSchema.RelocationStatusDetailsRaw, RelocationStatusDetails.LockdownStartedFinalDeltaSync),
			new ComparisonFilter(ComparisonOperator.Equal, TenantRelocationRequestSchema.RelocationStatusDetailsRaw, RelocationStatusDetails.LockdownFinishedFinalDeltaSync),
			new ComparisonFilter(ComparisonOperator.Equal, TenantRelocationRequestSchema.RelocationStatusDetailsRaw, RelocationStatusDetails.LockdownSwitchedGLS),
			new ComparisonFilter(ComparisonOperator.Equal, TenantRelocationRequestSchema.RelocationStatusDetailsRaw, RelocationStatusDetails.RetiredUpdatedSourceForest)
		});

		internal static readonly QueryFilter InFlightRelocationRequestsFilter;

		internal static readonly QueryFilter JustStartedRelocationRequestsFilter;

		internal static readonly QueryFilter TenantRelocationLandingFilter = new ExistsFilter(TenantRelocationRequestSchema.RelocationSourceForestRaw);
	}
}
