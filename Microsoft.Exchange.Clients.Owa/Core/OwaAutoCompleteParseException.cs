using System;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	[Serializable]
	public sealed class OwaAutoCompleteParseException : OwaPermanentException
	{
		public OwaAutoCompleteParseException(string message, Exception innerException, object thisObject) : base(message, innerException, thisObject)
		{
		}
	}
}
