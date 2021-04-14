using System;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Services.Wcf.Types
{
	internal abstract class TaskFolderActionBase<T> where T : TaskFolderActionResponse
	{
		private protected MailboxSession MailboxSession { protected get; private set; }

		public TaskFolderActionBase(MailboxSession session)
		{
			this.MailboxSession = session;
		}

		public abstract T Execute();
	}
}
