using System;
using System.Text;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.DocumentLibrary
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal class ClassifyResult
	{
		internal ClassifyResult(Uri originalUri, ClassificationError error)
		{
			this.originalUri = originalUri;
			this.error = error;
		}

		internal ClassifyResult(DocumentLibraryObjectId objectId, Uri originalUri, UriFlags uriFlags)
		{
			this.objectId = objectId;
			this.originalUri = originalUri;
			this.uriFlags = uriFlags;
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder(100);
			if (this.error != ClassificationError.None)
			{
				stringBuilder.AppendFormat("UriFlags:{0},\tOriginalUri:{1},\tObjectId:{2}", this.uriFlags, this.originalUri, this.objectId);
			}
			else
			{
				stringBuilder.AppendFormat("OriginalUri:{0}, Error:{1}", this.originalUri, this.error);
			}
			return stringBuilder.ToString();
		}

		public DocumentLibraryObjectId ObjectId
		{
			get
			{
				return this.objectId;
			}
		}

		public Uri OriginalUri
		{
			get
			{
				return this.originalUri;
			}
		}

		public ClassificationError Error
		{
			get
			{
				return this.error;
			}
		}

		public UriFlags UriFlags
		{
			get
			{
				return this.uriFlags;
			}
		}

		private Uri originalUri;

		private ClassificationError error;

		private UriFlags uriFlags = UriFlags.Other;

		private DocumentLibraryObjectId objectId;
	}
}
