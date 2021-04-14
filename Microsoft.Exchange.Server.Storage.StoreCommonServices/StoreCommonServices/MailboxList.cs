using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.PhysicalAccess;

namespace Microsoft.Exchange.Server.Storage.StoreCommonServices
{
	public class MailboxList : DisposableBase
	{
		public MailboxList(Context context, Column[] columns, StoreDatabase database, MailboxList.ListType listType)
		{
			IMailboxListRestriction mailboxListRestrictionFromListType = MailboxList.GetMailboxListRestrictionFromListType(listType);
			this.tableOperator = MailboxList.GetTableOperator(context, columns, database, mailboxListRestrictionFromListType);
		}

		public MailboxList(Context context, Column[] columns, StoreDatabase database, IMailboxListRestriction mailboxListRestriction)
		{
			this.tableOperator = MailboxList.GetTableOperator(context, columns, database, mailboxListRestriction);
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<MailboxList>(this);
		}

		protected override void InternalDispose(bool calledFromDispose)
		{
			if (calledFromDispose)
			{
				if (this.reader != null)
				{
					this.reader.Dispose();
					this.reader = null;
				}
				if (this.tableOperator != null)
				{
					this.tableOperator.Dispose();
					this.tableOperator = null;
				}
			}
		}

		public Reader OpenList()
		{
			if (base.IsDisposed)
			{
				throw new ObjectDisposedException("MailboxList");
			}
			this.reader = this.tableOperator.ExecuteReader(false);
			return this.reader;
		}

		private static TableOperator GetTableOperator(Context context, Column[] columns, StoreDatabase database, IMailboxListRestriction mailboxListRestriction)
		{
			MailboxTable mailboxTable = DatabaseSchema.MailboxTable(database);
			return Factory.CreateTableOperator(context.Culture, context, mailboxTable.Table, mailboxListRestriction.Index(mailboxTable), columns, mailboxListRestriction.Filter(context), null, 0, 0, KeyRange.AllRows, false, true);
		}

		private static IMailboxListRestriction GetMailboxListRestrictionFromListType(MailboxList.ListType listType)
		{
			IMailboxListRestriction result = null;
			switch (listType)
			{
			case MailboxList.ListType.Active:
				result = new MailboxListRestrictionActive();
				break;
			case MailboxList.ListType.ActiveAndDisabled:
				result = new MailboxListRestrictionActiveAndDisabled();
				break;
			case MailboxList.ListType.ActiveAndDisconnected:
				result = new MailboxListRestrictionActiveAndDisconnected();
				break;
			case MailboxList.ListType.SoftDeleted:
				result = new MailboxListRestrictionSoftDeleted();
				break;
			case MailboxList.ListType.FinalCleanup:
				result = new MailboxListRestrictionFinalCleanup();
				break;
			default:
				Microsoft.Exchange.Server.Storage.Common.Globals.AssertRetail(false, "Unknown ListType value");
				break;
			}
			return result;
		}

		private Reader reader;

		private TableOperator tableOperator;

		public enum ListType
		{
			Active,
			ActiveAndDisabled,
			ActiveAndDisconnected,
			SoftDeleted,
			FinalCleanup
		}
	}
}
