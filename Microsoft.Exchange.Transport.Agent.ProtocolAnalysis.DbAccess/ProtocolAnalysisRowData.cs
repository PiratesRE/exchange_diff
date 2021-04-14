using System;
using Microsoft.Exchange.Transport.Storage;

namespace Microsoft.Exchange.Transport.Agent.ProtocolAnalysis.DbAccess
{
	internal class ProtocolAnalysisRowData : DataRowAccessBase<ProtocolAnalysisTable, ProtocolAnalysisRowData>
	{
		public ProtocolAnalysisRowData()
		{
			this.Processing = false;
			this.ReverseDns = string.Empty;
			this.DataBlob = new byte[0];
			this.LastQueryTime = DateTime.UtcNow;
		}

		[PrimaryKey]
		public string SenderAddress
		{
			get
			{
				return ((ColumnCache<string>)base.Columns[0]).Value;
			}
			private set
			{
				((ColumnCache<string>)base.Columns[0]).Value = value;
			}
		}

		public DateTime LastUpdateTime
		{
			get
			{
				return ((ColumnCache<DateTime>)base.Columns[1]).Value;
			}
			set
			{
				((ColumnCache<DateTime>)base.Columns[1]).Value = value;
			}
		}

		public byte[] DataBlob
		{
			get
			{
				return ((ColumnCache<byte[]>)base.Columns[2]).Value;
			}
			set
			{
				((ColumnCache<byte[]>)base.Columns[2]).Value = value;
			}
		}

		public string ReverseDns
		{
			get
			{
				return ((ColumnCache<string>)base.Columns[3]).Value;
			}
			set
			{
				((ColumnCache<string>)base.Columns[3]).Value = value;
			}
		}

		public DateTime LastQueryTime
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

		public bool Processing
		{
			get
			{
				return ((ColumnCache<bool>)base.Columns[5]).Value;
			}
			set
			{
				((ColumnCache<bool>)base.Columns[5]).Value = value;
			}
		}
	}
}
