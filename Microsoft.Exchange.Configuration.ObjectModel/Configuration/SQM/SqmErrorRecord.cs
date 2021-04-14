using System;

namespace Microsoft.Exchange.Configuration.SQM
{
	internal class SqmErrorRecord
	{
		internal SqmErrorRecord(string exceptionType, string errorId)
		{
			this.ExceptionType = exceptionType;
			this.ErrorId = errorId;
		}

		public string ExceptionType { get; private set; }

		public string ErrorId { get; private set; }
	}
}
