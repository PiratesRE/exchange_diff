using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Sync;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.BackSync.Processors
{
	internal class OrganizationLookup : PropertyCache, IPropertyLookup
	{
		public OrganizationLookup(Func<ADObjectId[], PropertyDefinition[], Result<ADRawEntry>[]> getOrganizationProperties, QueryFilter scopeFilter) : base(getOrganizationProperties, OrganizationLookup.OrganizationProperties)
		{
			this.scopeFilter = scopeFilter;
		}

		public override IEnumerable<ADObjectId> GetObjectIds(PropertyBag propertyBag)
		{
			return new ADObjectId[]
			{
				ProcessorHelper.GetTenantOU(propertyBag)
			};
		}

		protected override bool MeetsAdditionalCriteria(ADRawEntry entry)
		{
			return OpathFilterEvaluator.FilterMatches(this.scopeFilter, entry);
		}

		private static readonly ADPropertyDefinition[] OrganizationProperties = new ADPropertyDefinition[]
		{
			ExchangeConfigurationUnitSchema.TargetForest,
			ExchangeConfigurationUnitSchema.RelocationSourceForestRaw,
			ExchangeConfigurationUnitSchema.WhenOrganizationStatusSet,
			ExchangeConfigurationUnitSchema.ExternalDirectoryOrganizationId,
			ExchangeConfigurationUnitSchema.OrganizationStatus,
			OrganizationSchema.ExcludedFromBackSync,
			ExchangeConfigurationUnitSchema.ProgramId,
			SyncObjectSchema.Deleted,
			OrganizationSchema.IsDirSyncRunning,
			OrganizationSchema.DirSyncStatus,
			TenantRelocationRequestSchema.TenantRelocationCompletionTargetVector
		};

		private readonly QueryFilter scopeFilter;
	}
}
