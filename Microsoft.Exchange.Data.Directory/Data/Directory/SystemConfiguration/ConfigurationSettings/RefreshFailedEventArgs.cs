using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration.ConfigurationSettings
{
	internal sealed class RefreshFailedEventArgs : EventArgs
	{
		public RefreshFailedEventArgs(Exception e)
		{
			this.Exception = e;
		}

		public Exception Exception { get; private set; }
	}
}
