using System;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.UM.UMCore
{
	internal class EmailNormalizedText
	{
		internal EmailNormalizedText(string inText)
		{
			inText = (inText ?? string.Empty);
			this.text = SpeechUtils.XmlEncode(Util.EmailNormalize(inText));
		}

		public override string ToString()
		{
			return this.text;
		}

		private string text;
	}
}
