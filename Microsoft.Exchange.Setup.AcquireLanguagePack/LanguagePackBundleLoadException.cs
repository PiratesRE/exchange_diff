using System;

namespace Microsoft.Exchange.Setup.AcquireLanguagePack
{
	internal class LanguagePackBundleLoadException : ApplicationException
	{
		public LanguagePackBundleLoadException(string message) : base(message)
		{
			Logger.LogError(this);
		}
	}
}
