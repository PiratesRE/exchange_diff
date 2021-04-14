using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal class ACLTooBigException : StoragePermanentException
	{
		public ACLTooBigException() : base(ServerStrings.ACLTooBig)
		{
		}

		protected ACLTooBigException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
