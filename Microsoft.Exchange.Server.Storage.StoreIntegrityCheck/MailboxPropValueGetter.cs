using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ManagedStore.StoreIntegrityCheck;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.PhysicalAccess;
using Microsoft.Exchange.Server.Storage.StoreCommonServices;

namespace Microsoft.Exchange.Server.Storage.StoreIntegrityCheck
{
	public class MailboxPropValueGetter
	{
		public MailboxPropValueGetter(Context context)
		{
			this.context = context;
		}

		public ErrorCode Execute(Column[] columnsToFetch, Func<Reader, ErrorCode> accessor, Func<bool> shouldContinue)
		{
			return this.Execute(Guid.Empty, columnsToFetch, accessor, shouldContinue);
		}

		public ErrorCode Execute(Guid mailboxGuid, Column[] columnsToFetch, Func<Reader, ErrorCode> accessor, Func<bool> shouldContinue)
		{
			MailboxTable mailboxTable = DatabaseSchema.MailboxTable(this.context.Database);
			KeyRange allRowsRange;
			if (!mailboxGuid.Equals(Guid.Empty))
			{
				StartStopKey startStopKey = new StartStopKey(true, new object[]
				{
					mailboxGuid
				});
				allRowsRange = new KeyRange(startStopKey, startStopKey);
			}
			else
			{
				allRowsRange = KeyRange.AllRowsRange;
			}
			using (TableOperator tableOperator = Factory.CreateTableOperator(this.context.Culture, this.context, mailboxTable.Table, mailboxTable.MailboxGuidIndex, columnsToFetch, Factory.CreateSearchCriteriaAnd(new SearchCriteria[]
			{
				Factory.CreateSearchCriteriaCompare(mailboxTable.DeletedOn, SearchCriteriaCompare.SearchRelOp.Equal, Factory.CreateConstantColumn(null, mailboxTable.DeletedOn)),
				Factory.CreateSearchCriteriaCompare(mailboxTable.MailboxGuid, SearchCriteriaCompare.SearchRelOp.NotEqual, Factory.CreateConstantColumn(null, mailboxTable.MailboxGuid))
			}), null, 0, 0, allRowsRange, false, true))
			{
				using (Reader reader = tableOperator.ExecuteReader(false))
				{
					while (reader.Read())
					{
						ErrorCode errorCode = accessor(reader);
						if (errorCode != ErrorCode.NoError)
						{
							if (ExTraceGlobals.OnlineIsintegTracer.IsTraceEnabled(TraceType.ErrorTrace))
							{
								ExTraceGlobals.OnlineIsintegTracer.TraceError<ErrorCode>(0L, "MailboxPropValueGetter.Execute failed with error {0}", errorCode);
							}
							return errorCode.Propagate((LID)35304U);
						}
						if (!shouldContinue())
						{
							if (ExTraceGlobals.OnlineIsintegTracer.IsTraceEnabled(TraceType.ErrorTrace))
							{
								ExTraceGlobals.OnlineIsintegTracer.TraceError(0L, "Task aborted");
							}
							return ErrorCode.CreateExiting((LID)39400U);
						}
					}
				}
			}
			return ErrorCode.NoError;
		}

		private readonly Context context;
	}
}
