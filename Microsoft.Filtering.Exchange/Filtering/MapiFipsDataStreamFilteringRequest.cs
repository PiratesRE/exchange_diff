using System;
using System.IO;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.UnifiedContent;
using Microsoft.Exchange.UnifiedContent.Exchange;

namespace Microsoft.Filtering
{
	internal sealed class MapiFipsDataStreamFilteringRequest : FipsDataStreamFilteringRequest
	{
		private MapiFipsDataStreamFilteringRequest(Item item, IExtendedMapiFilteringContext context, string id, ContentManager contentManager) : base(id, contentManager)
		{
			this.Item = item;
			this.Context = context;
		}

		public Item Item { get; private set; }

		public IExtendedMapiFilteringContext Context { get; private set; }

		public override RecoveryOptions RecoveryOptions
		{
			get
			{
				return this.Context.GetFipsRecoveryOptions();
			}
			set
			{
				this.Context.SetFipsRecoveryOptions(value);
			}
		}

		public static MapiFipsDataStreamFilteringRequest CreateInstance(Item item, IExtendedMapiFilteringContext context)
		{
			if (item == null)
			{
				throw new ArgumentNullException("item");
			}
			if (context == null)
			{
				throw new ArgumentNullException("context");
			}
			string id = item.Id.ToString();
			ContentManager contentManager = new ContentManager(Path.GetTempPath());
			return new MapiFipsDataStreamFilteringRequest(item, context, id, contentManager);
		}

		protected override void Serialize(UnifiedContentSerializer unifiedContentSerializer, bool bypassBodyTextTruncation = true)
		{
			this.Item.Serialize(this.Context, unifiedContentSerializer, bypassBodyTextTruncation);
		}
	}
}
