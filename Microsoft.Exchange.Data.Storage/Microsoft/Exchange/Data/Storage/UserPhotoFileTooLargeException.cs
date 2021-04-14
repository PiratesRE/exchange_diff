using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal class UserPhotoFileTooLargeException : UserPhotoProcessingException
	{
		public UserPhotoFileTooLargeException() : base(ClientStrings.UserPhotoFileTooLarge(20))
		{
		}

		protected UserPhotoFileTooLargeException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
