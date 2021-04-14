using System;
using System.Diagnostics;
using System.Text;
using System.Threading;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class DelegateSessionEntry
	{
		internal DelegateSessionEntry(MailboxSession mailboxSession, OpenBy openBy) : this(mailboxSession.MailboxOwnerLegacyDN, mailboxSession, openBy)
		{
		}

		private DelegateSessionEntry(string mailboxLegacyDn, MailboxSession mailboxSession, OpenBy openBy)
		{
			this.mailboxLegacyDn = mailboxLegacyDn;
			this.mailboxSession = mailboxSession;
			this.stackTrace = new StackTrace();
			this.Access(openBy);
		}

		internal MailboxSession MailboxSession
		{
			get
			{
				return this.mailboxSession;
			}
		}

		internal void ForceDispose()
		{
			if (this.MailboxSession.IsDisposed)
			{
				return;
			}
			if (!this.MailboxSession.IsDead && this.MailboxSession.IsConnected)
			{
				this.MailboxSession.Disconnect();
			}
			this.MailboxSession.CanDispose = true;
			this.MailboxSession.Dispose();
		}

		internal int ExternalRefCount
		{
			get
			{
				return this.externalRefCount;
			}
		}

		internal int DecrementExternalRefCount()
		{
			int result = this.externalRefCount;
			this.externalRefCount--;
			if (this.externalRefCount == 0 && this.IsConnected)
			{
				this.Disconnect();
			}
			return result;
		}

		internal string GetCallStack()
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (StackFrame stackFrame in this.stackTrace.GetFrames())
			{
				stringBuilder.AppendLine(stackFrame.GetMethod().ToString());
			}
			return stringBuilder.ToString();
		}

		internal bool IsConnected
		{
			get
			{
				return this.MailboxSession.IsConnected;
			}
		}

		internal void Disconnect()
		{
			this.MailboxSession.Disconnect();
		}

		internal void Connect()
		{
			this.MailboxSession.Connect();
		}

		internal void Access(OpenBy openBy)
		{
			if (openBy == OpenBy.Consumer)
			{
				this.externalRefCount++;
			}
			this.lastAccessed = DelegateSessionEntry.GetNextWaterMark();
		}

		internal int LastAccessed
		{
			get
			{
				return this.lastAccessed;
			}
		}

		internal string LegacyDn
		{
			get
			{
				return this.mailboxLegacyDn;
			}
		}

		private static int GetNextWaterMark()
		{
			return Interlocked.Increment(ref DelegateSessionEntry.waterMark);
		}

		private int externalRefCount;

		private readonly string mailboxLegacyDn;

		private readonly MailboxSession mailboxSession;

		private int lastAccessed;

		private static int waterMark;

		private StackTrace stackTrace;
	}
}
