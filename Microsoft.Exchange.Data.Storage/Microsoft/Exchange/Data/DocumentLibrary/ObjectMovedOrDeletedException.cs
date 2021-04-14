using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.DocumentLibrary
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal class ObjectMovedOrDeletedException : ObjectNotFoundException
	{
		internal ObjectMovedOrDeletedException(ObjectId objectId, string uri) : this(objectId, uri, null)
		{
		}

		internal ObjectMovedOrDeletedException(ObjectId objectId, string uri, Exception innerException) : base(objectId, Strings.ExObjectMovedOrDeleted(uri), innerException)
		{
			this.uri = uri;
		}

		public string Uri
		{
			get
			{
				return this.uri;
			}
		}

		private readonly string uri;
	}
}
