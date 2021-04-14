using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Data
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class CsvWrongNumberOfColumnsException : CsvValidationException
	{
		public CsvWrongNumberOfColumnsException(int rowNumber, int expectedColumnCount, int actualColumnCount) : base(DataStrings.WrongNumberOfColumns(rowNumber, expectedColumnCount, actualColumnCount))
		{
			this.rowNumber = rowNumber;
			this.expectedColumnCount = expectedColumnCount;
			this.actualColumnCount = actualColumnCount;
		}

		public CsvWrongNumberOfColumnsException(int rowNumber, int expectedColumnCount, int actualColumnCount, Exception innerException) : base(DataStrings.WrongNumberOfColumns(rowNumber, expectedColumnCount, actualColumnCount), innerException)
		{
			this.rowNumber = rowNumber;
			this.expectedColumnCount = expectedColumnCount;
			this.actualColumnCount = actualColumnCount;
		}

		protected CsvWrongNumberOfColumnsException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.rowNumber = (int)info.GetValue("rowNumber", typeof(int));
			this.expectedColumnCount = (int)info.GetValue("expectedColumnCount", typeof(int));
			this.actualColumnCount = (int)info.GetValue("actualColumnCount", typeof(int));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("rowNumber", this.rowNumber);
			info.AddValue("expectedColumnCount", this.expectedColumnCount);
			info.AddValue("actualColumnCount", this.actualColumnCount);
		}

		public int RowNumber
		{
			get
			{
				return this.rowNumber;
			}
		}

		public int ExpectedColumnCount
		{
			get
			{
				return this.expectedColumnCount;
			}
		}

		public int ActualColumnCount
		{
			get
			{
				return this.actualColumnCount;
			}
		}

		private readonly int rowNumber;

		private readonly int expectedColumnCount;

		private readonly int actualColumnCount;
	}
}
