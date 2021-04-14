using System;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	[Serializable]
	public class OwaSpellCheckerException : OwaPermanentException
	{
		public OwaSpellCheckerException(string message, Exception innerException) : base(message, innerException)
		{
		}

		public OwaSpellCheckerException(string message) : base(message)
		{
		}
	}
}
