using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal class UserPhotoNotFoundException : StoragePermanentException
	{
		public UserPhotoNotFoundException(bool preview) : base(preview ? ClientStrings.UserPhotoPreviewNotFound : ClientStrings.UserPhotoNotFound)
		{
		}

		protected UserPhotoNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
