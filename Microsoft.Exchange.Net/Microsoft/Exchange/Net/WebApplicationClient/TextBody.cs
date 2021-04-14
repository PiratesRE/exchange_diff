using System;
using System.IO;
using System.Text;

namespace Microsoft.Exchange.Net.WebApplicationClient
{
	internal abstract class TextBody : RequestBody
	{
		public sealed override void Write(Stream writeStream)
		{
			using (StreamWriter streamWriter = new StreamWriter(writeStream, Encoding.ASCII))
			{
				this.Write(streamWriter);
			}
		}

		public abstract void Write(TextWriter writer);
	}
}
