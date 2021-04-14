using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Sync;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics.Components.BackSync;

namespace Microsoft.Exchange.Management.BackSync.Processors
{
	internal class OrganizationFilter : PipelineProcessor
	{
		public OrganizationFilter(IDataProcessor next, IPropertyLookup organizationPropertyLookup, ExcludedObjectReporter reporter, bool relocationEnabled = false) : base(next)
		{
			this.organizationPropertyLookup = organizationPropertyLookup;
			this.reporter = reporter;
			this.relocationEnabled = relocationEnabled;
		}

		protected override bool ProcessInternal(PropertyBag propertyBag)
		{
			ADObjectId tenantOU = ProcessorHelper.GetTenantOU(propertyBag);
			if (tenantOU != null)
			{
				ADRawEntry properties = this.organizationPropertyLookup.GetProperties(tenantOU);
				if (properties != null)
				{
					string value = (string)properties[ExchangeConfigurationUnitSchema.ExternalDirectoryOrganizationId];
					if (!string.IsNullOrEmpty(value))
					{
						propertyBag.SetField(SyncObjectSchema.ContextId, value);
						if (ProcessorHelper.IsObjectOrganizationUnit(propertyBag))
						{
							if (this.ShouldIgnoreOrganizationUnit(propertyBag))
							{
								ExTraceGlobals.BackSyncTracer.TraceDebug((long)SyncConfiguration.TraceId, "<OrganizationFilter.ProcessInternal> tenant has no output attribute. Skip processing.");
								return false;
							}
							propertyBag.SetField(SyncObjectSchema.ObjectId, value);
						}
						propertyBag.SetField(OrganizationSchema.IsDirSyncRunning, properties[OrganizationSchema.IsDirSyncRunning]);
						propertyBag.SetField(OrganizationSchema.DirSyncStatus, properties[OrganizationSchema.DirSyncStatus]);
						if (OrganizationFilter.IsExcludedOrg(properties))
						{
							this.reporter.ReportExcludedObject(propertyBag, DirectoryObjectErrorCode.UnspecifiedError, ProcessingStage.OrganizationFilter);
							return false;
						}
						return true;
					}
				}
			}
			this.reporter.ReportExcludedObject(propertyBag, DirectoryObjectErrorCode.ObjectOutOfScope, ProcessingStage.OrganizationFilter);
			return false;
		}

		private bool ShouldIgnoreOrganizationUnit(PropertyBag propertyBag)
		{
			ExTraceGlobals.BackSyncTracer.TraceDebug((long)SyncConfiguration.TraceId, "<OrganizationFilter.ShouldIgnoreOrganizationUnit> entering");
			bool result = true;
			ADObjectId adobjectId = (ADObjectId)propertyBag[ADObjectSchema.Id];
			ExTraceGlobals.BackSyncTracer.TraceDebug<string>((long)SyncConfiguration.TraceId, "<OrganizationFilter.ShouldIgnoreOrganizationUnit> check tenant {0}", adobjectId.ToString());
			if (propertyBag.Contains(SyncCompanySchema.DirSyncStatusAck))
			{
				MultiValuedProperty<string> multiValuedProperty = (MultiValuedProperty<string>)propertyBag[SyncCompanySchema.DirSyncStatusAck];
				ExTraceGlobals.BackSyncTracer.TraceDebug<int>((long)SyncConfiguration.TraceId, "<OrganizationFilter.ShouldIgnoreOrganizationUnit> DirSyncStatusAck.count = {0}", multiValuedProperty.Count);
				if (multiValuedProperty.Count > 0)
				{
					ExTraceGlobals.BackSyncTracer.TraceDebug((long)SyncConfiguration.TraceId, "<OrganizationFilter.ShouldIgnoreOrganizationUnit> tenant SyncCompanySchema.DirSyncStatusAck is not NULL");
					result = false;
				}
			}
			else if (this.relocationEnabled && !ProcessorHelper.IsDeletedObject(propertyBag) && OrganizationFilter.IsRelocationCompletedSignal(propertyBag))
			{
				ExTraceGlobals.BackSyncTracer.TraceDebug((long)SyncConfiguration.TraceId, "<OrganizationFilter.ShouldIgnoreOrganizationUnit> tenant SyncCompanySchema.RelocationInProgress is changed to false");
				result = false;
			}
			else
			{
				ExTraceGlobals.BackSyncTracer.TraceDebug((long)SyncConfiguration.TraceId, "<OrganizationFilter.ShouldIgnoreOrganizationUnit> no BPOS interested attributes found on tenant. Skip tenant");
				result = true;
			}
			ExTraceGlobals.BackSyncTracer.TraceDebug<string>((long)SyncConfiguration.TraceId, "<OrganizationFilter.ShouldIgnoreOrganizationUnit> leaving ({0})", result.ToString());
			return result;
		}

		private static bool IsRelocationCompletedSignal(PropertyBag propertyBag)
		{
			return propertyBag.Contains(ADOrganizationalUnitSchema.MServSyncConfigFlags) && propertyBag[SyncCompanySchema.RelocationInProgress] != null && !(bool)propertyBag[SyncCompanySchema.RelocationInProgress];
		}

		private static bool IsExcludedOrg(ADRawEntry org)
		{
			return (bool)(org[OrganizationSchema.ExcludedFromBackSync] ?? false);
		}

		private readonly IPropertyLookup organizationPropertyLookup;

		private readonly ExcludedObjectReporter reporter;

		private readonly bool relocationEnabled;
	}
}
