using System;
using System.Text;

namespace Microsoft.Exchange.Server.Storage.StoreCommonServices
{
	public abstract class NotificationEvent
	{
		protected NotificationEvent(StoreDatabase database, int mailboxNumber, int eventTypeValue, Guid? userIdentityContext)
		{
			this.database = database;
			this.mailboxNumber = mailboxNumber;
			this.eventTypeValue = eventTypeValue;
			this.userIdentityContext = userIdentityContext;
		}

		public StoreDatabase Database
		{
			get
			{
				return this.database;
			}
		}

		public Guid MdbGuid
		{
			get
			{
				return this.database.MdbGuid;
			}
		}

		public int MailboxNumber
		{
			get
			{
				return this.mailboxNumber;
			}
		}

		public int EventTypeValue
		{
			get
			{
				return this.eventTypeValue;
			}
		}

		public Guid? UserIdentityContext
		{
			get
			{
				return this.userIdentityContext;
			}
		}

		public virtual NotificationEvent.RedundancyStatus GetRedundancyStatus(NotificationEvent oldNev)
		{
			return NotificationEvent.RedundancyStatus.FlagStopSearch;
		}

		public virtual NotificationEvent MergeWithOldEvent(NotificationEvent oldNev)
		{
			return this;
		}

		protected abstract void AppendClassName(StringBuilder sb);

		protected virtual void AppendFields(StringBuilder sb)
		{
			sb.Append("MdbGuid:[");
			sb.Append(this.Database.MdbGuid);
			sb.Append("] MailboxNumber:[");
			sb.Append(this.MailboxNumber);
			sb.Append("] EventTypeValue:[");
			sb.Append(this.EventTypeValue);
			sb.Append("] UserIdentityContext:[");
			sb.Append((this.userIdentityContext != null) ? this.userIdentityContext.ToString() : "none");
			sb.Append("]");
		}

		public virtual void AppendToString(StringBuilder sb)
		{
			this.AppendClassName(sb);
			sb.Append(":[");
			this.AppendFields(sb);
			sb.Append("]");
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder(250);
			this.AppendToString(stringBuilder);
			return stringBuilder.ToString();
		}

		private StoreDatabase database;

		private int mailboxNumber;

		private int eventTypeValue;

		private Guid? userIdentityContext;

		[Flags]
		public enum RedundancyStatus
		{
			FlagDropNew = 1,
			FlagDropOld = 2,
			FlagReplaceOld = 4,
			FlagMerge = 8,
			FlagStopSearch = -2147483648,
			Continue = 0,
			Stop = -2147483648,
			DropNewAndStop = -2147483647,
			DropOld = 2,
			DropOldAndStop = -2147483646,
			DropBothAndStop = -2147483645,
			ReplaceOldAndStop = -2147483644,
			MergeReplaceOldAndStop = -2147483636,
			Merge = 10
		}
	}
}
