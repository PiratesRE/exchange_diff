using System;
using System.IO;
using System.Net;

namespace Microsoft.Exchange.Net.WebApplicationClient
{
	internal class TextResponse : WebApplicationResponse
	{
		public override void SetResponse(HttpWebResponse response)
		{
			base.SetResponse(response);
			using (StreamReader streamReader = new StreamReader(response.GetResponseStream()))
			{
				this.Text = streamReader.ReadToEnd();
			}
		}

		public string Text { get; private set; }

		protected bool Contains(string text)
		{
			return this.Text.IndexOf(text, StringComparison.OrdinalIgnoreCase) >= 0;
		}
	}
}
