using System;

namespace Microsoft.Exchange.Setup.AcquireLanguagePack
{
	internal class WebDownloaderEventArgs : EventArgs
	{
		public static Exception ErrorException
		{
			get
			{
				return WebDownloaderEventArgs.errorException;
			}
			set
			{
				WebDownloaderEventArgs.errorException = value;
			}
		}

		private static Exception errorException;
	}
}
