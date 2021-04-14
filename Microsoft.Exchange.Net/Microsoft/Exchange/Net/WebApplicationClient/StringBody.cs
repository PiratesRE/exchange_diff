using System;
using System.IO;

namespace Microsoft.Exchange.Net.WebApplicationClient
{
	internal class StringBody : TextBody
	{
		public StringBody(string body)
		{
			this.Body = body;
		}

		public string Body { get; private set; }

		public override void Write(TextWriter writer)
		{
			if (this.Body != null)
			{
				writer.Write(this.Body);
			}
		}
	}
}
