using System;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Assistants
{
	internal sealed class InvokeArgs
	{
		private InvokeArgs(StoreSession storeSession, MailboxData mailboxData)
		{
			this.StoreSession = storeSession;
			this.mailboxData = mailboxData;
		}

		public Guid ActivityId { get; internal set; }

		public MailboxData MailboxData
		{
			get
			{
				return this.mailboxData;
			}
		}

		public string Parameters { get; private set; }

		public StoreSession StoreSession { get; private set; }

		public TimeSpan TimePerTask { get; private set; }

		public static InvokeArgs Create(StoreSession storeSession)
		{
			StoreMailboxData storeMailboxData = new StoreMailboxData(storeSession.MailboxGuid, storeSession.MdbGuid, storeSession.DisplayAddress, null);
			return InvokeArgs.Create(storeSession, storeMailboxData);
		}

		public static InvokeArgs Create(StoreSession storeSession, MailboxData mailboxData)
		{
			return new InvokeArgs(storeSession, mailboxData)
			{
				TimePerTask = TimeSpan.Zero
			};
		}

		public static InvokeArgs Create(StoreSession storeSession, TimeSpan timePerTask, MailboxData mailboxData)
		{
			InvokeArgs invokeArgs = InvokeArgs.Create(storeSession, mailboxData);
			invokeArgs.TimePerTask = timePerTask;
			MailboxDataForDemandJob mailboxDataForDemandJob = mailboxData as MailboxDataForDemandJob;
			if (mailboxDataForDemandJob != null)
			{
				invokeArgs.Parameters = mailboxDataForDemandJob.Parameters;
			}
			return invokeArgs;
		}

		private MailboxData mailboxData;
	}
}
