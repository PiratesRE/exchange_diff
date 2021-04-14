using System;

namespace Microsoft.Exchange.Common
{
	public class FailFastException : Exception
	{
		internal FailFastException(string message, string stackTrace) : base(message)
		{
			this.stackTrace = stackTrace;
		}

		public override string StackTrace
		{
			get
			{
				if (string.IsNullOrEmpty(base.StackTrace))
				{
					return this.stackTrace;
				}
				return base.StackTrace;
			}
		}

		private string stackTrace;
	}
}
