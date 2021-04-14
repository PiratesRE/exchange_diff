using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Transport.Sync.Manager
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal class VersionMismatchException : SerializationException
	{
		public VersionMismatchException(string message) : base(message)
		{
		}
	}
}
