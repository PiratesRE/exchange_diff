using System;
using System.Data;

namespace Microsoft.Exchange.Hygiene.Data.MessageTrace
{
	internal sealed class TvpInfo
	{
		public TvpInfo(HygienePropertyDefinition tableName, DataTable tvp, HygienePropertyDefinition[] columns)
		{
			this.tableName = tableName;
			this.tvp = tvp;
			this.columns = columns;
		}

		public HygienePropertyDefinition TableName
		{
			get
			{
				return this.tableName;
			}
		}

		public DataTable Tvp
		{
			get
			{
				return this.tvp;
			}
		}

		public HygienePropertyDefinition[] Columns
		{
			get
			{
				return this.columns;
			}
		}

		private readonly HygienePropertyDefinition tableName;

		private readonly DataTable tvp;

		private readonly HygienePropertyDefinition[] columns;
	}
}
