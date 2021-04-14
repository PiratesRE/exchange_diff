using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Data.Directory.DirSync;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.BackSync;

namespace Microsoft.Exchange.Data.Directory.Sync
{
	internal class IncrementalSyncConfiguration : SyncConfiguration
	{
		internal IMissingPropertyResolver MissingPropertyResolver { get; set; }

		public IncrementalSyncConfiguration(BackSyncCookie syncCookie, Guid invocationId, OutputResultDelegate writeResult, ISyncEventLogger eventLogger, IExcludedObjectReporter excludedObjectReporter) : base(invocationId, writeResult, eventLogger, excludedObjectReporter)
		{
			ExTraceGlobals.BackSyncTracer.TraceDebug((long)SyncConfiguration.TraceId, "New IncrementalSyncConfiguration");
			this.syncCookie = syncCookie;
		}

		protected virtual DateTime GetLastWhenChanged()
		{
			ADRawEntry adrawEntry = (this.MissingPropertyResolver != null) ? this.MissingPropertyResolver.LastProcessedEntry : null;
			if (adrawEntry != null && adrawEntry[ADObjectSchema.WhenChangedUTC] != null)
			{
				return (DateTime)adrawEntry[ADObjectSchema.WhenChangedUTC];
			}
			return this.syncCookie.LastWhenChanged;
		}

		protected virtual void CheckForFullSyncFallback()
		{
			if (this.NewCookie.LastWhenChanged != DateTime.MinValue)
			{
				TimeSpan timeSpan = this.syncCookie.LastWhenChanged - this.NewCookie.LastWhenChanged;
				if (timeSpan >= IncrementalSyncConfiguration.FullSyncDetectionThreshold)
				{
					ExTraceGlobals.BackSyncTracer.TraceWarning<DateTime, DateTime, TimeSpan>((long)SyncConfiguration.TraceId, "IncrementalSyncConfiguration.CheckForFullSyncFallback detected full sync: Previous cookie LastWhenChanged: {0}, current cookie LastWhenChanged: {1}, difference: {2}", this.syncCookie.LastWhenChanged, this.NewCookie.LastWhenChanged, timeSpan);
					if (base.EventLogger != null)
					{
						base.EventLogger.LogFullSyncFallbackDetectedEvent(this.syncCookie, this.NewCookie);
					}
				}
			}
		}

		public override bool MoreData
		{
			get
			{
				return this.NewCookie != null && this.NewCookie.MoreDirSyncData;
			}
		}

		public BackSyncCookie NewCookie { get; private set; }

		public override Exception HandleException(Exception e)
		{
			ExTraceGlobals.BackSyncTracer.TraceDebug<string>((long)SyncConfiguration.TraceId, "IncrementalSyncConfiguration.HandleException {0}", e.ToString());
			if (base.IsTransientException(e))
			{
				this.PrepareCookieForFailure();
				this.ReturnErrorCookie();
				return new BackSyncDataSourceTransientException(e);
			}
			return e;
		}

		public override byte[] GetResultCookie()
		{
			this.NewCookie.LastWhenChanged = this.GetLastWhenChanged();
			this.CheckForFullSyncFallback();
			if (this.NewCookie == null)
			{
				return null;
			}
			return this.NewCookie.ToByteArray();
		}

		public override IEnumerable<ADRawEntry> GetDataPage()
		{
			this.NewCookie = this.syncCookie;
			Guid invocationId;
			bool flag;
			byte[] array;
			ADRawEntry[] dirSyncData = this.GetDirSyncData(out invocationId, out flag, out array);
			ExTraceGlobals.BackSyncTracer.TraceDebug<int>((long)SyncConfiguration.TraceId, "IncrementalSyncConfiguration.GetDataPage result count = {0}", (dirSyncData != null) ? dirSyncData.Length : 0);
			byte[] lastDirSyncCookieWithReplicationVectors = (this.syncCookie != null) ? this.syncCookie.LastDirSyncCookieWithReplicationVectors : null;
			if (!flag)
			{
				lastDirSyncCookieWithReplicationVectors = array;
			}
			this.NewCookie = new BackSyncCookie(DateTime.UtcNow, DateTime.MinValue, DateTime.MinValue, invocationId, flag, array, null, lastDirSyncCookieWithReplicationVectors, this.NewCookie.ServiceInstanceId, this.NewCookie.SequenceId, this.NewCookie.SequenceStartTimestamp);
			return dirSyncData;
		}

		public override void CheckIfConnectionAllowed()
		{
			ExTraceGlobals.BackSyncTracer.TraceDebug((long)SyncConfiguration.TraceId, "CheckIfConnectionAllowed entering");
			if (!string.IsNullOrEmpty(base.RecipientSession.DomainController))
			{
				ExTraceGlobals.BackSyncTracer.TraceDebug<string>((long)SyncConfiguration.TraceId, "this.RecipientSession.DomainController {0}", base.RecipientSession.DomainController);
				ADServer adserver = base.RootOrgConfigurationSession.FindDCByFqdn(base.RecipientSession.DomainController);
				PartitionId partitionId = base.RootOrgConfigurationSession.SessionSettings.PartitionId;
				if (adserver == null || !ConnectionPoolManager.IsServerInPreferredSite(partitionId.ForestFQDN, adserver))
				{
					ExTraceGlobals.BackSyncTracer.TraceError<string>((long)SyncConfiguration.TraceId, "DC site {0} not in preferred site list.", (adserver != null) ? adserver.Site.DistinguishedName : "<null>");
					throw new BackSyncDataSourceNotInPreferredSiteException((adserver != null) ? adserver.DistinguishedName : "<null>");
				}
			}
		}

		protected virtual void PrepareCookieForFailure()
		{
			DateTime utcNow = DateTime.UtcNow;
			DateTime lastReadFailureStartTime = base.IsSubsequentFailedAttempt() ? this.GetLastReadFailureStartTime() : utcNow;
			Guid guid = this.syncCookie.InvocationId;
			DateTime sequenceStartTimestamp = this.syncCookie.SequenceStartTimestamp;
			Guid sequenceId = this.syncCookie.SequenceId;
			if (base.IsSubsequentFailedAttempt() && base.IsFailoverTimeoutExceeded(utcNow))
			{
				guid = Guid.Empty;
				sequenceId = Guid.NewGuid();
				sequenceStartTimestamp = DateTime.UtcNow;
			}
			bool moreData = this.syncCookie.MoreDirSyncData;
			byte[] rawCookie = this.syncCookie.DirSyncCookie;
			if (guid == Guid.Empty && this.syncCookie.LastDirSyncCookieWithReplicationVectors != null)
			{
				moreData = true;
				rawCookie = this.syncCookie.LastDirSyncCookieWithReplicationVectors;
			}
			this.NewCookie = new BackSyncCookie(utcNow, lastReadFailureStartTime, this.GetLastWhenChanged(), guid, moreData, rawCookie, this.syncCookie.ErrorObjectsAndFailureCounts, this.syncCookie.LastDirSyncCookieWithReplicationVectors, this.syncCookie.ServiceInstanceId, sequenceId, sequenceStartTimestamp);
			base.UpdateSyncCookieErrorObjectsAndFailureCounts(this.NewCookie);
			this.CheckForFullSyncFallback();
		}

		protected override DateTime GetLastReadFailureStartTime()
		{
			return this.syncCookie.LastReadFailureStartTime;
		}

		protected override DateTime GetSyncSequenceStartTime()
		{
			return this.syncCookie.SequenceStartTimestamp;
		}

		protected override bool IsDCFailoverResilienceEnabled()
		{
			return SyncConfiguration.EnableDCFailoverResilienceForIncrementalSync();
		}

		protected virtual QueryFilter GetDirSyncQueryFilter()
		{
			return new OrFilter(new QueryFilter[]
			{
				new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.ObjectClass, ADUser.MostDerivedClass),
				new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.ObjectClass, ADGroup.MostDerivedClass),
				new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.ObjectClass, ADContact.MostDerivedClass),
				new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.ObjectClass, ADOrganizationalUnit.MostDerivedClass)
			});
		}

		protected virtual ADRawEntry[] GetDirSyncData(out Guid invocationId, out bool moreData, out byte[] dirSyncCookie)
		{
			QueryFilter dirSyncQueryFilter = this.GetDirSyncQueryFilter();
			IEnumerable<PropertyDefinition> allBackSyncProperties = SyncSchema.Instance.AllBackSyncProperties;
			IEnumerable<PropertyDefinition> allShadowProperties = SyncSchema.Instance.AllShadowProperties;
			IEnumerable<PropertyDefinition> enumerable = allBackSyncProperties.Concat(allShadowProperties);
			if (ExTraceGlobals.BackSyncTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				int num = 0;
				foreach (PropertyDefinition propertyDefinition in enumerable)
				{
					ExTraceGlobals.BackSyncTracer.TraceDebug<int, string>((long)SyncConfiguration.TraceId, "incrementalBacksyncProperties[{0}] {1}", num++, propertyDefinition.Name);
				}
			}
			ADSessionSettings adsessionSettings = ADSessionSettings.SessionSettingsFactory.Default.FromAccountPartitionWideScopeSet(base.RecipientSession.SessionSettings.PartitionId);
			adsessionSettings.IncludeSoftDeletedObjects = true;
			IRecipientSession tenantOrRootOrgRecipientSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(base.RecipientSession.DomainController, true, ConsistencyMode.PartiallyConsistent, adsessionSettings, 339, "GetDirSyncData", "f:\\15.00.1497\\sources\\dev\\data\\src\\directory\\Sync\\BackSync\\Configuration\\IncrementalSyncConfiguration.cs");
			tenantOrRootOrgRecipientSession.UseGlobalCatalog = false;
			ADDirSyncReader<BackSyncRecipient> addirSyncReader = new ADDirSyncReader<BackSyncRecipient>(tenantOrRootOrgRecipientSession, dirSyncQueryFilter, enumerable)
			{
				Cookie = this.syncCookie.DirSyncCookie
			};
			ADRawEntry[] nextResultPage = addirSyncReader.GetNextResultPage();
			invocationId = SyncConfiguration.FindInvocationIdByFqdn(addirSyncReader.PreferredServerName, base.RecipientSession.SessionSettings.PartitionId);
			moreData = (addirSyncReader.MorePagesAvailable != null && addirSyncReader.MorePagesAvailable.Value);
			dirSyncCookie = addirSyncReader.Cookie;
			return nextResultPage;
		}

		protected virtual void ReturnErrorCookie()
		{
			ExTraceGlobals.BackSyncTracer.TraceDebug((long)SyncConfiguration.TraceId, "ReturnErrorCookie entering");
			base.WriteResult(this.NewCookie.ToByteArray(), SyncObject.CreateGetChangesResponse(new List<SyncObject>(), this.NewCookie.MoreDirSyncData, this.NewCookie.ToByteArray(), this.NewCookie.ServiceInstanceId));
		}

		private const string FullSyncDetectionThresholdValueName = "FullSyncDetectionThreshold";

		private const int DefaultFullSyncDetectionThreshold = 30;

		private readonly BackSyncCookie syncCookie;

		private static readonly TimeSpan FullSyncDetectionThreshold = TimeSpan.FromDays((double)SyncConfiguration.GetConfigurationValue<int>("FullSyncDetectionThreshold", 30));
	}
}
