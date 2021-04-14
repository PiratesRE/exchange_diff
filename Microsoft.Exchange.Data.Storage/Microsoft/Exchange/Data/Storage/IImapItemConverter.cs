using System;
using System.IO;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal abstract class IImapItemConverter : DisposableObject
	{
		public abstract MimePartInfo GetMimeStructure();

		public abstract void GetBody(Stream outStream);

		public abstract bool GetBody(Stream outStream, uint[] indices);

		public abstract void GetText(Stream outStream);

		public abstract bool GetText(Stream outStream, uint[] indices);

		public abstract MimePartHeaders GetHeaders();

		public abstract MimePartHeaders GetHeaders(uint[] indices);

		public abstract MimePartHeaders GetMime(uint[] indices);

		public abstract bool ItemNeedsSave { get; }
	}
}
