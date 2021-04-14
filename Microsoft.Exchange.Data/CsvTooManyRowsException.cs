using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Data
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class CsvTooManyRowsException : CsvValidationException
	{
		public CsvTooManyRowsException(int maximumRowCount) : base(DataStrings.TooManyRows(maximumRowCount))
		{
			this.maximumRowCount = maximumRowCount;
		}

		public CsvTooManyRowsException(int maximumRowCount, Exception innerException) : base(DataStrings.TooManyRows(maximumRowCount), innerException)
		{
			this.maximumRowCount = maximumRowCount;
		}

		protected CsvTooManyRowsException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.maximumRowCount = (int)info.GetValue("maximumRowCount", typeof(int));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("maximumRowCount", this.maximumRowCount);
		}

		public int MaximumRowCount
		{
			get
			{
				return this.maximumRowCount;
			}
		}

		private readonly int maximumRowCount;
	}
}
