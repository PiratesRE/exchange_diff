using System;

namespace Microsoft.Exchange.Server.Storage.PhysicalAccess
{
	public class TableLevelIOStats
	{
		public TableLevelIOStats(string tableName)
		{
			this.Reset();
			this.tableName = tableName;
		}

		public string TableName
		{
			get
			{
				return this.tableName;
			}
		}

		public int LogicalReads
		{
			get
			{
				return this.logicalReads;
			}
			set
			{
				this.logicalReads = value;
			}
		}

		public int PhysicalReads
		{
			get
			{
				return this.physicalReads;
			}
			set
			{
				this.physicalReads = value;
			}
		}

		public int ReadAheads
		{
			get
			{
				return this.readAheads;
			}
			set
			{
				this.readAheads = value;
			}
		}

		public int LobLogicalReads
		{
			get
			{
				return this.lobLogicalReads;
			}
			set
			{
				this.lobLogicalReads = value;
			}
		}

		public int LobPhysicalReads
		{
			get
			{
				return this.lobPhysicalReads;
			}
			set
			{
				this.lobPhysicalReads = value;
			}
		}

		public int LobReadAheads
		{
			get
			{
				return this.lobReadAheads;
			}
			set
			{
				this.lobReadAheads = value;
			}
		}

		public void Reset()
		{
			this.logicalReads = 0;
			this.physicalReads = 0;
			this.readAheads = 0;
			this.lobLogicalReads = 0;
			this.lobPhysicalReads = 0;
			this.lobReadAheads = 0;
		}

		private readonly string tableName;

		private int logicalReads;

		private int physicalReads;

		private int readAheads;

		private int lobLogicalReads;

		private int lobPhysicalReads;

		private int lobReadAheads;
	}
}
