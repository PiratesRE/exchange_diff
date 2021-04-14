using System;

namespace Microsoft.Exchange.Data.Directory.Sync
{
	internal interface IExcludedObjectReporter
	{
		void ReportExcludedObject(PropertyBag propertyBag, DirectoryObjectErrorCode errorCode, ProcessingStage processingStage);

		void ReportExcludedObject(SyncObjectId objectId, DirectoryObjectErrorCode errorCode, ProcessingStage processingStage);

		DirectoryObjectError[] GetDirectoryObjectErrors();
	}
}
