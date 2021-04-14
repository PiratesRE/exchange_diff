using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Data
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class CsvUnknownColumnsException : CsvValidationException
	{
		public CsvUnknownColumnsException(string columns, IEnumerable<string> unknownColumns) : base(DataStrings.UnknownColumns(columns, unknownColumns))
		{
			this.columns = columns;
			this.unknownColumns = unknownColumns;
		}

		public CsvUnknownColumnsException(string columns, IEnumerable<string> unknownColumns, Exception innerException) : base(DataStrings.UnknownColumns(columns, unknownColumns), innerException)
		{
			this.columns = columns;
			this.unknownColumns = unknownColumns;
		}

		protected CsvUnknownColumnsException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.columns = (string)info.GetValue("columns", typeof(string));
			this.unknownColumns = (IEnumerable<string>)info.GetValue("unknownColumns", typeof(IEnumerable<string>));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("columns", this.columns);
			info.AddValue("unknownColumns", this.unknownColumns);
		}

		public string Columns
		{
			get
			{
				return this.columns;
			}
		}

		public IEnumerable<string> UnknownColumns
		{
			get
			{
				return this.unknownColumns;
			}
		}

		private readonly string columns;

		private readonly IEnumerable<string> unknownColumns;
	}
}
