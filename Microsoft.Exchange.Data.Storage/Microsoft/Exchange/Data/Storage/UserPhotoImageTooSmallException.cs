using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal class UserPhotoImageTooSmallException : UserPhotoProcessingException
	{
		public UserPhotoImageTooSmallException() : base(ClientStrings.UserPhotoImageTooSmall(UserPhotoUtilities.MinImageSize))
		{
		}

		protected UserPhotoImageTooSmallException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
