using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.ApplicationLogic.UserPhotos
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal class TooComplexPhotoRequestException : Exception
	{
		public TooComplexPhotoRequestException()
		{
		}

		public TooComplexPhotoRequestException(string message) : base(message)
		{
		}

		public TooComplexPhotoRequestException(string message, Exception inner) : base(message, inner)
		{
		}

		protected TooComplexPhotoRequestException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
