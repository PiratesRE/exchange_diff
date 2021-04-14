using System;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	[Serializable]
	public sealed class OwaBodyConversionFailedException : OwaPermanentException
	{
		public OwaBodyConversionFailedException(string message, Exception innerException) : base(message, innerException)
		{
		}
	}
}
