using System;
using System.Globalization;
using System.Security.Principal;
using Microsoft.Exchange.Common.IL;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Server.Storage.Common;

namespace Microsoft.Exchange.Server.Storage.StoreCommonServices
{
	public class MailboxTaskContext : Context
	{
		public MailboxTaskContext()
		{
			base.SkipDatabaseLogsFlush = true;
		}

		public int MailboxNumber
		{
			get
			{
				return this.mailboxNumber;
			}
		}

		public Mailbox Mailbox
		{
			get
			{
				return this.mailbox;
			}
		}

		public override IMailboxContext PrimaryMailboxContext
		{
			get
			{
				return this.Mailbox;
			}
		}

		public SecurityIdentifier UserSid
		{
			get
			{
				return this.userSid;
			}
		}

		public StoreDatabase MailboxDatabase
		{
			get
			{
				return this.mailboxDatabase;
			}
		}

		public TaskExecutionDiagnostics TaskDiagnostics
		{
			get
			{
				return (TaskExecutionDiagnostics)base.Diagnostics;
			}
		}

		internal static TMailboxTaskContext CreateTaskExecutionContext<TMailboxTaskContext>(TaskTypeId taskTypeId, StoreDatabase database, int mailboxNumber, ClientType clientType, Guid clientActivityId, string clientComponentName, string clientProtocolName, string clientActionString, Guid userIdentity, SecurityIdentifier userSid, CultureInfo culture) where TMailboxTaskContext : MailboxTaskContext, new()
		{
			TMailboxTaskContext result = Activator.CreateInstance<TMailboxTaskContext>();
			result.PostConstructionInitialize(new TaskExecutionDiagnostics(taskTypeId, clientActivityId, clientComponentName, clientProtocolName, clientActionString), database, mailboxNumber, clientType, userIdentity, userSid, culture);
			return result;
		}

		protected void PostConstructionInitialize(TaskExecutionDiagnostics executionDiagnostics, StoreDatabase database, int mailboxNumber, ClientType clientType, Guid userIdentity, SecurityIdentifier userSid, CultureInfo culture)
		{
			base.PostConstructionInitialize(executionDiagnostics, Globals.ProcessSecurityContext, clientType, culture);
			this.mailboxDatabase = database;
			this.mailboxNumber = mailboxNumber;
			this.userSid = userSid;
			base.UserIdentity = userIdentity;
			base.InitializeMailboxExclusiveOperation(this.mailboxNumber, ExecutionDiagnostics.OperationSource.MailboxTask, LockManager.InfiniteTimeout);
		}

		protected override void ConnectDatabase()
		{
			base.ConnectDatabase();
			base.Connect(this.mailboxDatabase);
			if (!this.mailboxDatabase.IsOnlineActive)
			{
				base.Disconnect();
				throw new StoreException((LID)35456U, ErrorCodeValue.MdbNotInitialized);
			}
		}

		protected override void DisconnectDatabase()
		{
			base.Disconnect();
			base.DisconnectDatabase();
		}

		protected override void ConnectMailboxes()
		{
			base.ConnectMailboxes();
			bool flag = this.mailbox == null;
			if (flag)
			{
				DiagnosticContext.TraceLocation((LID)51840U);
				if (!base.LockedMailboxState.IsAccessible)
				{
					throw new StoreException((LID)37404U, ErrorCodeValue.TaskRequestFailed);
				}
				this.mailbox = Mailbox.OpenMailbox(this, base.LockedMailboxState);
				if (this.mailbox == null)
				{
					throw new StoreException((LID)53788U, ErrorCodeValue.TaskRequestFailed);
				}
			}
			else
			{
				DiagnosticContext.TraceLocation((LID)62080U);
				if (this.mailbox.IsDead)
				{
					throw new StoreException((LID)61980U, ErrorCodeValue.TaskRequestFailed);
				}
				this.mailbox.Reconnect(this);
			}
		}

		protected override void DisconnectMailboxes(bool pulseOnly)
		{
			base.DisconnectMailboxes(pulseOnly);
			if (this.mailbox != null && !pulseOnly)
			{
				this.mailbox.Dispose();
				this.mailbox = null;
			}
		}

		public override ErrorCode StartMailboxOperation(MailboxCreation mailboxCreation, bool findRemovedMailbox, bool skipQuarantineCheck)
		{
			return base.StartMailboxOperation(mailboxCreation, findRemovedMailbox, skipQuarantineCheck);
		}

		public override void EndMailboxOperation(bool commit, bool skipDisconnectingDatabase, bool pulseOnly)
		{
			try
			{
				base.EndMailboxOperation(commit, skipDisconnectingDatabase, pulseOnly);
			}
			finally
			{
				((TaskExecutionDiagnostics)base.Diagnostics).OnTaskEnd();
			}
		}

		private void Cleanup()
		{
			base.SystemCriticalOperation(new TryDelegate(this, (UIntPtr)ldftn(<Cleanup>b__0)));
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<MailboxTaskContext>(this);
		}

		protected override void InternalDispose(bool calledFromDispose)
		{
			if (calledFromDispose && this.mailbox != null)
			{
				this.Cleanup();
			}
			base.InternalDispose(calledFromDispose);
		}

		private StoreDatabase mailboxDatabase;

		private int mailboxNumber;

		private SecurityIdentifier userSid;

		private Mailbox mailbox;
	}
}
