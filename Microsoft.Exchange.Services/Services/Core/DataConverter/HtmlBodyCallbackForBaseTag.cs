using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.TextConverters;

namespace Microsoft.Exchange.Services.Core.DataConverter
{
	internal class HtmlBodyCallbackForBaseTag : HtmlCallbackBase
	{
		public Uri BaseHref { get; private set; }

		public override void ProcessTag(HtmlTagContext htmlTagContext, HtmlWriter htmlWriter)
		{
			if (htmlTagContext.TagId == HtmlTagId.Base)
			{
				foreach (HtmlTagContextAttribute htmlTagContextAttribute in htmlTagContext.Attributes)
				{
					if (htmlTagContextAttribute.Id == HtmlAttributeId.Href)
					{
						string value = htmlTagContextAttribute.Value;
						this.BaseHref = HtmlBodyCallback.TryParseUri(value, UriKind.Absolute);
						break;
					}
				}
			}
		}
	}
}
