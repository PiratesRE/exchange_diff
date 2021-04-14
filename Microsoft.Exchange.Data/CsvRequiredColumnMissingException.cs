using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Data
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class CsvRequiredColumnMissingException : CsvValidationException
	{
		public CsvRequiredColumnMissingException(string missingColumn) : base(DataStrings.RequiredColumnMissing(missingColumn))
		{
			this.missingColumn = missingColumn;
		}

		public CsvRequiredColumnMissingException(string missingColumn, Exception innerException) : base(DataStrings.RequiredColumnMissing(missingColumn), innerException)
		{
			this.missingColumn = missingColumn;
		}

		protected CsvRequiredColumnMissingException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.missingColumn = (string)info.GetValue("missingColumn", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("missingColumn", this.missingColumn);
		}

		public string MissingColumn
		{
			get
			{
				return this.missingColumn;
			}
		}

		private readonly string missingColumn;
	}
}
