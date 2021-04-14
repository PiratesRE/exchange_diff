using System;

namespace Microsoft.Exchange.HttpProxy
{
	[Serializable]
	internal class InvalidBackEndCookieException : Exception
	{
		public InvalidBackEndCookieException() : base(InvalidBackEndCookieException.ErrorMessage)
		{
		}

		private static readonly string ErrorMessage = "Invalid back end cookie entry.";
	}
}
