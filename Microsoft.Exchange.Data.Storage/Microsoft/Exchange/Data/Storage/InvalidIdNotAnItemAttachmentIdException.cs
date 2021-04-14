using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class InvalidIdNotAnItemAttachmentIdException : StoragePermanentException
	{
		internal InvalidIdNotAnItemAttachmentIdException() : base(LocalizedString.Empty)
		{
		}

		private InvalidIdNotAnItemAttachmentIdException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
