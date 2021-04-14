using System;

namespace Microsoft.Exchange.Diagnostics.Components.ServiceHost.PhotoGarbageCollection
{
	public static class ExTraceGlobals
	{
		public static Trace GarbageCollectionTracer
		{
			get
			{
				if (ExTraceGlobals.garbageCollectionTracer == null)
				{
					ExTraceGlobals.garbageCollectionTracer = new Trace(ExTraceGlobals.componentGuid, 0);
				}
				return ExTraceGlobals.garbageCollectionTracer;
			}
		}

		private static Guid componentGuid = new Guid("D741A9E4-436A-4266-80F4-E6B7EC1E8611");

		private static Trace garbageCollectionTracer = null;
	}
}
