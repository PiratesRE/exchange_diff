using System;
using System.Threading;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.InfoWorker.Common;

namespace Microsoft.Exchange.InfoWorker.Common
{
	internal static class TraceContext
	{
		public static object Get()
		{
			object obj = Thread.GetData(TraceContext.slotId);
			if (obj == null)
			{
				obj = "<no context set>";
			}
			return obj;
		}

		public static void Set(MailboxSession mailboxSession)
		{
			if (mailboxSession == null)
			{
				throw new ArgumentNullException("mailboxSession");
			}
			TraceContext.SetInternal("Mailbox:" + mailboxSession.MailboxOwner.ToString());
		}

		public static void Set(object context)
		{
			if (context == null)
			{
				throw new ArgumentNullException("context");
			}
			TraceContext.SetInternal(context);
		}

		public static void Reset()
		{
			TraceContext.SetInternal(null);
		}

		private static void SetInternal(object context)
		{
			object data = Thread.GetData(TraceContext.slotId);
			TraceContext.Tracer.TraceDebug((long)TraceContext.slotId.GetHashCode(), "New context: {0}. Previous context: {1}.", new object[]
			{
				(context == null) ? "<null>" : context,
				(data == null) ? "<null>" : context
			});
			Thread.SetData(TraceContext.slotId, context);
		}

		private static LocalDataStoreSlot slotId = Thread.AllocateDataSlot();

		private static readonly Trace Tracer = ExTraceGlobals.TraceContextTracer;
	}
}
