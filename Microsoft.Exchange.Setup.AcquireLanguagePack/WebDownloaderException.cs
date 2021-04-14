using System;

namespace Microsoft.Exchange.Setup.AcquireLanguagePack
{
	internal class WebDownloaderException : ApplicationException
	{
		public WebDownloaderException(string message) : base(message)
		{
			Logger.LogError(this);
		}
	}
}
