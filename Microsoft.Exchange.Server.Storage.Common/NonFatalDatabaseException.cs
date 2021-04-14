using System;

namespace Microsoft.Exchange.Server.Storage.Common
{
	public class NonFatalDatabaseException : Exception
	{
		public NonFatalDatabaseException(string message) : base(message)
		{
			this.errorCode = ErrorCodeValue.DatabaseError;
		}

		public NonFatalDatabaseException(string message, Exception innerException) : base(message, innerException)
		{
			this.errorCode = ErrorCodeValue.DatabaseError;
		}

		public NonFatalDatabaseException(ErrorCodeValue errorCode, string message, Exception innerException) : base(message, innerException)
		{
			this.errorCode = errorCode;
		}

		public ErrorCodeValue Error
		{
			get
			{
				return this.errorCode;
			}
		}

		private const string ErrorCodeSerializationLabel = "errorCode";

		private ErrorCodeValue errorCode;
	}
}
