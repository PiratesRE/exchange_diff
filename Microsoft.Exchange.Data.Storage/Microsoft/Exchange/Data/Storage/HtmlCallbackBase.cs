using System;
using Microsoft.Exchange.Data.TextConverters;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal abstract class HtmlCallbackBase : ConversionCallbackBase
	{
		protected HtmlCallbackBase()
		{
		}

		protected HtmlCallbackBase(Item item) : this(item.CoreItem)
		{
		}

		protected HtmlCallbackBase(ICoreItem item) : base(item)
		{
		}

		protected HtmlCallbackBase(AttachmentCollection collection, Body itemBody) : this(collection.CoreAttachmentCollection, itemBody)
		{
		}

		protected HtmlCallbackBase(CoreAttachmentCollection collection, Body itemBody) : base(collection, itemBody)
		{
		}

		public abstract void ProcessTag(HtmlTagContext tagContext, HtmlWriter writer);
	}
}
