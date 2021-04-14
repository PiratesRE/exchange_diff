using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal class NonCanonicalACLException : StoragePermanentException
	{
		public NonCanonicalACLException(string canonicalErrorInformation) : base(ServerStrings.NonCanonicalACL(canonicalErrorInformation))
		{
		}

		protected NonCanonicalACLException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
