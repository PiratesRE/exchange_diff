using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Sync;
using Microsoft.Exchange.Diagnostics.Components.BackSync;

namespace Microsoft.Exchange.Management.BackSync.Processors
{
	internal abstract class PipelineProcessor : IDataProcessor
	{
		protected PipelineProcessor(IDataProcessor next)
		{
			this.next = next;
		}

		public void Process(PropertyBag propertyBag)
		{
			Type type = base.GetType();
			string fullName = type.FullName;
			string text = string.Format("PipelineProcessor <{0}>", fullName);
			ADObjectId adobjectId = (ADObjectId)propertyBag[ADObjectSchema.Id];
			ExTraceGlobals.BackSyncTracer.TraceDebug<string, string>((long)SyncConfiguration.TraceId, "{0} process {1} ...", text, adobjectId.ToString());
			ProcessorHelper.TracePropertBag(text, propertyBag);
			if (this.ProcessInternal(propertyBag))
			{
				this.next.Process(propertyBag);
			}
			else
			{
				ExTraceGlobals.BackSyncTracer.TraceDebug<string>((long)SyncConfiguration.TraceId, "{0} processing terminated", text);
			}
			ExTraceGlobals.BackSyncTracer.TraceDebug<string, string>((long)SyncConfiguration.TraceId, "{0} completed process {1}", text, adobjectId.ToString());
		}

		public void Flush(Func<byte[]> getCookieDelegate, bool moreData)
		{
			this.ProcessRemainingEntries();
			this.next.Flush(getCookieDelegate, moreData);
		}

		protected virtual void ProcessRemainingEntries()
		{
		}

		protected abstract bool ProcessInternal(PropertyBag propertyBag);

		private readonly IDataProcessor next;
	}
}
