using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Migration
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class MigrationCSVParsingException : MigrationPermanentException
	{
		public MigrationCSVParsingException(int rowIndex, string errorMessage) : base(Strings.ErrorParsingCSV(rowIndex, errorMessage))
		{
			this.rowIndex = rowIndex;
			this.errorMessage = errorMessage;
		}

		public MigrationCSVParsingException(int rowIndex, string errorMessage, Exception innerException) : base(Strings.ErrorParsingCSV(rowIndex, errorMessage), innerException)
		{
			this.rowIndex = rowIndex;
			this.errorMessage = errorMessage;
		}

		protected MigrationCSVParsingException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.rowIndex = (int)info.GetValue("rowIndex", typeof(int));
			this.errorMessage = (string)info.GetValue("errorMessage", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("rowIndex", this.rowIndex);
			info.AddValue("errorMessage", this.errorMessage);
		}

		public int RowIndex
		{
			get
			{
				return this.rowIndex;
			}
		}

		public string ErrorMessage
		{
			get
			{
				return this.errorMessage;
			}
		}

		private readonly int rowIndex;

		private readonly string errorMessage;
	}
}
