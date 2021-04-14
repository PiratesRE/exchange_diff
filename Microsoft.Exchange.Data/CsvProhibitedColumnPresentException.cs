using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Data
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class CsvProhibitedColumnPresentException : CsvValidationException
	{
		public CsvProhibitedColumnPresentException(string prohibitedColumn) : base(DataStrings.ProhibitedColumnPresent(prohibitedColumn))
		{
			this.prohibitedColumn = prohibitedColumn;
		}

		public CsvProhibitedColumnPresentException(string prohibitedColumn, Exception innerException) : base(DataStrings.ProhibitedColumnPresent(prohibitedColumn), innerException)
		{
			this.prohibitedColumn = prohibitedColumn;
		}

		protected CsvProhibitedColumnPresentException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.prohibitedColumn = (string)info.GetValue("prohibitedColumn", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("prohibitedColumn", this.prohibitedColumn);
		}

		public string ProhibitedColumn
		{
			get
			{
				return this.prohibitedColumn;
			}
		}

		private readonly string prohibitedColumn;
	}
}
