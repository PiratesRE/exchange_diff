using System;

namespace Microsoft.Exchange.Search.Core.Abstraction
{
	internal class FailedEventArgs : EventArgs
	{
		internal FailedEventArgs(ComponentFailedException exception)
		{
			this.exception = exception;
		}

		internal ComponentFailedException Reason
		{
			get
			{
				return this.exception;
			}
		}

		private readonly ComponentFailedException exception;
	}
}
