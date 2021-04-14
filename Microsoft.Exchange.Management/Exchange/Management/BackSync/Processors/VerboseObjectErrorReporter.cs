using System;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Sync;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.BackSync.Processors
{
	internal class VerboseObjectErrorReporter : ExcludedObjectReporter
	{
		public VerboseObjectErrorReporter(Task.TaskVerboseLoggingDelegate verboseLoggingDelegate)
		{
			this.verboseLoggingDelegate = verboseLoggingDelegate;
		}

		public override void ReportExcludedObject(SyncObjectId objectId, DirectoryObjectErrorCode errorCode, ProcessingStage processingStage)
		{
			base.ReportExcludedObject(objectId, errorCode, processingStage);
			this.verboseLoggingDelegate(Strings.BackSyncObjectExcluded(objectId.ToString(), errorCode, processingStage));
		}

		public override void ReportExcludedObject(PropertyBag propertyBag, DirectoryObjectErrorCode errorCode, ProcessingStage processingStage)
		{
			base.ReportExcludedObject(propertyBag, errorCode, processingStage);
			DirectoryObjectClass directoryObjectClass = DirectoryObjectClass.Account;
			if (propertyBag.Contains(ADObjectSchema.ObjectClass))
			{
				directoryObjectClass = SyncRecipient.GetRecipientType(propertyBag);
			}
			ADObjectId adObjectId = (ADObjectId)propertyBag[ADObjectSchema.Id];
			string objectId = (string)propertyBag[SyncObjectSchema.ObjectId];
			this.verboseLoggingDelegate(Strings.BackSyncObjectExcludedExtended(directoryObjectClass, objectId, adObjectId, errorCode, processingStage));
		}

		private Task.TaskVerboseLoggingDelegate verboseLoggingDelegate;
	}
}
