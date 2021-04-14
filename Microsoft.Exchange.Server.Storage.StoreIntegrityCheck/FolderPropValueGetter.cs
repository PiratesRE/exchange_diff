using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ManagedStore.StoreIntegrityCheck;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.LogicalDataModel;
using Microsoft.Exchange.Server.Storage.PhysicalAccess;
using Microsoft.Exchange.Server.Storage.StoreCommonServices;

namespace Microsoft.Exchange.Server.Storage.StoreIntegrityCheck
{
	public sealed class FolderPropValueGetter
	{
		public FolderPropValueGetter(Context context, int mailboxPartitionNumber)
		{
			this.context = context;
			this.mailboxPartitionNumber = mailboxPartitionNumber;
		}

		public ErrorCode Execute(Column[] columnsToFetch, Func<Reader, ErrorCode> accessor, Func<bool> shouldContinue)
		{
			return this.Execute(null, columnsToFetch, accessor, shouldContinue);
		}

		public ErrorCode Execute(byte[] folderId, Column[] columnsToFetch, Func<Reader, ErrorCode> accessor, Func<bool> shouldContinue)
		{
			FolderTable folderTable = Microsoft.Exchange.Server.Storage.LogicalDataModel.DatabaseSchema.FolderTable(this.context.Database);
			KeyRange keyRange;
			if (folderId == null)
			{
				StartStopKey startStopKey = new StartStopKey(true, new object[]
				{
					this.mailboxPartitionNumber
				});
				keyRange = new KeyRange(startStopKey, startStopKey);
			}
			else
			{
				StartStopKey startStopKey2 = new StartStopKey(true, new object[]
				{
					this.mailboxPartitionNumber,
					folderId
				});
				keyRange = new KeyRange(startStopKey2, startStopKey2);
			}
			using (TableOperator tableOperator = Factory.CreateTableOperator(this.context.Culture, this.context, folderTable.Table, folderTable.Table.PrimaryKeyIndex, columnsToFetch, null, null, 0, 0, keyRange, false, true))
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
								ExTraceGlobals.OnlineIsintegTracer.TraceError<ErrorCode>(0L, "FolderPropValueGetter.Execute failed with error {0}", errorCode);
							}
							return errorCode.Propagate((LID)51688U);
						}
						if (!shouldContinue())
						{
							if (ExTraceGlobals.OnlineIsintegTracer.IsTraceEnabled(TraceType.ErrorTrace))
							{
								ExTraceGlobals.OnlineIsintegTracer.TraceError(0L, "Task aborted");
							}
							return ErrorCode.CreateExiting((LID)55784U);
						}
					}
				}
			}
			return ErrorCode.NoError;
		}

		private readonly Context context;

		private readonly int mailboxPartitionNumber;
	}
}
