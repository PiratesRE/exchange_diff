using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.DocumentLibrary
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal class PathTooLongException : DocumentLibraryException
	{
		internal PathTooLongException(ObjectId objectId, string message, Exception innerException) : base(message, innerException)
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
