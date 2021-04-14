using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.Sync;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics.Components.BackSync;

namespace Microsoft.Exchange.Management.BackSync.Processors
{
	internal class TenantRelocationProcessor : PipelineProcessor
	{
		public TenantRelocationProcessor(IDataProcessor next, IPropertyLookup organizationPropertyLookup, ExcludedObjectReporter reporter, GetTenantRelocationStateDelegate getTenantRelocationState, Guid invocationId, bool isIncrementalSync) : base(next)
		{
			this.organizationPropertyLookup = organizationPropertyLookup;
			this.reporter = reporter;
			this.getTenantRelocationState = getTenantRelocationState;
			this.invocationId = invocationId;
			this.isIncrementalSync = isIncrementalSync;
		}

		protected override bool ProcessInternal(PropertyBag propertyBag)
		{
			ADObjectId tenantOU = ProcessorHelper.GetTenantOU(propertyBag);
			ADRawEntry properties = this.organizationPropertyLookup.GetProperties(tenantOU);
			if (!TenantRelocationProcessor.IsDeletedOrg(properties, tenantOU) && TenantRelocationProcessor.HasBeenInvolvedInRelocation(properties))
			{
				bool isSourceOfRelocation;
				TenantRelocationState tenantRelocationState = this.getTenantRelocationState(tenantOU, out isSourceOfRelocation, false);
				bool flag = tenantRelocationState.SourceForestState == TenantRelocationStatus.Retired;
				if (!flag || tenantRelocationState.SourceForestState == TenantRelocationStatus.Lockdown)
				{
					tenantRelocationState = this.getTenantRelocationState(tenantOU, out isSourceOfRelocation, true);
					flag = (tenantRelocationState.SourceForestState == TenantRelocationStatus.Retired);
				}
				return this.HandleRelocationState(flag, isSourceOfRelocation, propertyBag, properties);
			}
			return true;
		}

		private static bool HasBeenInvolvedInRelocation(ADRawEntry org)
		{
			return org != null && (!string.IsNullOrEmpty((string)org[ExchangeConfigurationUnitSchema.TargetForest]) || !string.IsNullOrEmpty((string)org[ExchangeConfigurationUnitSchema.RelocationSourceForestRaw]));
		}

		private static bool IsDeletedOrg(ADRawEntry org, ADObjectId ouId)
		{
			return (org != null && org.Id.IsDeleted) || (ouId != null && ouId.IsDeleted);
		}

		private static WatermarkMap GetVectorToFilterRelocationData(ADRawEntry org)
		{
			WatermarkMap watermarkMap = WatermarkMap.Empty;
			byte[] array = (byte[])org[TenantRelocationRequestSchema.TenantRelocationCompletionTargetVector];
			if (array != null)
			{
				try
				{
					watermarkMap = WatermarkMap.Parse(array);
				}
				catch (FormatException arg)
				{
					ExTraceGlobals.BackSyncTracer.TraceError<ADObjectId, FormatException>((long)SyncConfiguration.TraceId, "TenantRelocationProcessor::GetVectorToFilterRelocationData - Error parsing relocation completion vector tenant {0}. Error {1}.", org.Id, arg);
				}
			}
			ExTraceGlobals.BackSyncTracer.TraceDebug<ADObjectId, string>((long)SyncConfiguration.TraceId, "TenantRelocationProcessor::GetVectorToFilterRelocationData - Relocation completion vector found for tenant {0} is:\\n{1}.", org.Id, watermarkMap.SerializeToString());
			return watermarkMap;
		}

		private bool HandleRelocationState(bool isRelocationCompleted, bool isSourceOfRelocation, PropertyBag propertyBag, ADRawEntry org)
		{
			if (!isRelocationCompleted)
			{
				if (!isSourceOfRelocation)
				{
					this.reporter.ReportExcludedObject(propertyBag, DirectoryObjectErrorCode.ObjectOutOfScope, ProcessingStage.RelocationStageFilter);
				}
				return isSourceOfRelocation;
			}
			if (!isSourceOfRelocation)
			{
				if (this.isIncrementalSync)
				{
					WatermarkMap vectorToFilterRelocationData = TenantRelocationProcessor.GetVectorToFilterRelocationData(org);
					if (vectorToFilterRelocationData.ContainsKey(this.invocationId))
					{
						long num = vectorToFilterRelocationData[this.invocationId];
						if ((long)(propertyBag[ADRecipientSchema.UsnChanged] ?? 9223372036854775807L) < num)
						{
							this.reporter.ReportExcludedObject(propertyBag, DirectoryObjectErrorCode.ObjectOutOfScope, ProcessingStage.RelocationPartOfRelocationSyncFilter);
							return false;
						}
					}
				}
				return true;
			}
			if (ProcessorHelper.IsObjectOrganizationUnit(propertyBag))
			{
				this.reporter.ReportExcludedObject(propertyBag, DirectoryObjectErrorCode.ObjectOutOfScope, ProcessingStage.RelocationStageFilter);
				return false;
			}
			ServiceInstanceId value = new ServiceInstanceId(string.Format("exchange/{0}", org[ExchangeConfigurationUnitSchema.TargetForest]));
			propertyBag.SetField(SyncObjectSchema.FaultInServiceInstance, value);
			return true;
		}

		private readonly IPropertyLookup organizationPropertyLookup;

		private readonly ExcludedObjectReporter reporter;

		private readonly GetTenantRelocationStateDelegate getTenantRelocationState;

		private readonly Guid invocationId;

		private readonly bool isIncrementalSync;
	}
}
