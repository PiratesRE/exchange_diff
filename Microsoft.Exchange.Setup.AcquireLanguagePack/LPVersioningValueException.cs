using System;

namespace Microsoft.Exchange.Setup.AcquireLanguagePack
{
	internal class LPVersioningValueException : ApplicationException
	{
		public LPVersioningValueException(string message) : base(message)
		{
			Logger.LogError(this);
		}
	}
}
