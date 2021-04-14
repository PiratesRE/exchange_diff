using System;

namespace Microsoft.Exchange.EdgeSync.Ehf
{
	internal sealed class EhfTransientFailureInfo
	{
		public EhfTransientFailureInfo(Exception failureException, string operationName)
		{
			this.failureException = failureException;
			this.operationName = operationName;
		}

		public Exception FailureException
		{
			get
			{
				return this.failureException;
			}
		}

		public string OperationName
		{
			get
			{
				return this.operationName;
			}
		}

		private readonly Exception failureException;

		private readonly string operationName;
	}
}
