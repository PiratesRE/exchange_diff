using System;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Services.Wcf.Types
{
	internal abstract class CalendarActionBase<T> where T : CalendarActionResponse
	{
		private protected MailboxSession MailboxSession { protected get; private set; }

		public CalendarActionBase(MailboxSession session)
		{
			this.MailboxSession = session;
		}

		public abstract T Execute();
	}
}
