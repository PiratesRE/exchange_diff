using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.DocumentLibrary
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal class AccessDeniedException : DocumentLibraryException
	{
		internal AccessDeniedException(ObjectId objectId, string message) : this(objectId, message, null)
		{
		}

		internal AccessDeniedException(ObjectId objectId, string message, Exception innerException) : base(message, innerException)
		{
			this.objectId = objectId;
		}

		public ObjectId ObjectId
		{
			get
			{
				return this.objectId;
			}
		}

		private readonly ObjectId objectId;
	}
}
