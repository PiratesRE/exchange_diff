using System;

namespace Microsoft.Exchange.Server.Storage.PhysicalAccess
{
	public class NullColumnException : Exception
	{
		public NullColumnException(Column column) : base(string.Format("Column {0} was null so a read only stream cannot be opened", column.Name))
		{
		}

		public NullColumnException(Column column, Exception innerException) : base(string.Format("Column {0} was null so a read only stream cannot be opened", column.Name), innerException)
		{
		}

		private const string NullColumnMessage = "Column {0} was null so a read only stream cannot be opened";
	}
}
