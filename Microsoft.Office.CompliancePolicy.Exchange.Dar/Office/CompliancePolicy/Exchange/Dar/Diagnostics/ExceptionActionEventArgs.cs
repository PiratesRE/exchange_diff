using System;

namespace Microsoft.Office.CompliancePolicy.Exchange.Dar.Diagnostics
{
	internal class ExceptionActionEventArgs : EventArgs
	{
		public ExceptionActionEventArgs(Exception exception, ExceptionAction requestedAction)
		{
			this.Exception = exception;
			this.RequestedAction = requestedAction;
		}

		public ExceptionAction RequestedAction { get; private set; }

		public Exception Exception { get; private set; }
	}
}
