using System;
using System.Text;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	internal class ContentFilterPhraseQueryFilter : QueryFilter
	{
		public ContentFilterPhraseQueryFilter(string phrase)
		{
			this.phrase = phrase;
		}

		public override void ToString(StringBuilder sb)
		{
			sb.Append("(");
			sb.Append(this.phrase);
			sb.Append(")");
		}

		public string Phrase
		{
			get
			{
				return this.phrase;
			}
		}

		private string phrase;
	}
}
