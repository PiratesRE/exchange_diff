using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal class UserPhotoFileTooSmallException : UserPhotoProcessingException
	{
		public UserPhotoFileTooSmallException() : base(ClientStrings.UserPhotoFileTooSmall(0))
		{
		}

		protected UserPhotoFileTooSmallException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
