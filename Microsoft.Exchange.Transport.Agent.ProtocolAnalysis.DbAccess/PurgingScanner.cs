using System;
using System.Globalization;
using Microsoft.Exchange.Transport.Storage;

namespace Microsoft.Exchange.Transport.Agent.ProtocolAnalysis.DbAccess
{
	internal class PurgingScanner<T, DTable> : ChunkingScanner
	{
		public PurgingScanner(DateTime cutoffTime)
		{
			this.cutoffTime = cutoffTime;
			this.typeInfo = typeof(T);
		}

		public PurgingScanner()
		{
			this.cutoffTime = new DateTime(0L);
			this.typeInfo = typeof(T);
			if (this.typeInfo != typeof(SenderReputationRowData))
			{
				throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "Illegal type: {0}. This constructor can only be used to purge SenderReputationTable", new object[]
				{
					this.typeInfo
				}));
			}
		}

		public void Scan()
		{
			if (Database.DataSource == null)
			{
				return;
			}
			using (DataConnection dataConnection = Database.DataSource.DemandNewConnection())
			{
				if (dataConnection != null)
				{
					DataTable tableByType = DbAccessServices.GetTableByType(typeof(DTable));
					using (DataTableCursor dataTableCursor = tableByType.OpenCursor(dataConnection))
					{
						if (dataTableCursor != null)
						{
							using (Transaction transaction = dataTableCursor.BeginTransaction())
							{
								if (transaction != null)
								{
									base.Scan(transaction, dataTableCursor, true);
									transaction.Commit();
								}
							}
						}
					}
				}
			}
		}

		protected override ChunkingScanner.ScanControl HandleRecord(DataTableCursor cursor)
		{
			DateTime t = new DateTime(0L);
			if (typeof(ProtocolAnalysisRowData) == this.typeInfo)
			{
				DataColumn<DateTime> dataColumn = (DataColumn<DateTime>)Database.ProtocolAnalysisTable.Schemas[1];
				t = dataColumn.ReadFromCursor(cursor);
			}
			else if (typeof(OpenProxyStatusTable) == this.typeInfo)
			{
				DataColumn<DateTime> dataColumn2 = (DataColumn<DateTime>)Database.OpenProxyStatusTable.Schemas[1];
				t = dataColumn2.ReadFromCursor(cursor);
			}
			if (this.typeInfo == typeof(SenderReputationTable) || t <= this.cutoffTime)
			{
				cursor.DeleteCurrentRow();
			}
			return ChunkingScanner.ScanControl.Continue;
		}

		private Type typeInfo;

		private DateTime cutoffTime;
	}
}
