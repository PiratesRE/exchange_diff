using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal class UserPhotoNotAnImageException : UserPhotoProcessingException
	{
		public UserPhotoNotAnImageException() : base(ClientStrings.UserPhotoNotAnImage)
		{
		}

		protected UserPhotoNotAnImageException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
