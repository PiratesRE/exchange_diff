using System;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	[Serializable]
	public class OwaCreateClientSecurityContextFailedException : OwaTransientException
	{
		public OwaCreateClientSecurityContextFailedException(string message) : base(message)
		{
		}
	}
}
