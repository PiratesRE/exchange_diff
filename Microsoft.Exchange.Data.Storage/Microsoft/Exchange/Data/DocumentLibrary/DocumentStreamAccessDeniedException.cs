using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.DocumentLibrary
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal class DocumentStreamAccessDeniedException : AccessDeniedException
	{
		internal DocumentStreamAccessDeniedException(ObjectId objectId, string uri, Exception innerException) : base(objectId, Strings.ExDocumentStreamAccessDenied(uri), innerException)
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
