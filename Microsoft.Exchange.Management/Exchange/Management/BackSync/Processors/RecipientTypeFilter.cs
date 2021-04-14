using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.Sync;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics.Components.BackSync;

namespace Microsoft.Exchange.Management.BackSync.Processors
{
	internal class RecipientTypeFilter : PipelineProcessor
	{
		public RecipientTypeFilter(IDataProcessor next, RecipientTypeDetails acceptedRecipientTypes, ExcludedObjectReporter reporter) : base(next)
		{
			this.acceptedRecipientTypes = acceptedRecipientTypes;
			this.reporter = reporter;
		}

		protected override bool ProcessInternal(PropertyBag propertyBag)
		{
			if (!RecipientTypeFilter.HasObjectId(propertyBag))
			{
				ExTraceGlobals.BackSyncTracer.TraceDebug((long)SyncConfiguration.TraceId, "RecipientTypeFilter:: - Skipping object {0}. No object id set on object.", new object[]
				{
					propertyBag[ADObjectSchema.Id]
				});
				this.reporter.ReportExcludedObject(propertyBag, DirectoryObjectErrorCode.ObjectOutOfScope, ProcessingStage.RecipientTypeFilter);
				return false;
			}
			if (ProcessorHelper.IsDeletedObject(propertyBag))
			{
				return !ProvisioningTasksConfigImpl.UseBecAPIsforLiveId || !ProcessorHelper.IsUserObject(propertyBag);
			}
			if (RecipientTypeFilter.IsObjectExcluded(propertyBag))
			{
				ExTraceGlobals.BackSyncTracer.TraceDebug((long)SyncConfiguration.TraceId, "RecipientTypeFilter:: - Skipping object {0}. It's marked as excluded form backsync.", new object[]
				{
					propertyBag[ADObjectSchema.Id]
				});
				this.reporter.ReportExcludedObject(propertyBag, DirectoryObjectErrorCode.UnspecifiedError, ProcessingStage.RecipientTypeFilter);
				return false;
			}
			bool flag;
			if (!ProcessorHelper.IsObjectOrganizationUnit(propertyBag))
			{
				RecipientTypeDetails recipientTypeDetails = (RecipientTypeDetails)propertyBag[ADRecipientSchema.RecipientTypeDetails];
				flag = this.IsAcceptedRecipientType(recipientTypeDetails);
				if (!flag)
				{
					ExTraceGlobals.BackSyncTracer.TraceDebug<object, RecipientTypeDetails>((long)SyncConfiguration.TraceId, "RecipientTypeFilter:: - Skipping object {0}. Recipient type {1} is not included in backsync.", propertyBag[SyncObjectSchema.ObjectId], recipientTypeDetails);
					this.reporter.ReportExcludedObject(propertyBag, DirectoryObjectErrorCode.ObjectOutOfScope, ProcessingStage.RecipientTypeFilter);
				}
			}
			else
			{
				flag = true;
			}
			return flag;
		}

		internal static bool HasObjectId(PropertyBag propertyBag)
		{
			return propertyBag.Contains(SyncObjectSchema.ObjectId) && !string.IsNullOrEmpty((string)propertyBag[SyncObjectSchema.ObjectId]) && RecipientTypeFilter.IsValidGuid((string)propertyBag[SyncObjectSchema.ObjectId]);
		}

		private static bool IsObjectExcluded(PropertyBag propertyBag)
		{
			return (bool)(propertyBag[ADRecipientSchema.ExcludedFromBackSync] ?? false);
		}

		private static bool IsValidGuid(string objectId)
		{
			Guid a;
			try
			{
				a = new Guid(objectId);
			}
			catch (Exception arg)
			{
				ExTraceGlobals.BackSyncTracer.TraceDebug<string, Exception>((long)SyncConfiguration.TraceId, "RecipientTypeFilter:: - Skipping object {0}. Failed to convert object id to a guid. Exception: {1}.", objectId, arg);
				return false;
			}
			return a != Guid.Empty;
		}

		private bool IsAcceptedRecipientType(RecipientTypeDetails recipientType)
		{
			return (this.acceptedRecipientTypes & recipientType) != RecipientTypeDetails.None;
		}

		private readonly RecipientTypeDetails acceptedRecipientTypes;

		private readonly ExcludedObjectReporter reporter;
	}
}
