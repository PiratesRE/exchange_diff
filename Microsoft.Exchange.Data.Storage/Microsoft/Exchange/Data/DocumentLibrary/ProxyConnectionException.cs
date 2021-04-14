using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.DocumentLibrary
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal class ProxyConnectionException : ConnectionException
	{
		internal ProxyConnectionException(ObjectId objectId, string message, Exception innerException) : base(objectId, message, innerException)
		{
		}
	}
}
