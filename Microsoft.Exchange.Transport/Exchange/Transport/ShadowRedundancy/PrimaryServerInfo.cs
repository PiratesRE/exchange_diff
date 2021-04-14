using System;
using System.Collections.Generic;
using Microsoft.Exchange.Transport.Storage;

namespace Microsoft.Exchange.Transport.ShadowRedundancy
{
	internal sealed class PrimaryServerInfo : DataRow
	{
		public PrimaryServerInfo(DataTable dataTable) : base(dataTable)
		{
			this.EndTime = DateTime.MaxValue;
		}

		public PrimaryServerInfo(PrimaryServerInfo primaryServerInfo, DataTable dataTable) : base(dataTable)
		{
			this.ServerFqdn = primaryServerInfo.ServerFqdn;
			this.DatabaseState = primaryServerInfo.DatabaseState;
			this.StartTime = primaryServerInfo.StartTime;
			this.EndTime = primaryServerInfo.EndTime;
			this.Version = primaryServerInfo.Version;
		}

		public string ServerFqdn
		{
			get
			{
				return ((ColumnCache<string>)base.Columns[2]).Value;
			}
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					throw new ArgumentNullException("serverFqdn");
				}
				((ColumnCache<string>)base.Columns[2]).Value = value;
			}
		}

		public string DatabaseState
		{
			get
			{
				return ((ColumnCache<string>)base.Columns[1]).Value;
			}
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					throw new ArgumentNullException("databaseState");
				}
				((ColumnCache<string>)base.Columns[1]).Value = value;
			}
		}

		public DateTime StartTime
		{
			get
			{
				return ((ColumnCache<DateTime>)base.Columns[3]).Value;
			}
			set
			{
				((ColumnCache<DateTime>)base.Columns[3]).Value = value;
			}
		}

		public DateTime EndTime
		{
			get
			{
				return ((ColumnCache<DateTime>)base.Columns[4]).Value;
			}
			set
			{
				((ColumnCache<DateTime>)base.Columns[4]).Value = value;
			}
		}

		public int Id
		{
			get
			{
				return ((ColumnCache<int>)base.Columns[0]).Value;
			}
		}

		public ShadowRedundancyCompatibilityVersion Version
		{
			get
			{
				return (ShadowRedundancyCompatibilityVersion)((ColumnCache<int>)base.Columns[5]).Value;
			}
			set
			{
				((ColumnCache<int>)base.Columns[5]).Value = (int)value;
			}
		}

		public bool IsActive
		{
			get
			{
				return this.EndTime.Year == DateTime.MaxValue.Year;
			}
		}

		public static PrimaryServerInfo Load(DataTableCursor cursor, DataTable dataTable)
		{
			PrimaryServerInfo primaryServerInfo = new PrimaryServerInfo(dataTable);
			primaryServerInfo.LoadFromCurrentRow(cursor);
			return primaryServerInfo;
		}

		public static void CommitLazy(IEnumerable<PrimaryServerInfo> serversToCommit, DataTable dataTable)
		{
			using (DataTableCursor cursor = dataTable.GetCursor())
			{
				using (Transaction transaction = cursor.BeginTransaction())
				{
					foreach (PrimaryServerInfo primaryServerInfo in serversToCommit)
					{
						primaryServerInfo.MaterializeToCursor(transaction, cursor);
					}
					transaction.Commit(TransactionCommitMode.Lazy);
				}
			}
		}

		public void Delete()
		{
			this.MarkToDelete();
		}

		public new void Commit(TransactionCommitMode transactionCommitMode)
		{
			base.Commit(transactionCommitMode);
		}
	}
}
