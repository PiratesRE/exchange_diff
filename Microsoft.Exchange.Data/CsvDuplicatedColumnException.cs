using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Data
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class CsvDuplicatedColumnException : CsvValidationException
	{
		public CsvDuplicatedColumnException(string duplicatedColumn) : base(DataStrings.DuplicatedColumn(duplicatedColumn))
		{
			this.duplicatedColumn = duplicatedColumn;
		}

		public CsvDuplicatedColumnException(string duplicatedColumn, Exception innerException) : base(DataStrings.DuplicatedColumn(duplicatedColumn), innerException)
		{
			this.duplicatedColumn = duplicatedColumn;
		}

		protected CsvDuplicatedColumnException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.duplicatedColumn = (string)info.GetValue("duplicatedColumn", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("duplicatedColumn", this.duplicatedColumn);
		}

		public string DuplicatedColumn
		{
			get
			{
				return this.duplicatedColumn;
			}
		}

		private readonly string duplicatedColumn;
	}
}
