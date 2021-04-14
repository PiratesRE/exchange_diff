using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.DocumentLibrary
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal class DocumentModifiedException : AccessDeniedException
	{
		internal DocumentModifiedException(ObjectId objectId, string uri) : base(objectId, Strings.ExDocumentModified(uri))
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
