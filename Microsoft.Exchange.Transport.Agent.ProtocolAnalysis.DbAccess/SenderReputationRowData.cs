using System;
using Microsoft.Exchange.Transport.Storage;

namespace Microsoft.Exchange.Transport.Agent.ProtocolAnalysis.DbAccess
{
	internal class SenderReputationRowData : DataRowAccessBase<SenderReputationTable, SenderReputationRowData>
	{
		[PrimaryKey]
		public byte[] SenderAddrHash
		{
			get
			{
				return ((ColumnCache<byte[]>)base.Columns[0]).Value;
			}
			private set
			{
				((ColumnCache<byte[]>)base.Columns[0]).Value = value;
			}
		}

		public int Srl
		{
			get
			{
				return ((ColumnCache<int>)base.Columns[1]).Value;
			}
			set
			{
				((ColumnCache<int>)base.Columns[1]).Value = value;
			}
		}

		public bool OpenProxy
		{
			get
			{
				return ((ColumnCache<bool>)base.Columns[2]).Value;
			}
			set
			{
				((ColumnCache<bool>)base.Columns[2]).Value = value;
			}
		}

		public DateTime ExpirationTime
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
	}
}
