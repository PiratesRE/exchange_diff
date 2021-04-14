using System;

namespace Microsoft.Exchange.Configuration.Tasks
{
	public class TaskErrorEventArg
	{
		public TaskErrorEventArg(Exception exception, bool? isUnknownException)
		{
			this.Exception = exception;
			this.IsUnknownException = isUnknownException;
		}

		public Exception Exception { get; private set; }

		public bool ExceptionHandled { get; set; }

		public bool? IsUnknownException { get; set; }
	}
}
