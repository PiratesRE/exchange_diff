using System;
using System.IO;
using System.Net.Mime;

namespace Microsoft.Exchange.Net.WebApplicationClient
{
	internal class StreamBody : RequestBody
	{
		public StreamBody(Stream stream, string contentType)
		{
			this.Stream = stream;
			base.ContentType = new ContentType(contentType);
		}

		public Stream Stream { get; private set; }

		public override void Write(Stream writeStream)
		{
			if (this.Stream != null)
			{
				this.Stream.CopyTo(writeStream);
			}
		}
	}
}
