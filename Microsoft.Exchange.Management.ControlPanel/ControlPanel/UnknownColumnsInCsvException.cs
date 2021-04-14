using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class UnknownColumnsInCsvException : LocalizedException
	{
		public UnknownColumnsInCsvException(string columns) : base(Strings.UnknownColumnsInCsv(columns))
		{
			this.columns = columns;
		}

		public UnknownColumnsInCsvException(string columns, Exception innerException) : base(Strings.UnknownColumnsInCsv(columns), innerException)
		{
			this.columns = columns;
		}

		protected UnknownColumnsInCsvException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.columns = (string)info.GetValue("columns", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("columns", this.columns);
		}

		public string Columns
		{
			get
			{
				return this.columns;
			}
		}

		private readonly string columns;
	}
}
