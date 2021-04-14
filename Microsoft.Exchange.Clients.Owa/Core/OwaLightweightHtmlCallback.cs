using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.TextConverters;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	internal class OwaLightweightHtmlCallback : HtmlCallbackBase
	{
		public Uri BaseRef
		{
			get
			{
				return this.baseRef;
			}
		}

		public override void ProcessTag(HtmlTagContext context, HtmlWriter writer)
		{
			if (context.TagId == HtmlTagId.Base)
			{
				foreach (HtmlTagContextAttribute attribute in context.Attributes)
				{
					if (OwaSafeHtmlCallbackBase.IsBaseTag(context.TagId, attribute))
					{
						string value = attribute.Value;
						this.baseRef = Utilities.TryParseUri(value);
						break;
					}
				}
			}
		}

		protected Uri baseRef;
	}
}
