using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Sync;
using Microsoft.Exchange.Diagnostics.Components.BackSync;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.BackSync.Processors
{
	internal class PagedOutputResultWriter : IDataProcessor
	{
		public PagedOutputResultWriter(WriteResultDelegate writeDelegate, Func<IEnumerable<SyncObject>, bool, byte[], ServiceInstanceId, object> createResponseDelegate, Action<int> serializeCountDelegate, AddErrorSyncObjectDelegate addErrorObjectDelegate, ServiceInstanceId currentServiceInstanceId)
		{
			this.writeDelegate = writeDelegate;
			this.createResponseDelegate = createResponseDelegate;
			this.serializeCountDelegate = serializeCountDelegate;
			this.entries = new List<PropertyBag>();
			this.addErrorSyncObjectDelegate = addErrorObjectDelegate;
			this.currentServiceInstanceId = currentServiceInstanceId;
		}

		public void Process(PropertyBag propertyBag)
		{
			this.CollectResult(propertyBag);
		}

		public void Flush(Func<byte[]> getCookieDelegate, bool moreData)
		{
			this.writeDelegate(this.CreateOutputObject(getCookieDelegate(), moreData));
		}

		protected void CollectResult(PropertyBag propertyBag)
		{
			this.entries.Add(propertyBag);
		}

		private IConfigurable CreateOutputObject(byte[] cookie, bool moreData)
		{
			ExTraceGlobals.BackSyncTracer.TraceDebug((long)SyncConfiguration.TraceId, "PagedOutputResultWriter.CreateOutputObject entering");
			ExTraceGlobals.BackSyncTracer.TraceDebug((long)SyncConfiguration.TraceId, "Create SyncObject list from ADPropertyBag list ...");
			List<SyncObject> list = new List<SyncObject>();
			foreach (PropertyBag propertyBag in this.entries)
			{
				ProcessorHelper.TracePropertBag("<PagedOutputResultWriter::CreateOutputObject>", propertyBag);
				list.Add(SyncObject.Create((ADPropertyBag)propertyBag));
			}
			ExTraceGlobals.BackSyncTracer.TraceDebug<int>((long)SyncConfiguration.TraceId, "SyncObject count = {0}", list.Count);
			if (this.serializeCountDelegate != null)
			{
				this.serializeCountDelegate(this.entries.Count);
			}
			ExTraceGlobals.BackSyncTracer.TraceDebug((long)SyncConfiguration.TraceId, "Create SyncData from SyncObject list ...");
			SyncData result = null;
			try
			{
				result = new SyncData(cookie, this.createResponseDelegate(list, moreData, cookie, this.currentServiceInstanceId));
				ExTraceGlobals.BackSyncTracer.TraceDebug((long)SyncConfiguration.TraceId, "SyncData created successfully");
			}
			catch (Exception ex)
			{
				ExTraceGlobals.BackSyncTracer.TraceError<string>((long)SyncConfiguration.TraceId, "Encountered exception during SyncData creation {0}", ex.ToString());
				if (this.addErrorSyncObjectDelegate == null)
				{
					throw ex;
				}
				if (this.FindErrorSyncObjects(cookie, moreData, list) == 0)
				{
					ExTraceGlobals.BackSyncTracer.TraceError<string>((long)SyncConfiguration.TraceId, "Didn't find SyncObject that caused the exception. Re-throw exception {0}.", ex.ToString());
					throw ex;
				}
				ExTraceGlobals.BackSyncTracer.TraceError((long)SyncConfiguration.TraceId, "PagedOutputResultWriter.CreateOutputObject Throw DataSourceTransientException");
				throw new DataSourceTransientException(Strings.BackSyncFailedToConvertSyncObjectToDirectoryObject, ex);
			}
			return result;
		}

		private int FindErrorSyncObjects(byte[] cookie, bool moreData, List<SyncObject> syncObjects)
		{
			ExTraceGlobals.BackSyncTracer.TraceDebug((long)SyncConfiguration.TraceId, "PagedOutputResultWriter.FindErrorSyncObjects entering");
			int num = 0;
			foreach (SyncObject syncObject in syncObjects)
			{
				string objectId = syncObject.ObjectId;
				ExTraceGlobals.BackSyncTracer.TraceDebug<string>((long)SyncConfiguration.TraceId, "PagedOutputResultWriter.FindErrorSyncObjects check {0}", objectId);
				try
				{
					List<SyncObject> list = new List<SyncObject>();
					list.Add(syncObject);
					new SyncData(cookie, this.createResponseDelegate(list, moreData, cookie, this.currentServiceInstanceId));
					ExTraceGlobals.BackSyncTracer.TraceDebug<string>((long)SyncConfiguration.TraceId, "PagedOutputResultWriter.FindErrorSyncObjects passed {0}", objectId);
				}
				catch (Exception ex)
				{
					ExTraceGlobals.BackSyncTracer.TraceError<string, string>((long)SyncConfiguration.TraceId, "PagedOutputResultWriter.FindErrorSyncObjects failed {0} exception {1}", objectId, ex.ToString());
					this.addErrorSyncObjectDelegate(syncObject, ex);
					num++;
				}
			}
			ExTraceGlobals.BackSyncTracer.TraceDebug<int>((long)SyncConfiguration.TraceId, "PagedOutputResultWriter.FindErrorSyncObjects this.syncConfiguration.ErrorSyncObjects.count = {0}", num);
			return num;
		}

		private readonly WriteResultDelegate writeDelegate;

		private readonly Func<IEnumerable<SyncObject>, bool, byte[], ServiceInstanceId, object> createResponseDelegate;

		private readonly Action<int> serializeCountDelegate;

		private readonly List<PropertyBag> entries;

		private readonly AddErrorSyncObjectDelegate addErrorSyncObjectDelegate;

		private readonly ServiceInstanceId currentServiceInstanceId;
	}
}
