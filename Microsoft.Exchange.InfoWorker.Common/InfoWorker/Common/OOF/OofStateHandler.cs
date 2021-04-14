using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Principal;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.InfoWorker.Common;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.InfoWorker.Common.OOF
{
	internal static class OofStateHandler
	{
		public static void Set(MailboxSession itemStore, bool oofState, DateTime userChangeTime)
		{
			if (itemStore == null)
			{
				throw new ArgumentNullException("itemStore");
			}
			itemStore.Mailbox[MailboxSchema.MailboxOofState] = oofState;
			itemStore.Mailbox[MailboxSchema.MailboxOofStateUserChangeTime] = new ExDateTime(itemStore.ExTimeZone, userChangeTime);
			OofStateHandler.Tracer.TraceDebug<IExchangePrincipal, bool>(0L, "Mailbox:{0}: OOF state set to: {1}", itemStore.MailboxOwner, oofState);
		}

		public static bool Get(MailboxSession itemStore)
		{
			if (itemStore == null)
			{
				throw new ArgumentNullException("itemStore");
			}
			bool flag = false;
			object obj = itemStore.Mailbox[MailboxSchema.MailboxOofState];
			if (obj is bool)
			{
				flag = (bool)obj;
			}
			OofStateHandler.Tracer.TraceDebug<IExchangePrincipal, bool>(0L, "Mailbox:{0}: get OOF state: {1}", itemStore.MailboxOwner, flag);
			return flag;
		}

		public static DateTime? GetUserChangeTime(MailboxSession itemStore)
		{
			if (itemStore == null)
			{
				throw new ArgumentNullException("itemStore");
			}
			DateTime? result = null;
			object obj = itemStore.Mailbox.TryGetProperty(MailboxSchema.MailboxOofStateUserChangeTime);
			if (obj is ExDateTime)
			{
				return new DateTime?((DateTime)((ExDateTime)obj).ToUtc());
			}
			OofStateHandler.Tracer.TraceDebug<IExchangePrincipal, object>(0L, "Mailbox:{0}: get OOF user change time: {1}", itemStore.MailboxOwner, obj);
			return result;
		}

		private static readonly Trace Tracer = ExTraceGlobals.OOFTracer;
	}
}
