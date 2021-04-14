using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics.Components.BackSync;

namespace Microsoft.Exchange.Data.Directory.Sync
{
	internal class DirSyncBasedTenantFullSyncConfiguration : IncrementalSyncConfiguration
	{
		public TenantFullSyncPageToken FullSyncPageToken { get; private set; }

		public ExchangeConfigurationUnit OrganizationCU { get; private set; }

		public DirSyncBasedTenantFullSyncConfiguration(TenantFullSyncPageToken pageToken, ExchangeConfigurationUnit tenantFullSyncOrganizationCU, Guid invocationId, OutputResultDelegate writeResult, ISyncEventLogger eventLogger, IExcludedObjectReporter excludedObjectReporter) : base(pageToken.TenantScopedBackSyncCookie, invocationId, writeResult, eventLogger, excludedObjectReporter)
		{
			if (pageToken == null)
			{
				throw new ArgumentNullException("pageToken");
			}
			ExTraceGlobals.BackSyncTracer.TraceDebug((long)SyncConfiguration.TraceId, "New DirSyncBasedTenantFullSyncConfiguration");
			this.FullSyncPageToken = pageToken;
			this.OrganizationCU = tenantFullSyncOrganizationCU;
		}

		protected override QueryFilter GetDirSyncQueryFilter()
		{
			QueryFilter dirSyncQueryFilter = base.GetDirSyncQueryFilter();
			ExTraceGlobals.BackSyncTracer.TraceDebug((long)SyncConfiguration.TraceId, "DirSyncBasedTenantFullSyncConfiguration.GetDirSyncQueryFilter entering");
			return new AndFilter(new QueryFilter[]
			{
				dirSyncQueryFilter,
				new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.ConfigurationUnit, this.OrganizationCU.ConfigurationUnit)
			});
		}

		protected override void CheckForFullSyncFallback()
		{
		}

		public override byte[] GetResultCookie()
		{
			byte[] resultCookie = base.GetResultCookie();
			if (resultCookie != null)
			{
				this.FullSyncPageToken.TenantScopedBackSyncCookie = BackSyncCookie.Parse(resultCookie);
			}
			return this.FullSyncPageToken.ToByteArray();
		}

		protected override void PrepareCookieForFailure()
		{
			base.PrepareCookieForFailure();
			this.FullSyncPageToken.TenantScopedBackSyncCookie = base.NewCookie;
		}

		protected override void ReturnErrorCookie()
		{
			ExTraceGlobals.BackSyncTracer.TraceDebug((long)SyncConfiguration.TraceId, "DirSyncBasedTenantFullSyncConfiguration.ReturnErrorCookie entering");
			base.WriteResult(this.FullSyncPageToken.ToByteArray(), SyncObject.CreateGetChangesResponse(new List<SyncObject>(), this.MoreData, this.FullSyncPageToken.ToByteArray(), this.FullSyncPageToken.ServiceInstanceId));
		}

		public override IEnumerable<ADRawEntry> GetDataPage()
		{
			IEnumerable<ADRawEntry> dataPage = base.GetDataPage();
			this.FullSyncPageToken.TenantScopedBackSyncCookie = base.NewCookie;
			if (!this.MoreData)
			{
				this.FinishFullSync();
			}
			return dataPage;
		}

		protected virtual void FinishFullSync()
		{
			this.FullSyncPageToken.FinishFullSync();
		}
	}
}
