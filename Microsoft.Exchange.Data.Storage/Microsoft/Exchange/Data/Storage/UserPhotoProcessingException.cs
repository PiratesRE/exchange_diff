using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal class UserPhotoProcessingException : StoragePermanentException
	{
		public UserPhotoProcessingException(LocalizedString msg) : base(msg)
		{
		}

		protected UserPhotoProcessingException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
