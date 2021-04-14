using System;

namespace Microsoft.Exchange.Diagnostics
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class ObjectNotDisposedException<T> : Exception where T : IDisposable
	{
		public ObjectNotDisposedException(string ctorStackTrace, bool wasReset) : this()
		{
			this.stackTrace = ctorStackTrace;
			this.wasReset = wasReset;
		}

		private ObjectNotDisposedException()
		{
		}

		public override string StackTrace
		{
			get
			{
				return this.stackTrace;
			}
		}

		public override string Message
		{
			get
			{
				string str = "This object implements interface IDisposable and its Dispose method was never called." + Environment.NewLine;
				return str + (this.wasReset ? "The stack trace reflects the last point where SetReportedStacktraceToCurrentLocation was called." : "The stack trace was taken at the time the object was constructed.");
			}
		}

		public override string ToString()
		{
			string text = base.ToString();
			if (this.stackTrace != null)
			{
				text = text + Environment.NewLine + this.stackTrace;
			}
			return text;
		}

		private string stackTrace;

		private bool wasReset;
	}
}
