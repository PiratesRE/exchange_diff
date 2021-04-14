using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal abstract class RtfCallbackBase : ConversionCallbackBase
	{
		protected RtfCallbackBase()
		{
		}

		protected RtfCallbackBase(ICoreItem coreItem) : base(coreItem)
		{
		}

		protected RtfCallbackBase(CoreAttachmentCollection collection, Body itemBody) : base(collection, itemBody)
		{
		}

		public abstract bool ProcessImage(string imageUrl, int approximateRenderingPosition);
	}
}
