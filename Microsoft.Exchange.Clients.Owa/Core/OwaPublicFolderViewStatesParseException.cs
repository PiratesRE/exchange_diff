using System;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	[Serializable]
	public sealed class OwaPublicFolderViewStatesParseException : OwaPermanentException
	{
		public OwaPublicFolderViewStatesParseException(string message, Exception innerException, object thisObject) : base(message, innerException, thisObject)
		{
		}
	}
}
