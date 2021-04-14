using System;
using System.Text;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[Serializable]
	internal class ContentFilterPhraseIdentity : ObjectId
	{
		public ContentFilterPhraseIdentity(string phrase)
		{
			this.phrase = phrase;
		}

		public override byte[] GetBytes()
		{
			return Encoding.Unicode.GetBytes(this.phrase);
		}

		public override string ToString()
		{
			return this.phrase;
		}

		private string phrase;
	}
}
