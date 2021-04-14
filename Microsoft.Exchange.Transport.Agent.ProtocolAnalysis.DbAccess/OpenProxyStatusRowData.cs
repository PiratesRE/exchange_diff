using System;
using Microsoft.Exchange.Transport.Storage;

namespace Microsoft.Exchange.Transport.Agent.ProtocolAnalysis.DbAccess
{
	internal class OpenProxyStatusRowData : DataRowAccessBase<OpenProxyStatusTable, OpenProxyStatusRowData>
	{
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

		public DateTime LastAccessTime
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

		public DateTime LastDetectionTime
		{
			get
			{
				return ((ColumnCache<DateTime>)base.Columns[2]).Value;
			}
			set
			{
				((ColumnCache<DateTime>)base.Columns[2]).Value = value;
			}
		}

		public bool Processing
		{
			get
			{
				return ((ColumnCache<bool>)base.Columns[3]).Value;
			}
			set
			{
				((ColumnCache<bool>)base.Columns[3]).Value = value;
			}
		}

		public string Message
		{
			get
			{
				return ((ColumnCache<string>)base.Columns[4]).Value;
			}
			set
			{
				((ColumnCache<string>)base.Columns[4]).Value = value;
			}
		}

		public int OpenProxyStatus
		{
			get
			{
				return ((ColumnCache<int>)base.Columns[5]).Value;
			}
			set
			{
				((ColumnCache<int>)base.Columns[5]).Value = value;
			}
		}
	}
}
