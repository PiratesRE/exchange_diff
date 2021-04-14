using System;
using System.Globalization;
using Microsoft.Exchange.Transport.Storage;

namespace Microsoft.Exchange.Transport.Agent.ProtocolAnalysis.DbAccess
{
	internal class RowBaseScanner<T> : ChunkingScanner
	{
		public RowBaseScanner(int count, NextMessage<ProtocolAnalysisRowData> nextMessageHandler)
		{
			this.typeInfo = typeof(T);
			this.paHandler = nextMessageHandler;
			this.count = count;
		}

		public RowBaseScanner(int count, NextMessage<OpenProxyStatusRowData> nextMessageHandler)
		{
			this.typeInfo = typeof(T);
			this.opHandler = nextMessageHandler;
			this.count = count;
		}

		public RowBaseScanner(int count, NextMessage<SenderReputationRowData> nextMessageHandler)
		{
			this.typeInfo = typeof(T);
			this.srHandler = nextMessageHandler;
			this.count = count;
		}

		public RowBaseScanner(int count, NextMessage<ConfigurationDataRowData> nextMessageHandler)
		{
			this.typeInfo = typeof(T);
			this.confHandler = nextMessageHandler;
			this.count = count;
		}

		public RowBaseScanner()
		{
			this.typeInfo = typeof(T);
			this.count = 0;
			this.action = "truncate";
		}

		public void Scan()
		{
			if (typeof(ProtocolAnalysisRowData) == this.typeInfo)
			{
				using (DataConnection dataConnection = Database.DataSource.DemandNewConnection())
				{
					using (DataTableCursor dataTableCursor = Database.ProtocolAnalysisTable.OpenCursor(dataConnection))
					{
						using (Transaction transaction = dataTableCursor.BeginTransaction())
						{
							base.Scan(transaction, dataTableCursor, true);
							transaction.Commit();
						}
					}
					return;
				}
			}
			if (typeof(OpenProxyStatusRowData) == this.typeInfo)
			{
				using (DataConnection dataConnection2 = Database.DataSource.DemandNewConnection())
				{
					using (DataTableCursor dataTableCursor2 = Database.OpenProxyStatusTable.OpenCursor(dataConnection2))
					{
						using (Transaction transaction2 = dataTableCursor2.BeginTransaction())
						{
							base.Scan(transaction2, dataTableCursor2, true);
							transaction2.Commit();
						}
					}
					return;
				}
			}
			if (typeof(SenderReputationRowData) == this.typeInfo)
			{
				using (DataConnection dataConnection3 = Database.DataSource.DemandNewConnection())
				{
					using (DataTableCursor dataTableCursor3 = Database.SenderReputationTable.OpenCursor(dataConnection3))
					{
						using (Transaction transaction3 = dataTableCursor3.BeginTransaction())
						{
							base.Scan(transaction3, dataTableCursor3, true);
							transaction3.Commit();
						}
					}
					return;
				}
			}
			if (typeof(ConfigurationDataRowData) == this.typeInfo)
			{
				using (DataConnection dataConnection4 = Database.DataSource.DemandNewConnection())
				{
					using (DataTableCursor dataTableCursor4 = Database.ConfigurationDataTable.OpenCursor(dataConnection4))
					{
						using (Transaction transaction4 = dataTableCursor4.BeginTransaction())
						{
							base.Scan(transaction4, dataTableCursor4, true);
							transaction4.Commit();
						}
					}
					return;
				}
			}
			throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "Illegal type: {0}", new object[]
			{
				this.typeInfo
			}));
		}

		protected override ChunkingScanner.ScanControl HandleRecord(DataTableCursor cursor)
		{
			if ("truncate" == this.action)
			{
				cursor.DeleteCurrentRow();
				return ChunkingScanner.ScanControl.Continue;
			}
			if (this.count > 0 && this.numScanned >= this.count)
			{
				return ChunkingScanner.ScanControl.Stop;
			}
			if (typeof(ProtocolAnalysisRowData) == this.typeInfo)
			{
				ProtocolAnalysisRowData data = DataRowAccessBase<ProtocolAnalysisTable, ProtocolAnalysisRowData>.LoadCurrentRow(cursor);
				this.paHandler(data);
			}
			else if (typeof(OpenProxyStatusRowData) == this.typeInfo)
			{
				OpenProxyStatusRowData data2 = DataRowAccessBase<OpenProxyStatusTable, OpenProxyStatusRowData>.LoadCurrentRow(cursor);
				this.opHandler(data2);
			}
			else if (typeof(SenderReputationRowData) == this.typeInfo)
			{
				SenderReputationRowData data3 = DataRowAccessBase<SenderReputationTable, SenderReputationRowData>.LoadCurrentRow(cursor);
				this.srHandler(data3);
			}
			else
			{
				if (!(typeof(ConfigurationDataRowData) == this.typeInfo))
				{
					throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "Illegal type: {0}", new object[]
					{
						this.typeInfo
					}));
				}
				ConfigurationDataRowData data4 = DataRowAccessBase<ConfigurationDataTable, ConfigurationDataRowData>.LoadCurrentRow(cursor);
				this.confHandler(data4);
			}
			this.numScanned++;
			return ChunkingScanner.ScanControl.Continue;
		}

		private Type typeInfo;

		private NextMessage<ProtocolAnalysisRowData> paHandler;

		private NextMessage<OpenProxyStatusRowData> opHandler;

		private NextMessage<SenderReputationRowData> srHandler;

		private NextMessage<ConfigurationDataRowData> confHandler;

		private int count;

		private int numScanned;

		private string action = "get";
	}
}
