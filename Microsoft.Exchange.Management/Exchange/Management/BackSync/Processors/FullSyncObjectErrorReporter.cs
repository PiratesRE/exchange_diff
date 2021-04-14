using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Sync;
using Microsoft.Exchange.Management.BackSync.Configuration;

namespace Microsoft.Exchange.Management.BackSync.Processors
{
	internal class FullSyncObjectErrorReporter : ExcludedObjectReporter
	{
		public FullSyncObjectErrorReporter(PerformanceCounterSession performanceCounterSession)
		{
			this.performanceCounterSession = performanceCounterSession;
		}

		public override DirectoryObjectError[] GetDirectoryObjectErrors()
		{
			return this.errors.ToArray();
		}

		public override void ReportExcludedObject(SyncObjectId objectId, DirectoryObjectErrorCode errorCode, ProcessingStage processingStage)
		{
			base.ReportExcludedObject(objectId, errorCode, processingStage);
			DirectoryObjectError directoryObjectError = new DirectoryObjectError();
			directoryObjectError.ErrorCode = errorCode;
			directoryObjectError.ObjectId = objectId.ObjectId;
			directoryObjectError.ContextId = objectId.ContextId;
			directoryObjectError.ObjectClass = objectId.ObjectClass;
			directoryObjectError.ObjectClassSpecified = true;
			this.errors.Add(directoryObjectError);
			if (this.performanceCounterSession != null)
			{
				this.performanceCounterSession.IncrementUserError();
			}
		}

		public override void ReportExcludedObject(PropertyBag propertyBag, DirectoryObjectErrorCode errorCode, ProcessingStage processingStage)
		{
			SyncObjectId syncObjectId = FullSyncObjectErrorReporter.GetSyncObjectId(propertyBag);
			if (syncObjectId != null)
			{
				this.ReportExcludedObject(syncObjectId, errorCode, processingStage);
				return;
			}
			base.ReportExcludedObject(propertyBag, errorCode, processingStage);
			DirectoryObjectError directoryObjectError = new DirectoryObjectError();
			directoryObjectError.ErrorCode = errorCode;
			directoryObjectError.ObjectId = (string)propertyBag[SyncObjectSchema.ObjectId];
			if (propertyBag.Contains(ADObjectSchema.ObjectClass))
			{
				directoryObjectError.ObjectClass = SyncRecipient.GetRecipientType(propertyBag);
			}
			if (propertyBag.Contains(SyncObjectSchema.ContextId))
			{
				directoryObjectError.ContextId = (string)propertyBag[SyncObjectSchema.ContextId];
			}
			this.errors.Add(directoryObjectError);
			if (this.performanceCounterSession != null)
			{
				this.performanceCounterSession.IncrementUserError();
			}
		}

		private static SyncObjectId GetSyncObjectId(PropertyBag propertyBag)
		{
			return (SyncObjectId)propertyBag[SyncObjectSchema.SyncObjectId];
		}

		private readonly List<DirectoryObjectError> errors = new List<DirectoryObjectError>();

		private PerformanceCounterSession performanceCounterSession;
	}
}
