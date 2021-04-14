using System;
using Microsoft.Exchange.Data.TextConverters;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	internal sealed class OwaSafeHtmlWebReadyCallbacks : OwaSafeHtmlOutboundCallbacks
	{
		public OwaSafeHtmlWebReadyCallbacks(string documentID) : base(OwaContext.Current, true, true)
		{
			this.urlPrefix = string.Concat(new string[]
			{
				"ev.owa?ns=WebReady&ev=GetFile",
				Utilities.GetCanaryRequestParameter(),
				"&d=",
				documentID,
				"&fileName="
			});
		}

		protected override void ProcessImageTag(HtmlTagContextAttribute filterAttribute, HtmlTagContext context, HtmlWriter writer)
		{
			writer.WriteAttribute(filterAttribute.Id, this.urlPrefix + filterAttribute.Value);
		}

		public override void ProcessTag(HtmlTagContext context, HtmlWriter writer)
		{
			if (context == null)
			{
				throw new ArgumentNullException("context");
			}
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			if (OwaSafeHtmlWebReadyCallbacks.IsStyleSheetLinkTag(context))
			{
				context.WriteTag();
				using (HtmlTagContext.AttributeCollection.Enumerator enumerator = context.Attributes.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						HtmlTagContextAttribute htmlTagContextAttribute = enumerator.Current;
						if (htmlTagContextAttribute.Id == HtmlAttributeId.Href)
						{
							writer.WriteAttribute(htmlTagContextAttribute.Id, this.urlPrefix + htmlTagContextAttribute.Value);
						}
						else
						{
							htmlTagContextAttribute.Write();
						}
					}
					return;
				}
			}
			base.ProcessTag(context, writer);
		}

		private static bool IsStyleSheetLinkTag(HtmlTagContext context)
		{
			if (context.TagId != HtmlTagId.Link)
			{
				return false;
			}
			foreach (HtmlTagContextAttribute htmlTagContextAttribute in context.Attributes)
			{
				if (htmlTagContextAttribute.Id == HtmlAttributeId.Rel && !string.IsNullOrEmpty(htmlTagContextAttribute.Value) && htmlTagContextAttribute.Value.Equals("stylesheet", StringComparison.OrdinalIgnoreCase))
				{
					return true;
				}
			}
			return false;
		}

		protected override void ProcessUseMapAttribute(HtmlTagContextAttribute filterAttribute, HtmlTagContext context, HtmlWriter writer)
		{
			if (context.TagId == HtmlTagId.Img)
			{
				filterAttribute.Write();
				return;
			}
			base.ProcessTag(context, writer);
		}

		private string urlPrefix;
	}
}
