using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ManagedStore.StoreIntegrityCheck;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.LogicalDataModel;
using Microsoft.Exchange.Server.Storage.PhysicalAccess;
using Microsoft.Exchange.Server.Storage.StoreCommonServices;

namespace Microsoft.Exchange.Server.Storage.StoreIntegrityCheck
{
	public sealed class MessagePropValueGetter
	{
		public MessagePropValueGetter(Context context, int mailboxPartitionNumber, ExchangeId folderId)
		{
			this.context = context;
			this.mailboxPartitionNumber = mailboxPartitionNumber;
			this.folderId = folderId;
		}

		public ErrorCode Execute(Column[] columnsToFetch, Func<Reader, ErrorCode> accessor, Func<bool> shouldContinue)
		{
			return this.Execute(true, columnsToFetch, accessor, shouldContinue);
		}

		public ErrorCode Execute(bool opportunedPreread, Column[] columnsToFetch, Func<Reader, ErrorCode> accessor, Func<bool> shouldContinue)
		{
			ErrorCode errorCode = this.Execute(false, opportunedPreread, null, columnsToFetch, accessor, shouldContinue);
			if (errorCode == ErrorCode.NoError)
			{
				errorCode = this.Execute(true, opportunedPreread, null, columnsToFetch, accessor, shouldContinue);
			}
			return errorCode;
		}

		public ErrorCode Execute(bool associated, byte[] mid, Column[] columnsToFetch, Func<Reader, ErrorCode> accessor, Func<bool> shouldContinue)
		{
			return this.Execute(associated, true, mid, columnsToFetch, accessor, shouldContinue);
		}

		public ErrorCode Execute(bool associated, bool opportunedPreread, byte[] mid, Column[] columnsToFetch, Func<Reader, ErrorCode> accessor, Func<bool> shouldContinue)
		{
			ErrorCode errorCode = ErrorCode.NoError;
			MessageTable messageTable = Microsoft.Exchange.Server.Storage.LogicalDataModel.DatabaseSchema.MessageTable(this.context.Database);
			KeyRange keyRange;
			if (mid == null)
			{
				StartStopKey startStopKey = new StartStopKey(true, new object[]
				{
					this.mailboxPartitionNumber,
					this.folderId.To26ByteArray(),
					associated
				});
				keyRange = new KeyRange(startStopKey, startStopKey);
			}
			else
			{
				StartStopKey startStopKey2 = new StartStopKey(true, new object[]
				{
					this.mailboxPartitionNumber,
					this.folderId.To26ByteArray(),
					associated,
					mid
				});
				keyRange = new KeyRange(startStopKey2, startStopKey2);
			}
			using (TableOperator tableOperator = Factory.CreateTableOperator(this.context.Culture, this.context, messageTable.Table, messageTable.MessageUnique, columnsToFetch, null, null, null, 0, 0, new KeyRange[]
			{
				keyRange
			}, false, opportunedPreread, true))
			{
				using (Reader reader = tableOperator.ExecuteReader(false))
				{
					while (reader.Read())
					{
						errorCode = accessor(reader);
						if (errorCode != ErrorCode.NoError)
						{
							if (ExTraceGlobals.OnlineIsintegTracer.IsTraceEnabled(TraceType.ErrorTrace))
							{
								ExTraceGlobals.OnlineIsintegTracer.TraceError<ErrorCode>(0L, "MessagePropValueGetter.Execute failed with error {0}", errorCode);
							}
							return errorCode.Propagate((LID)59880U);
						}
						if (!shouldContinue())
						{
							if (ExTraceGlobals.OnlineIsintegTracer.IsTraceEnabled(TraceType.ErrorTrace))
							{
								ExTraceGlobals.OnlineIsintegTracer.TraceError(0L, "Task aborted");
							}
							return ErrorCode.CreateExiting((LID)43496U);
						}
					}
				}
			}
			return ErrorCode.NoError;
		}

		private readonly Context context;

		private readonly int mailboxPartitionNumber;

		private readonly ExchangeId folderId;
	}
}
