using System;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	[Serializable]
	public sealed class OwaConfigurationParserException : OwaPermanentException
	{
		public OwaConfigurationParserException(string message, Exception innerException, object thisObject) : base(message, innerException, thisObject)
		{
		}
	}
}
