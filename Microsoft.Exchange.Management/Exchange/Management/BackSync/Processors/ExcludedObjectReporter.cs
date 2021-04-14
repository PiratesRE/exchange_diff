using System;
using System.Threading;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Sync;
using Microsoft.Exchange.Diagnostics.Components.BackSync;

namespace Microsoft.Exchange.Management.BackSync.Processors
{
	internal class ExcludedObjectReporter : IExcludedObjectReporter
	{
		public virtual void ReportExcludedObject(PropertyBag propertyBag, DirectoryObjectErrorCode errorCode, ProcessingStage processingStage)
		{
			ExcludedObjectReporter.LogExcludedObject(ExcludedObjectReporter.GetId(propertyBag), errorCode, processingStage);
		}

		public virtual void ReportExcludedObject(SyncObjectId objectId, DirectoryObjectErrorCode errorCode, ProcessingStage processingStage)
		{
			ExcludedObjectReporter.LogExcludedObject(objectId.ToString(), errorCode, processingStage);
		}

		public virtual DirectoryObjectError[] GetDirectoryObjectErrors()
		{
			return null;
		}

		private static string GetId(PropertyBag propertyBag)
		{
			if (propertyBag.Contains(ADObjectSchema.Id))
			{
				return ((ADObjectId)propertyBag[ADObjectSchema.Id]).DistinguishedName;
			}
			if (propertyBag.Contains(ADObjectSchema.DistinguishedName))
			{
				return (string)propertyBag[ADObjectSchema.DistinguishedName];
			}
			if (propertyBag.Contains(SyncObjectSchema.ObjectId))
			{
				return (string)propertyBag[SyncObjectSchema.ObjectId];
			}
			return string.Empty;
		}

		private static void LogExcludedObject(string objectId, DirectoryObjectErrorCode errorCode, ProcessingStage processingStage)
		{
			ExTraceGlobals.BackSyncTracer.TraceDebug<string, DirectoryObjectErrorCode, ProcessingStage>((long)Thread.CurrentThread.ManagedThreadId, "Object '{0}' excluded from backsyc stream. Reason: {1}. Stage: {2}", objectId, errorCode, processingStage);
		}
	}
}
