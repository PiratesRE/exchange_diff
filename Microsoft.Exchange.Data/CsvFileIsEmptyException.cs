using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Data
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class CsvFileIsEmptyException : CsvValidationException
	{
		public CsvFileIsEmptyException() : base(DataStrings.FileIsEmpty)
		{
		}

		public CsvFileIsEmptyException(Exception innerException) : base(DataStrings.FileIsEmpty, innerException)
		{
		}

		protected CsvFileIsEmptyException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
