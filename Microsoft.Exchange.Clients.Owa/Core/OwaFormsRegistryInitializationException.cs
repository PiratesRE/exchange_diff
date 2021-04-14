using System;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	[Serializable]
	public sealed class OwaFormsRegistryInitializationException : OwaPermanentException
	{
		public OwaFormsRegistryInitializationException(string message, Exception innerException) : base(message, innerException, null)
		{
		}
	}
}
