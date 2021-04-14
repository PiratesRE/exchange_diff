using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class InvalidIdTooManyAttachmentLevelsException : StoragePermanentException
	{
		internal InvalidIdTooManyAttachmentLevelsException() : base(LocalizedString.Empty)
		{
		}

		private InvalidIdTooManyAttachmentLevelsException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
